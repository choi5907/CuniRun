using UnityEngine;

// 게임 오브젝트를 계속 왼쪽으로 움직이는 스크립트
public class ScrollingObject : MonoBehaviour {
    public float speed = 10f; // 이동 속도

    private void Update() {
        // 게임 오브젝트를 왼쪽으로 일정 속도로 평행 이동하는 처리
        
        // 게임 오버가 아닌 경우에만
        if (!GameManager.instance.isGameover) {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            // Translate : 평행이동 메서드 Vector3타입으로 받는다.
            // Vector3.left = ( -1, 0, 0 ) -1 * speed = ( -speed, 0, 0 )
        }
    }
}