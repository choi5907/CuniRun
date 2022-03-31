using UnityEngine;

// 발판으로서 필요한 동작을 담은 스크립트
public class Platform : MonoBehaviour {
    public GameObject[] obstacles; // 장애물 오브젝트들 ( 배열로 여러개를 담음 )
    private bool stepped = false; // 플레이어 캐릭터가 밟았었는가
    public GameObject[] coins; // 코인 오브젝트 ( 배열로 담기 )

    // 컴포넌트가 활성화될때 마다 매번 실행되는 메서드
    private void OnEnable() {
        // 발판을 리셋하는 처리
        stepped = false;

        // 장애물의 수만큼 루프
        for (int i = 0; i < obstacles.Length; i++) {
            // 현재 순번의 장애물을 1/3의 확률로 활성화
            if(Random.Range(0,3) == 0) {
                obstacles[i].SetActive(true);
            } else {
                obstacles[i].SetActive(false);
            }
        }
        for (int i = 0; i < coins.Length; i++) {
            // 코인 수 만큼 확률적으로 활성화
            if(Random.Range(0,5) == 0) {
                coins[i].SetActive(true);
            } else {
                coins[i].SetActive(false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // 플레이어 캐릭터가 자신을 밟았을때 점수를 추가하는 처리
        if(collision.collider.tag == "Player" && !stepped) {
            // 점수를 추가하고 밟힘 상태를 참으로 변경
            stepped = true;
            GameManager.instance.AddScore(1);
            // AddScore에 싱글턴으로 접근하여 1점 추가
        }
    }

}