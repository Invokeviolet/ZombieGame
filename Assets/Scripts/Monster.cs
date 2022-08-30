using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] float speed = 5f;
	[SerializeField] int maxHP = 100;   // 내가 가질 수 있는 최대 HP
	[SerializeField] float attackRange = 5f; // 공격 가능 범위
	[SerializeField] float attackPower = 10f;
	[SerializeField] float attackInterval = 1f;
	[Header("[이펙트 내용]")]
	[SerializeField] ResourceDataObj ResDataObj = null;
	[SerializeField] Transform transEff = null; // 이펙트가 출력될 위치

	int curHP = 0;  // 현재 HP
	bool IsDeath { get { return (curHP <= 0); } } // 내가 죽었나?

	// 내가 속한 영역의 인덱스를 보관
	public int MyPosIdx { get; set; } = -1;	// 나의 영역의 인덱스

	Player targetPlayer = null;
	Animator myAnimator = null;
	AnimationEventReceiver animReceiver = null;
	CapsuleCollider myCC = null;

	private void Awake()
	{
		myAnimator = GetComponentInChildren<Animator>();
		animReceiver = GetComponentInChildren<AnimationEventReceiver>();
		animReceiver.callbackAttackEvent = onAttackEvent;

		myCC = GetComponent<CapsuleCollider>();
	}

	private void OnEnable()
	{
		curHP = maxHP; // HP 를 갱신
		nextState(STATE.IDLE);
	}

	// Start is called before the first frame update
	void Start()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
		if (IsDeath) return;
	}


	// 
	// MonsterData 를 이용해서 몬스터의 능력치를 변경
	public void SetData(MonsterData mobData)
	{
		gameObject.name = mobData.Name;
		attackPower = mobData.AttackPower;
		maxHP = mobData.MaxHP;
	}

	// 입력된 콜라이더와 몬스터의 콜라이더가 충돌했는지 체크해 주는 함수
	// 반환값이 true이면 충돌한 거다.
	public bool CheckWithCollider(SphereCollider otherCol)
	{
		return myCC.bounds.Intersects(otherCol.bounds);
	}


	// 공격이 발생하면 호출되는 함수
	void onAttackEvent()
	{
		//Debug.Log("## 몬스터의 공격이벤트 처리함수");

		targetPlayer.SendMessage("TransferDamge", attackPower, SendMessageOptions.DontRequireReceiver);
	}


	// 누가 나를 공격했어~!!! 얼만큼?? damageValue 만큼
	void TransferDamge(DamageInfo dmgInfo)
	{
		//Debug.Log("## 공격 받았다 : " + damageValue);
		if (IsDeath) return;

		// 데미지 영향으로 나의 HP 가 변경되었다.
		curHP -= (int)dmgInfo.AttackPower;
		
		// 데미지 텍스트 출력
		DamageTextMgr.Inst.AddText(dmgInfo.AttackPower, transform.position, Vector3.up * 1.5f);

		if (curHP <= 0)
		{
			// 보상을 준다.
			dmgInfo.Attacker.SendMessage("Reward", 10, SendMessageOptions.DontRequireReceiver);


			curHP = 0;			
			nextState(STATE.DEATH);			
		}
		// 피격
		else
		{
			nextState(STATE.HIT);
		}
		
	}




	//---------------------------------------------------------------------------------------------
	//
	// 상태를 나타내는 코투린들
	//

	// 코루틴을 상태의 종류만큼 생성 예정
	// 몬스터의 상태 의 종류 (대기, 이동, 공격, 피격, 죽음)
	public enum STATE
	{
		NONE,	// 아무것도 아닌 상태
		IDLE,
		MOVE,
		ATTACK,
		HIT,
		DEATH,

		MAX
	}


	// 상태전이에 따른 코루틴을 전환하는 함수
	Coroutine prevCoroutine = null;
	STATE curState = STATE.NONE;
	void nextState(STATE newState)
	{
		if (newState == curState)
			return;
		// 기존에 뭔가 실행되고 있는 코루틴이 있었다면 종료~!!!
		if (prevCoroutine != null)
			StopCoroutine(prevCoroutine);

		// 새로운 상태로 변경해 주고
		curState = newState;
		prevCoroutine = StartCoroutine(newState.ToString() + "_State");
	}

	// 대기 상태를 처리하는 코루틴
	IEnumerator IDLE_State()
	{
		//Debug.Log("## Start IDLE_State 코루틴 !!!");
		myAnimator.SetBool("move", false);

		// 죽지 않는한 이 상태는 계속 유지
		while (IsDeath == false)
		{
			targetPlayer = FindObjectOfType<Player>();
			if (targetPlayer != null)
			{
				nextState(STATE.MOVE);
				yield break;
			}
			
			yield return null; 
		}		
	}

	//
	// 이동 상태를 처리하는 코루틴
	IEnumerator MOVE_State()
	{
		//Debug.Log("## Start MOVE_State 코루틴 !!!");

		// 이동 애니메이션 출력
		myAnimator.SetBool("move", true);

		// 죽지 않는한 이 상태는 계속 유지
		while (IsDeath == false)
		{
			// 타겟과의 거리가 나의 공격범위보다 큰가??? 
			// 크면 타겟 방향으로 이동하고, 작거나 같으면 멈춘다(이동을 안한다)
			if (Vector3.Distance(targetPlayer.transform.position, transform.position) <= attackRange)
			{
				// 멈춘다.
				// 대기(Idle) 애니메이션 출력
				myAnimator.SetBool("move", false);

				// 공격 시작
				nextState(STATE.ATTACK);
				yield break;
			}

			//
			// 플레이어를 추적하기~~

			// 나의 이동방향은 플레이어 기준으로 계산해야 한다.
			Vector3 moveDir = (targetPlayer.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(moveDir);
			// 회전을 하면 나의 forward(앞방향)이 바뀌므로 항상 forward 기준으로 이동시킨다.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

			// 몬스터목록관리자에게 나의 이동을 알린다.
			MonsterListMgr.Inst.UpdateMonster(this);
			yield return null;
		}
	}

	//
	// 공격 상태를 처리하는 코루틴
	IEnumerator ATTACK_State()
	{
		//Debug.Log("## Start ATTACK_State 코루틴 !!!");

		// 죽지 않는한 이 상태는 계속 유지
		while (IsDeath == false)
		{
			//0. 타겟과 나의 거리르 체크해서 공격가능 범위인지 확인!!!
			if (Vector3.Distance(targetPlayer.transform.position, transform.position) > attackRange)
			{
				nextState(STATE.MOVE);
				yield break;
			}

			// 공격방향으로 몸을 회전하기
			Vector3 moveDir = (targetPlayer.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(moveDir);

			//1. 일단 공격
			//Debug.Log("## 난 널 공격할끄야~!!");
			myAnimator.SetTrigger("attack");

			//2. 다음 공격까지 대기
			yield return new WaitForSeconds(attackInterval);
		}
	}

	// s
	// 피격 상태를 처리하는 코루틴
	IEnumerator HIT_State()
	{
		//Debug.Log("## Start HIT_State 코루틴 !!!");

		// 피격 이펙트 출력
		GameObject instObj = Instantiate(ResDataObj.EffHit, transEff.position, Quaternion.identity);
		Destroy(instObj, 2f); // 2초 뒤에 삭제된다.

		// 피격 애니메이션 출력
		myAnimator.SetTrigger("hit");


		nextState(STATE.IDLE);
		yield return null;
	}

	//
	// 죽음 상태를 처리하는 코루틴
	IEnumerator DEATH_State()
	{
		//Debug.Log("## Start DEATH_State 코루틴 !!!");

		// 죽었다면 죽는애니메이션 호출
		myAnimator.SetTrigger("death");
		yield return null;

		// #1
		//gameObject.Recycle();
		// #2
		MonsterPool.Inst.DestroyMonster(this);
	}
	//---------------------------------------------------------------------------------------------
}
