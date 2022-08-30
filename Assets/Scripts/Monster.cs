using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] float speed = 5f;
	[SerializeField] int maxHP = 100;   // ���� ���� �� �ִ� �ִ� HP
	[SerializeField] float attackRange = 5f; // ���� ���� ����
	[SerializeField] float attackPower = 10f;
	[SerializeField] float attackInterval = 1f;
	[Header("[����Ʈ ����]")]
	[SerializeField] ResourceDataObj ResDataObj = null;
	[SerializeField] Transform transEff = null; // ����Ʈ�� ��µ� ��ġ

	int curHP = 0;  // ���� HP
	bool IsDeath { get { return (curHP <= 0); } } // ���� �׾���?

	// ���� ���� ������ �ε����� ����
	public int MyPosIdx { get; set; } = -1;	// ���� ������ �ε���

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
		curHP = maxHP; // HP �� ����
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
	// MonsterData �� �̿��ؼ� ������ �ɷ�ġ�� ����
	public void SetData(MonsterData mobData)
	{
		gameObject.name = mobData.Name;
		attackPower = mobData.AttackPower;
		maxHP = mobData.MaxHP;
	}

	// �Էµ� �ݶ��̴��� ������ �ݶ��̴��� �浹�ߴ��� üũ�� �ִ� �Լ�
	// ��ȯ���� true�̸� �浹�� �Ŵ�.
	public bool CheckWithCollider(SphereCollider otherCol)
	{
		return myCC.bounds.Intersects(otherCol.bounds);
	}


	// ������ �߻��ϸ� ȣ��Ǵ� �Լ�
	void onAttackEvent()
	{
		//Debug.Log("## ������ �����̺�Ʈ ó���Լ�");

		targetPlayer.SendMessage("TransferDamge", attackPower, SendMessageOptions.DontRequireReceiver);
	}


	// ���� ���� �����߾�~!!! ��ŭ?? damageValue ��ŭ
	void TransferDamge(DamageInfo dmgInfo)
	{
		//Debug.Log("## ���� �޾Ҵ� : " + damageValue);
		if (IsDeath) return;

		// ������ �������� ���� HP �� ����Ǿ���.
		curHP -= (int)dmgInfo.AttackPower;
		
		// ������ �ؽ�Ʈ ���
		DamageTextMgr.Inst.AddText(dmgInfo.AttackPower, transform.position, Vector3.up * 1.5f);

		if (curHP <= 0)
		{
			// ������ �ش�.
			dmgInfo.Attacker.SendMessage("Reward", 10, SendMessageOptions.DontRequireReceiver);


			curHP = 0;			
			nextState(STATE.DEATH);			
		}
		// �ǰ�
		else
		{
			nextState(STATE.HIT);
		}
		
	}




	//---------------------------------------------------------------------------------------------
	//
	// ���¸� ��Ÿ���� ��������
	//

	// �ڷ�ƾ�� ������ ������ŭ ���� ����
	// ������ ���� �� ���� (���, �̵�, ����, �ǰ�, ����)
	public enum STATE
	{
		NONE,	// �ƹ��͵� �ƴ� ����
		IDLE,
		MOVE,
		ATTACK,
		HIT,
		DEATH,

		MAX
	}


	// �������̿� ���� �ڷ�ƾ�� ��ȯ�ϴ� �Լ�
	Coroutine prevCoroutine = null;
	STATE curState = STATE.NONE;
	void nextState(STATE newState)
	{
		if (newState == curState)
			return;
		// ������ ���� ����ǰ� �ִ� �ڷ�ƾ�� �־��ٸ� ����~!!!
		if (prevCoroutine != null)
			StopCoroutine(prevCoroutine);

		// ���ο� ���·� ������ �ְ�
		curState = newState;
		prevCoroutine = StartCoroutine(newState.ToString() + "_State");
	}

	// ��� ���¸� ó���ϴ� �ڷ�ƾ
	IEnumerator IDLE_State()
	{
		//Debug.Log("## Start IDLE_State �ڷ�ƾ !!!");
		myAnimator.SetBool("move", false);

		// ���� �ʴ��� �� ���´� ��� ����
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
	// �̵� ���¸� ó���ϴ� �ڷ�ƾ
	IEnumerator MOVE_State()
	{
		//Debug.Log("## Start MOVE_State �ڷ�ƾ !!!");

		// �̵� �ִϸ��̼� ���
		myAnimator.SetBool("move", true);

		// ���� �ʴ��� �� ���´� ��� ����
		while (IsDeath == false)
		{
			// Ÿ�ٰ��� �Ÿ��� ���� ���ݹ������� ū��??? 
			// ũ�� Ÿ�� �������� �̵��ϰ�, �۰ų� ������ �����(�̵��� ���Ѵ�)
			if (Vector3.Distance(targetPlayer.transform.position, transform.position) <= attackRange)
			{
				// �����.
				// ���(Idle) �ִϸ��̼� ���
				myAnimator.SetBool("move", false);

				// ���� ����
				nextState(STATE.ATTACK);
				yield break;
			}

			//
			// �÷��̾ �����ϱ�~~

			// ���� �̵������� �÷��̾� �������� ����ؾ� �Ѵ�.
			Vector3 moveDir = (targetPlayer.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(moveDir);
			// ȸ���� �ϸ� ���� forward(�չ���)�� �ٲ�Ƿ� �׻� forward �������� �̵���Ų��.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

			// ���͸�ϰ����ڿ��� ���� �̵��� �˸���.
			MonsterListMgr.Inst.UpdateMonster(this);
			yield return null;
		}
	}

	//
	// ���� ���¸� ó���ϴ� �ڷ�ƾ
	IEnumerator ATTACK_State()
	{
		//Debug.Log("## Start ATTACK_State �ڷ�ƾ !!!");

		// ���� �ʴ��� �� ���´� ��� ����
		while (IsDeath == false)
		{
			//0. Ÿ�ٰ� ���� �Ÿ��� üũ�ؼ� ���ݰ��� �������� Ȯ��!!!
			if (Vector3.Distance(targetPlayer.transform.position, transform.position) > attackRange)
			{
				nextState(STATE.MOVE);
				yield break;
			}

			// ���ݹ������� ���� ȸ���ϱ�
			Vector3 moveDir = (targetPlayer.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(moveDir);

			//1. �ϴ� ����
			//Debug.Log("## �� �� �����Ҳ���~!!");
			myAnimator.SetTrigger("attack");

			//2. ���� ���ݱ��� ���
			yield return new WaitForSeconds(attackInterval);
		}
	}

	// s
	// �ǰ� ���¸� ó���ϴ� �ڷ�ƾ
	IEnumerator HIT_State()
	{
		//Debug.Log("## Start HIT_State �ڷ�ƾ !!!");

		// �ǰ� ����Ʈ ���
		GameObject instObj = Instantiate(ResDataObj.EffHit, transEff.position, Quaternion.identity);
		Destroy(instObj, 2f); // 2�� �ڿ� �����ȴ�.

		// �ǰ� �ִϸ��̼� ���
		myAnimator.SetTrigger("hit");


		nextState(STATE.IDLE);
		yield return null;
	}

	//
	// ���� ���¸� ó���ϴ� �ڷ�ƾ
	IEnumerator DEATH_State()
	{
		//Debug.Log("## Start DEATH_State �ڷ�ƾ !!!");

		// �׾��ٸ� �״¾ִϸ��̼� ȣ��
		myAnimator.SetTrigger("death");
		yield return null;

		// #1
		//gameObject.Recycle();
		// #2
		MonsterPool.Inst.DestroyMonster(this);
	}
	//---------------------------------------------------------------------------------------------
}
