using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
// ������ ���͵��� ����� ������ �ش�.
//
// ���Ͱ� �����ǰų�, �װų� �ϸ� ���� �����Ǵ� ��Ͽ��� �ݿ��ȴ�.
//
// �÷��̾ ����� ��û�ϸ� ������ ����� ������ �ش�.

public class MonsterListMgr
{
	//----------------------------------------------------------------
	//
	#region �̱���
	private MonsterListMgr() { }
	static MonsterListMgr instance = null;
	public static MonsterListMgr Inst
	{
		get
		{
			if(instance == null)
				instance = new MonsterListMgr();
			return instance;
		}
	}
	#endregion // �̱���
	//----------------------------------------------------------------

	List<Monster>[] listMobs = new List<Monster>[] { new List<Monster>(), new List<Monster>(), new List<Monster>(), new List<Monster>() };


	// ���� ���� ��
	public int GetAllCount()
	{
		int count = 0;
		for(int i =0; i < listMobs.Length; ++i)
			count += listMobs[i].Count;

		return count;
	}

	//
	// �� ��ġ�� �������� ���� �ִ� ������ �ε����� ���Ѵ�.
	int findMyPosIdx(Vector3 pos)
	{
		if (pos.x < 0f && pos.z > 0f)
			return 0; // ���� ��
		if (pos.x >= 0f && pos.z > 0f)
			return 1;   // ������ ��
		if (pos.x < 0f && pos.z <= 0f)
			return 2; // ���� �Ʒ�
		//if (pos.x >= 0f && pos.z <= 0f)
			return 3;   // ������ �Ʒ�

	}


	// ���� �߰�
	public void AddMonster(Monster mob)
	{
		int posIdx = findMyPosIdx(mob.transform.position);
		mob.MyPosIdx = posIdx;
		listMobs[posIdx].Add(mob);
	}

	public void UpdateMonster(Monster mob)
	{
		// ���� ��ġ�� �������� ���� ���� ������ ����� ã�´�.
		int posIdx = findMyPosIdx(mob.transform.position);

		// ���� ������ ������ġ�� �޶������� Ȯ���� ����.
		if (mob.MyPosIdx == posIdx || mob.MyPosIdx == -1)
			return;
		// �޶����ٸ� ������ ���� ���� �������� ���� �����ϰ�, ���ο� ������ ���� �־��ش�.
		if(mob.MyPosIdx >= 0)
			listMobs[mob.MyPosIdx].Remove(mob);

		mob.MyPosIdx = posIdx;
		listMobs[posIdx].Add(mob);

		//Debug.Log("## ���� ��ġ Index : " + posIdx);
	}


    // ���� ����
    public void DelMonster(Monster mob)
	{
		//int posIdx = findMyPosIdx(mob.transform.position);
		listMobs[mob.MyPosIdx].Remove(mob);
	}


	//
	// �Էµ� ���� �ȿ� ���ԵǴ� ������ ����� �ּ���.
	List<Monster> listRes = new List<Monster>();
	public List<Monster> FindMonsterListBy(Vector3 pos, float range)
	{
		int posIdx = findMyPosIdx(pos);
		


		listRes.Clear();
		foreach (Monster mob in listMobs[posIdx])
		{
			// �Էµ� pos �� �������� �� ���Ϳ��� �Ÿ��� ����ؼ�
			// range ���� �ȿ� ������ �͵��� ã�´�.
			if (Vector3.Distance(pos, mob.transform.position) < range)
			{
				listRes.Add(mob);
			}
		}
		return listRes;
	}

}
