using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HP 
public delegate void OnChangedHP(int curHP, int maxHP);

public class Player : MonoBehaviour
{

	[Header("[�÷��̾� �⺻�Ӽ���]")]
    [SerializeField] float speed = 5f; // �̵��ӵ�
    [SerializeField] int maxHP = 100;   // ���� ���� �� �ִ� �ִ� HP
    [SerializeField] float attackPower = 10f; // ���ݷ�
    [SerializeField] float attackRange = 6f;    // ���� ����
    [Header("[����Ʈ ����]")]
    [SerializeField] ResourceDataObj ResDataObj = null; // ���ҽ� ������
	[SerializeField] Transform transEff = null; // ����Ʈ�� ��µ� ��ġ

    int curHP = 0;  // ���� HP
    bool IsDeath { get { return (curHP <= 0); } } // ���� �׾���?

    public OnChangedHP CallbackChangedHP = null;    // HP�� ����Ǹ� ȣ��

    public int Level { get; set; } = 1; // ����
    public int Exp { get; set; } = 0;   // ����ġ








    Transform transCam = null;

    CharacterController myCC = null;
    Animator myAnimator = null;
    AnimationEventReceiver myEventReceiver = null;

    private void Awake()
	{
		transCam  = FindObjectOfType<Camera>().transform;
        myCC = GetComponent<CharacterController>();

        myAnimator = GetComponentInChildren<Animator>();
        myEventReceiver = GetComponentInChildren<AnimationEventReceiver>(); // ������ �پ� �ִ� �ִϸ��̼� �̺�Ʈ ���ù��� ���´�.
        myEventReceiver.callbackAttackEvent = OnAttackEvent;
        myEventReceiver.callbackAnimEndEvent = OnAnimEndEvent;
    }

	private void Start()
	{
        curHP = maxHP; // �÷��̾ �����Ǹ� HP �� ����
	}

    bool isAttackProcess = false;
	Vector3 moveDir = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        // ���� ������ �̵��� ���� ���ϵ��� �Ѵ�.
        if (IsDeath) return;




		// ���� ��ư�� ������ ���ݾִϸ��̼� ���
		if (Input.GetKeyDown(KeyCode.Return))
		{
            isAttackProcess = true;
            //myAnimator.SetBool("move", false);
            myAnimator.SetTrigger("attack");
            return;
		}
        if (isAttackProcess) return;




        //
        // �̵� ��� ��ƾ
        //
		float xAxis = Input.GetAxis("Horizontal");
		float zAxis = Input.GetAxis("Vertical");

        // �Է��� �ִٸ� true, ������ false �� ������ ����
        bool isMove = (xAxis != 0f || zAxis != 0f);
        // �Է��� ���¸� �ִϸ������� �Ķ���ͷ� �Ѱ��ش�.
        myAnimator.SetBool("move", isMove);

        if (isMove)
		{

			// ������ �����, ���� ������ �������� �Էµ� ���� �������� ȸ�� ������ �����Ѵ�.
			moveDir = Vector3.right * xAxis + Vector3.forward * zAxis;
            transform.rotation = Quaternion.LookRotation(moveDir);

            // ĳ������Ʈ�ѷ��� �̿��ؼ� �̵��ϱ�
            myCC.Move(moveDir * speed * Time.deltaTime);
            //transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);



            // # ī�޶� ������ +Z ���� �ٶ󺸴� ���
            //
            /*///
            // ������ �����, ���� ������ �������� �Էµ� ���� �������� ȸ�� ������ �����Ѵ�.
            Vector3 moveDir = Vector3.right * xAxis + Vector3.forward * zAxis;
            transform.rotation = Quaternion.LookRotation(moveDir);
            // ȸ���� �ϸ� ���� forward(�չ���)�� �ٲ�Ƿ� �׻� forward �������� �̵���Ų��.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            //*/

            /*//
            // # ī�޶� ������ +Z ��� �ٸ��� �ִ� ���
             Vector3 right = Vector3.Cross(Vector3.up, transCam.forward);
             Vector3 forward = Vector3.Cross(right, Vector3.up);
             transform.rotation = Quaternion.LookRotation(right * xAxis + forward * zAxis);

            // ī�޶��� forward�� right �� �״�� ���ٸ�??? 
            // forward ���Ͱ� ȸ���� ����Ǿ� �ִ� ���¶�� ĳ���͵� ȸ���� �������� �̵��Ѵ�.
			//transform.rotation = Quaternion.LookRotation(transCam.right * xAxis + transCam.forward * zAxis);

			// ȸ���� �ϸ� ���� forward(�չ���)�� �ٲ�Ƿ� �׻� forward �������� �̵���Ų��.
			transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            //*/


        }


        
	}


    // ���͸� ���̸� ������ �޴´�.
    void Reward(int rewardValue)
	{
        Exp += rewardValue; // ����ġ ȹ��
        // ȹ��� ����ġ�� �������� ������ �ö󰬴ٸ� 
        // �ٸ� �ɷ�ġ�� �ݿ��� �ش�.
        PlayerData playerData = GameDataMgr.Inst.FindPlayerDataByExp(Exp);
        if(playerData.Level != Level)
		{
            attackPower = playerData.AttackPower;
            maxHP = playerData.MaxHP;
            curHP = maxHP;
            Level = playerData.Level;
        }
	}


    // ���� ���� �����߾�~!!! ��ŭ?? damageValue ��ŭ
    void TransferDamge(float damageValue)
	{
        //Debug.Log("## ���� �޾Ҵ� : " + damageValue);
        if (IsDeath) return;

        // ������ �������� ���� HP �� ����Ǿ���.
        curHP -= (int)damageValue;
        if (curHP <= 0)
        {
            curHP = 0;
            // �׾��ٸ� �״¾ִϸ��̼� ȣ��
            myAnimator.SetTrigger("death");
        }

        //Debug.Log("## ���� HP : " + curHP);
        
        // ����� HP ������ �ʿ��� ���� ���� delegate �Լ� ȣ��
        if (CallbackChangedHP != null)
            CallbackChangedHP(curHP, maxHP);


        // �ǰ� ����Ʈ ���
        GameObject instObj = Instantiate(ResDataObj.EffHit, transEff.position, Quaternion.identity);
        Destroy(instObj, 2f); // 2�� �ڿ� �����ȴ�.

        // ������ �ؽ�Ʈ ���
        DamageTextMgr.Inst.AddText(damageValue, transform.position, Vector3.up * 1.5f);
    }

    //
    // ���ݾִϸ��̼� �߿� �����̺�Ʈ�� �߻��ϴ� ������ ȣ��
    void OnAttackEvent()
	{
        //Debug.Log("## Player : onAttackEvent");

        // ���� ����� ã�°� �ʿ�!!!

        // ���� ���� ����
        DamageInfo dmgInfo = new DamageInfo();
        dmgInfo.Attacker = this.gameObject;
        dmgInfo.AttackPower = attackPower;

        // �� Ÿ���� �� ���ɼ��� ���� Monster ���� ����� ���ϱ�
        List<Monster> mobs = MonsterListMgr.Inst.FindMonsterListBy(transform.position, attackRange);
        for (int i = 0; i < mobs.Count; i++)
        {
            // �� �þ� ���� ���� ���ԵǴ� ������ �ѹ� �� üũ�ؾ� �Ѵ�.
            Vector3 mobFromMe = (mobs[i].transform.position - transform.position).normalized;
            // �� �þ߿� ��������?
            if(Vector3.Dot(mobFromMe, transform.forward) > 0)
			{
				//  ����ó��
				mobs[i].SendMessage("TransferDamge", dmgInfo, SendMessageOptions.DontRequireReceiver);
			}
        }

	}

    //
    // ���ݾִϸ��̼� ����� �� ȣ��Ǵ� �Լ�
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
