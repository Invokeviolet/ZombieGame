using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
//
// MonsterPool �� ���͸� �����ϰ�, �����ϴµ� �־ 
// �� �� ȿ������ ����� �����ϰ��� �ϴ� ���ӿ�����Ʈ
// ��, Object(Monster) Pooling �ϴ� �ڴٴ� ��

public class MonsterPool : MonoBehaviour
{
	// �̱������� ������
	#region �̱���
	static MonsterPool instance = null;
    public static MonsterPool Inst
	{
		get
		{
			if(instance == null)
			{
				// ���̷�Ű�� �����ϴ� ���ӿ�����Ʈ��� ���� ã�ƺ���.
				instance = FindObjectOfType<MonsterPool>();
				if (instance == null)
					instance = new GameObject("MonsterPool").AddComponent<MonsterPool>();
			}
			return instance;
		}
	}
	#endregion // Singleton
	//-----------------------------------------------------------------------------

	Monster prefabMob = null;
	Queue<Monster> pool = new Queue<Monster>();


	private void Awake()
	{
		// (���Ϸ� �����ϴ�)�������� �ε��ϴ� �Լ��̴�.
		prefabMob = Resources.Load<Monster>("Monster");
	}

	//
	// ���� ��û 
	public Monster CreateMonster( Vector3 pos)
	{
		Monster instMob = null;
		//ó������ �ƹ��͵� ������ ��������
		if (pool.Count == 0)
		{
			// �ε��� �������� �̿��ؼ� �ν���Ʈ ��ü �Ѱ��� �����.
 			instMob = Instantiate(prefabMob, pos, Quaternion.identity, this.transform);
			// ���� ��ϰ����ڿ��� 1�� �߰�
			MonsterListMgr.Inst.AddMonster(instMob);
 			return instMob;
		}

		// ������ ��� �߿��� ��Ȱ��ȭ �� ���� ã�´�.
		// ���� ť������ �� �տ� �ִ� �� �ϳ��� ������ �ָ�
		instMob = pool.Dequeue();
		//instMob.transform.parent = null;
		instMob.transform.position = pos;
		instMob.transform.rotation = Quaternion.identity;
		instMob.gameObject.SetActive(true);
		// ���� ��ϰ����ڿ��� 1�� �߰�
		MonsterListMgr.Inst.AddMonster(instMob);
		return instMob;
	}

	//
	// ��Ȱ��ȭ 
	public void DestroyMonster(Monster mob)
	{
		//mob.transform.parent = this.transform;
		mob.gameObject.SetActive(false);
		pool.Enqueue(mob); // pool �� 1�� �þ��.

		// ���� ��ϰ����ڿ��� 1�� ����
		MonsterListMgr.Inst.DelMonster(mob);
	}

	


}
