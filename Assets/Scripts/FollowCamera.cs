using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Player MyPlayer;

    Vector3 offsetDir = Vector3.zero;
    float distance = 0f;    // 플레이어와 카메라 간의 거리
    // Start is called before the first frame update
    void Awake()
    {
        MyPlayer = FindObjectOfType<Player>();
        // 현재 플레이어와 시작할 때, 얼만큼 떨어져 있는지 계산해서
        offsetDir = (transform.position - MyPlayer.transform.position).normalized;
        // 플레이어와 카메라가 얼만큼 떨어져 있는지 계산해서 저장
        distance = Vector3.Distance(transform.position, MyPlayer.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        // #1
        // 플레이어 기준으로 왼쪽/오른쪽으로 일정거리만큼 떨어져 있는 카메라
        /*Vector3 pos = MyPlayer.transform.position + MyPlayer.transform.right * 2f - MyPlayer.transform.forward * 2;
        pos.y = transform.position.y;
        transform.position = pos;*/
        
        // #2
        // 시작할 때 플레이어와의 떨어져 있는 정도를 기록해서 
        transform.position = MyPlayer.transform.position + offsetDir * distance;
        
    }
}
