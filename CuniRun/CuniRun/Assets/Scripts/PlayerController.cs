using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour {
   public AudioClip deathClip; // 사망시 재생할 오디오 클립
   public float jumpForce = 700f; // 점프 힘

   private int jumpCount = 0; // 누적 점프 횟수
   private bool isGrounded = false; // 바닥에 닿았는지 나타냄
   private bool isDead = false; // 사망 상태

   private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
   private Animator animator; // 사용할 애니메이터 컴포넌트
   private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

   private void Start() {
        // 초기화
        // 게임 오브젝트로부터 사용할 컴포넌트들을 가져와 변수에 할당
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
   }

   private void Update() {
        // 사용자 입력을 감지하고 점프하는 처리
        if (isDead) {
            // 사망 시 처리를 더 이상 진행하지 않고 종료
            return;
        }
        // 마우스 왼쪽 버튼을 눌렀으며 && 최대 점프 횟수(2)에 도달하지 않았다면
        if (Input.GetMouseButtonDown(0) && jumpCount < 2) {
            // Input.GetMouseButtonDown() : 0 - 왼, 1 - 오, 2 - 휠 스크롤
            // 점프 횟수 증가
            jumpCount++;
            // 점프 직전에 속도를 순간적으로 제로(0, 0)로 변경
            playerRigidbody.velocity = Vector2.zero;
            // 2D이므로 Vector2를 사용
            // Vector2.zero = new Vector2(0, 0)
            // 리지드바디에 위쪽으로 힘 주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            // 오디오 소스 재생
            playerAudio.Play();
        // Input.GetMouseButton Down - 누르는 순간, x - 누르고 있는 동안, Up - 손을 떼는 순간
        } else if (Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0) {
            // 마우스 왼쪽 버튼에서 손을 떼는 순간 && 속도의 y 값이 양수라면(위로 상승중)
            // 현재 속도를 절반으로 변경
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
            // velocity의 속도는 +일 때 상승 -일 떄 낙하이므로 0보다 클 때 줄여야함.
        }

        // 애니메이터의 Grounded 파라미터를 isGrounded 값으로 갱신
        animator.SetBool("Grounded", isGrounded);
        // animator 컴포넌트의 타입은 파라미터 값을 변경 할 수 있는 Set 메서드 제공
        // SetBool(파라미터 이름 : name, 해당 파라미터에 할당할 값 : value)
        // Grounded의 isGrounded를 SetBool을 사용하여 전역변수인
        // isGrounded의 false값을 입력한다.
    }

    private void Die() {
        // 사망 처리
        // 애니메이터의 Die 트리거 파라미터를 셋
        animator.SetTrigger("Die");
        // SetTrigger는 파라미터로 방아쇠를 당긴다.
        // animator에서 Die 파라미터 생성 -> 메서드로 Die 작성 ->
        // animator.SetTrigger로 에니메이터 Die를 불러온다.

        // 오디오 소스에 할당된 오디오 클립을 deathClip으로 변경
        playerAudio.clip = deathClip;
        // 사망 효과음 재생
        playerAudio.Play();

        // 속도를 제로(0, 0)로 변경
        playerRigidbody.velocity = Vector2.zero;
        // 사망 상태를 true로 변경
        isDead = true;
        // 애니메이터 > 코드 > 사운드 > 속도 > Dead

        // 게임 매니저의 게임오버 처리 실행 : 플레이어가 죽고나서 실행되야하므로
        GameManager.instance.OnPlayerDead();
   }

    // 충돌감지 부분 OnTriggerEnter, 2D 콜라이더이므로 2D붙은 메서드 사용
   private void OnTriggerEnter2D(Collider2D other) {
        // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
        if (other.tag == "Dead" && !isDead) {
            // 충돌한 상대방의 태그가 Dead이며 아직 사망하지 않았다면 Die() 실행
            Die();
        }
        // Dead tag와 충돌할 경우 Die메서드 실행

        if (other.tag == "DeLife" && !isDead) {
            // DeLife 태그와 충돌시 라이프 감소
            Debug.Log("장애물 충돌");
            if (GameManager.instance.DehpScore()==false) Die();
        }

        if (other.tag == "Coin" && !isDead) {
            // 코인 태그와 충돌 & 죽지 않았다면 비활성화, Score 실행
            other.gameObject.SetActive(false);
            GameManager.instance.AddScore(10);
        }
    }

    // Update() 메서드에서 사용한 isGrounded의 값과 jumpCount를 리셋하는 과정
    // 유한상태머신에서의 전이과정을 코드로 작성
    private void OnCollisionEnter2D(Collision2D collision) {
       // 바닥에 닿았음을 감지하는 처리
       // 어떤 콜라이더와 닿았으며, 충돌 표면이 위쪽을 보고 있으면
       if (collision.contacts[0].normal.y > 0.7f) {
            // collision.contacts[0]은 normal벡터의 y값으로 방향을 받음
            // -1은 아래 +1은 위 +1에 가까울수록 경사가 완만해짐
            // 절벽과 천장을 인식하지 않게끔(0.7f = 약 45의 경사를 가진채 위를 향함)
            // isGrounded를 true로 변경하고, 누적 점프 횟수를 0으로 리셋
            isGrounded = true;
            jumpCount = 0;
        }
   }

   private void OnCollisionExit2D(Collision2D collision) {
        // 바닥에서 벗어났음을 감지하는 처리
        // 어떤 콜라이더에서 떼어진 경우 isGrounded를 false로 변경
        isGrounded = false;
   }
}