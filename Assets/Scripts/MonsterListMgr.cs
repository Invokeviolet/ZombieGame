using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
// 생성된 몬스터들의 목록을 관리해 준다.
//
// 몬스터가 생성되거나, 죽거나 하면 현재 관리되는 목록에도 반영된다.
//
// 플레이어가 목록을 요청하면 적절한 목록을 전달해 준다.

public class MonsterListMgr
{
	//----------------------------------------------------------------
	//
	#region 싱글턴
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
	#endregion // 싱글턴
	//----------------------------------------------------------------

	List<Monster>[] listMobs = new List<Monster>[] { new List<Monster>(), new List<Monster>(), new List<Monster>(), new List<Monster>() };


	// 현재 몬스터 수
	public int GetAllCount()
	{
		int count = 0;
		for(int i =0; i < listMobs.Length; ++i)
			count += listMobs[i].Count;

		return count;
	}

	//
	// 내 위치를 기준으로 속해 있는 영역의 인덱스를 구한다.
	int findMyPosIdx(Vector3 pos)
	{
		if (pos.x < 0f && pos.z > 0f)
			return 0; // 왼쪽 위
		if (pos.x >= 0f && pos.z > 0f)
			return 1;   // 오른쪽 위
		if (pos.x < 0f && pos.z <= 0f)
			return 2; // 왼쪽 아래
		//if (pos.x >= 0f && pos.z <= 0f)
			return 3;   // 오른쪽 아래

	}


	// 몬스터 추가
	public void AddMonster(Monster mob)
	{
		int posIdx = findMyPosIdx(mob.transform.position);
		mob.MyPosIdx = posIdx;
		listMobs[posIdx].Add(mob);
	}

	public void UpdateMonster(Monster mob)
	{
		// 현재 위치를 기준으로 내가 속한 영역이 어딘지 찾는다.
		int posIdx = findMyPosIdx(mob.transform.position);

		// 내가 원래의 기존위치와 달라졌는지 확인해 본다.
		if (mob.MyPosIdx == posIdx || mob.MyPosIdx == -1)
			return;
		// 달라졌다면 기존에 내가 속한 영역에서 나를 제거하고, 새로운 영역에 나를 넣어준다.
		if(mob.MyPosIdx >= 0)
			listMobs[mob.MyPosIdx].Remove(mob);

		mob.MyPosIdx = posIdx;
		listMobs[posIdx].Add(mob);

		//Debug.Log("## 몬스터 위치 Index : " + posIdx);
	}


    // 몬스터 삭제
    public void DelMonster(Monster mob)
	{
		//int posIdx = findMyPosIdx(mob.transform.position);
		listMobs[mob.MyPosIdx].Remove(mob);
	}


	//
	// 입력된 범위 안에 포함되는 몬스터의 목록을 주세요.
	List<Monster> listRes = new List<Monster>();
	public List<Monster> FindMonsterListBy(Vector3 pos, float range)
	{
		int posIdx = findMyPosIdx(pos);
		


		listRes.Clear();
		foreach (Monster mob in listMobs[posIdx])
		{
			// 입력된 pos 를 기준으로 각 몬스터와의 거리를 계산해서
			// range 범위 안에 들어오는 것들을 찾는다.
			if (Vector3.Distance(pos, mob.transform.position) < range)
			{
				listRes.Add(mob);
			}
		}
		return listRes;
	}

}
