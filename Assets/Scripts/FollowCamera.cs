using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Player MyPlayer;

    Vector3 offsetDir = Vector3.zero;
    float distance = 0f;    // �÷��̾�� ī�޶� ���� �Ÿ�
    // Start is called before the first frame update
    void Awake()
    {
        MyPlayer = FindObjectOfType<Player>();
        // ���� �÷��̾�� ������ ��, ��ŭ ������ �ִ��� ����ؼ�
        offsetDir = (transform.position - MyPlayer.transform.position).normalized;
        // �÷��̾�� ī�޶� ��ŭ ������ �ִ��� ����ؼ� ����
        distance = Vector3.Distance(transform.position, MyPlayer.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        // #1
        // �÷��̾� �������� ����/���������� �����Ÿ���ŭ ������ �ִ� ī�޶�
        /*Vector3 pos = MyPlayer.transform.position + MyPlayer.transform.right * 2f - MyPlayer.transform.forward * 2;
        pos.y = transform.position.y;
        transform.position = pos;*/
        
        // #2
        // ������ �� �÷��̾���� ������ �ִ� ������ ����ؼ� 
        transform.position = MyPlayer.transform.position + offsetDir * distance;
        
    }
}
