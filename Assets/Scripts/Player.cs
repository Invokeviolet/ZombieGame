using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HP 
public delegate void OnChangedHP(int curHP, int maxHP);

public class Player : MonoBehaviour
{

	[Header("[플레이어 기본속성들]")]
    [SerializeField] float speed = 5f; // 이동속도
    [SerializeField] int maxHP = 100;   // 내가 가질 수 있는 최대 HP
    [SerializeField] float attackPower = 10f; // 공격력
    [SerializeField] float attackRange = 6f;    // 공격 범위
    [Header("[이펙트 내용]")]
    [SerializeField] ResourceDataObj ResDataObj = null; // 리소스 데이터
	[SerializeField] Transform transEff = null; // 이펙트가 출력될 위치

    int curHP = 0;  // 현재 HP
    bool IsDeath { get { return (curHP <= 0); } } // 내가 죽었나?

    public OnChangedHP CallbackChangedHP = null;    // HP가 변경되면 호출

    public int Level { get; set; } = 1; // 레벨
    public int Exp { get; set; } = 0;   // 경험치








    Transform transCam = null;

    CharacterController myCC = null;
    Animator myAnimator = null;
    AnimationEventReceiver myEventReceiver = null;

    private void Awake()
	{
		transCam  = FindObjectOfType<Camera>().transform;
        myCC = GetComponent<CharacterController>();

        myAnimator = GetComponentInChildren<Animator>();
        myEventReceiver = GetComponentInChildren<AnimationEventReceiver>(); // 하위에 붙어 있는 애니메이션 이벤트 리시버를 얻어온다.
        myEventReceiver.callbackAttackEvent = OnAttackEvent;
        myEventReceiver.callbackAnimEndEvent = OnAnimEndEvent;
    }

	private void Start()
	{
        curHP = maxHP; // 플레이어가 생성되면 HP 를 갱신
	}

    bool isAttackProcess = false;
	Vector3 moveDir = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        // 내가 죽으면 이동을 하지 못하도록 한다.
        if (IsDeath) return;




		// 공격 버튼을 누르면 공격애니메이션 출력
		if (Input.GetKeyDown(KeyCode.Return))
		{
            isAttackProcess = true;
            //myAnimator.SetBool("move", false);
            myAnimator.SetTrigger("attack");
            return;
		}
        if (isAttackProcess) return;




        //
        // 이동 계산 루틴
        //
		float xAxis = Input.GetAxis("Horizontal");
		float zAxis = Input.GetAxis("Vertical");

        // 입력이 있다면 true, 없으면 false 를 만들어내는 문장
        bool isMove = (xAxis != 0f || zAxis != 0f);
        // 입력의 상태를 애니메이터의 파라미터로 넘겨준다.
        myAnimator.SetBool("move", isMove);

        if (isMove)
		{

			// 오른쪽 방향과, 위쪽 방향을 기준으로 입력된 값을 기준으로 회전 방향을 결정한다.
			moveDir = Vector3.right * xAxis + Vector3.forward * zAxis;
            transform.rotation = Quaternion.LookRotation(moveDir);

            // 캐릭터컨트롤러를 이용해서 이동하기
            myCC.Move(moveDir * speed * Time.deltaTime);
            //transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);



            // # 카메라 방향이 +Z 축을 바라보는 경우
            //
            /*///
            // 오른쪽 방향과, 위쪽 방향을 기준으로 입력된 값을 기준으로 회전 방향을 결정한다.
            Vector3 moveDir = Vector3.right * xAxis + Vector3.forward * zAxis;
            transform.rotation = Quaternion.LookRotation(moveDir);
            // 회전을 하면 나의 forward(앞방향)이 바뀌므로 항상 forward 기준으로 이동시킨다.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            //*/

            /*//
            // # 카메라 방향이 +Z 축과 다르게 있는 경우
             Vector3 right = Vector3.Cross(Vector3.up, transCam.forward);
             Vector3 forward = Vector3.Cross(right, Vector3.up);
             transform.rotation = Quaternion.LookRotation(right * xAxis + forward * zAxis);

            // 카메라의 forward와 right 를 그대로 쓴다면??? 
            // forward 벡터가 회전이 적용되어 있는 상태라면 캐릭터도 회전된 방향으로 이동한다.
			//transform.rotation = Quaternion.LookRotation(transCam.right * xAxis + transCam.forward * zAxis);

			// 회전을 하면 나의 forward(앞방향)이 바뀌므로 항상 forward 기준으로 이동시킨다.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            //*/


        }


        
	}


    // 몬스터를 죽이면 보상을 받는다.
    void Reward(int rewardValue)
	{
        Exp += rewardValue; // 경험치 획득
        // 획득된 경험치를 바탕으로 레벨이 올라갔다면 
        // 다른 능력치도 반영해 준다.
        PlayerData playerData = GameDataMgr.Inst.FindPlayerDataByExp(Exp);
        if(playerData.Level != Level)
		{
            attackPower = playerData.AttackPower;
            maxHP = playerData.MaxHP;
            curHP = maxHP;
            Level = playerData.Level;
        }
	}


    // 누가 나를 공격했어~!!! 얼만큼?? damageValue 만큼
    void TransferDamge(float damageValue)
	{
        //Debug.Log("## 공격 받았다 : " + damageValue);
        if (IsDeath) return;

        // 데미지 영향으로 나의 HP 가 변경되었다.
        curHP -= (int)damageValue;
        if (curHP <= 0)
        {
            curHP = 0;
            // 죽었다면 죽는애니메이션 호출
            myAnimator.SetTrigger("death");
        }

        //Debug.Log("## 현재 HP : " + curHP);
        
        // 변경된 HP 정보가 필요한 곳을 위해 delegate 함수 호출
        if (CallbackChangedHP != null)
            CallbackChangedHP(curHP, maxHP);


        // 피격 이펙트 출력
        GameObject instObj = Instantiate(ResDataObj.EffHit, transEff.position, Quaternion.identity);
        Destroy(instObj, 2f); // 2초 뒤에 삭제된다.

        // 데미지 텍스트 출력
        DamageTextMgr.Inst.AddText(damageValue, transform.position, Vector3.up * 1.5f);
    }

    //
    // 공격애니메이션 중에 공격이벤트가 발생하는 시점에 호출
    void OnAttackEvent()
	{
        //Debug.Log("## Player : onAttackEvent");

        // 공격 대상을 찾는게 필요!!!

        // 나의 공격 정보
        DamageInfo dmgInfo = new DamageInfo();
        dmgInfo.Attacker = this.gameObject;
        dmgInfo.AttackPower = attackPower;

        // 내 타겟이 될 가능성이 높은 Monster 들의 목록을 구하기
        List<Monster> mobs = MonsterListMgr.Inst.FindMonsterListBy(transform.position, attackRange);
        for (int i = 0; i < mobs.Count; i++)
        {
            // 내 시야 범위 내에 포함되는 놈인지 한번 더 체크해야 한다.
            Vector3 mobFromMe = (mobs[i].transform.position - transform.position).normalized;
            // 내 시야에 들어오는지?
            if(Vector3.Dot(mobFromMe, transform.forward) > 0)
			{
				//  공격처리
				mobs[i].SendMessage("TransferDamge", dmgInfo, SendMessageOptions.DontRequireReceiver);
			}
        }

	}

    //
    // 공격애니메이션 종료될 때 호출되는 함수
    void OnAnimEndEvent()
	{
		//Debug.Log("## Player : OnAnimEndEvent");
        isAttackProcess = false;
    }





	private void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transEff.position, 0.1f);
	}
}
