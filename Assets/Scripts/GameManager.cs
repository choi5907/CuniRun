using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// 게임 오버 상태를 표현하고, 게임 점수와 UI를 관리하는 게임 매니저
// 씬에는 단 하나의 게임 매니저만 존재할 수 있다.
public class GameManager : MonoBehaviour {
    public static GameManager instance; // 싱글톤을 할당할 전역 변수

    public bool isGameover = false; // 게임 오버 상태
    public Text scoreText; // 점수를 출력할 UI 텍스트
    public GameObject gameoverUI; // 게임 오버시 활성화 할 UI 게임 오브젝트
    public Text lifeText; // 목숨을 나타낼 UI 텍스트

    private int score = 0; // 게임 점수
    private int hpscore = 3; // hp 개수

    public Button menuButton, Continue, reTry, Exit;
    public GameObject menuPanel;

    // 게임 시작과 동시에 싱글톤을 구성
    void Awake() {
        // 싱글톤 변수 instance가 비어있는가?
        if (instance == null)
        {
            // instance가 비어있다면(null) 그곳에 자기 자신을 할당
            instance = this;
        }
        else
        {
            // instance에 이미 다른 GameManager 오브젝트가 할당되어 있는 경우

            // 씬에 두개 이상의 GameManager 오브젝트가 존재한다는 의미.
            // 싱글톤 오브젝트는 하나만 존재해야 하므로 자신의 게임 오브젝트를 파괴
            Debug.LogWarning("씬에 두개 이상의 게임 매니저가 존재합니다!");
            Destroy(gameObject);
        }
    }
    private void Start() {
        // hp 개수표시, 메뉴패널 비활성화, 버튼 AddListener 함수사용
        lifeText.text = hpscore.ToString();
        menuPanel.SetActive(false);
        menuButton.onClick.AddListener(callMenu);
        Continue.onClick.AddListener(callCon);
        reTry.onClick.AddListener(callRe);
        Exit.onClick.AddListener(callExit);
    }

    void Update() {
        // 게임 오버 상태에서 게임을 재시작할 수 있게 하는 처리
        if(isGameover && Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // SceneManager.GetActivScene() 현재 활성화된 씬의 정보를
            // Scene형태의 오브젝트로 가져오는 메서드 .name은 이름을 가져옴
            // 그 이름의 씬을 다시 로드
        }
    }

    // 점수를 증가시키는 메서드
    public void AddScore(int newScore) {
        // 게임오버가 아니라면
        if (!isGameover) {
            // 점수를 증가
            score += newScore;
            scoreText.text = "Score : " + score;
        }
    }

    // 플레이어 캐릭터가 사망시 게임 오버를 실행하는 메서드
    public void OnPlayerDead() {
        isGameover = true;
        gameoverUI.SetActive(true);
        // gameover 활성화 gameoverUI에 할당 된 오브젝트 활성화
    }
    
    // 체력을 감소시키는 메서드
    public bool DehpScore() {
        hpscore -= 1;
            lifeText.text = "= " + hpscore;
        if ( hpscore > 0 )return true;
        return false;
    }
    // 메뉴 불러오기
    public void callMenu() {
        Time.timeScale = 0;
        menuPanel.SetActive(true);
    }
    // 계속하기
    public void callCon() {
        Time.timeScale = 1;
        menuPanel.SetActive(false);
    }
    // 다시시작
    public void callRe() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
    public void callExit() {
    // #if else로 유니티 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}