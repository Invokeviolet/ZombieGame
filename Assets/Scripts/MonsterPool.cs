using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
//
// MonsterPool 은 몬스터를 생성하고, 삭제하는데 있어서 
// 좀 더 효율적인 방법을 구현하고자 하는 게임오브젝트
// 즉, Object(Monster) Pooling 하는 겠다는 뜻

public class MonsterPool : MonoBehaviour
{
	// 싱글턴으로 만들자
	#region 싱글톤
	static MonsterPool instance = null;
    public static MonsterPool Inst
	{
		get
		{
			if(instance == null)
			{
				// 하이러키에 존재하는 게임오브젝트라면 먼저 찾아본다.
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
		// (파일로 존재하는)프리팹을 로드하는 함수이다.
		prefabMob = Resources.Load<Monster>("Monster");
	}

	//
	// 생성 요청 
	public Monster CreateMonster( Vector3 pos)
	{
		Monster instMob = null;
		//처음에는 아무것도 없으니 생성하자
		if (pool.Count == 0)
		{
			// 로드한 프리팹을 이용해서 인스턴트 객체 한개를 만든다.
 			instMob = Instantiate(prefabMob, pos, Quaternion.identity, this.transform);
			// 몬스터 목록관리자에게 1개 추가
			MonsterListMgr.Inst.AddMonster(instMob);
 			return instMob;
		}

		// 기존의 목록 중에서 비활성화 된 것을 찾는다.
		// 지금 큐에서는 맨 앞에 있는 것 하나를 전달해 주면
		instMob = pool.Dequeue();
		//instMob.transform.parent = null;
		instMob.transform.position = pos;
		instMob.transform.rotation = Quaternion.identity;
		instMob.gameObject.SetActive(true);
		// 몬스터 목록관리자에게 1개 추가
		MonsterListMgr.Inst.AddMonster(instMob);
		return instMob;
	}

	//
	// 비활성화 
	public void DestroyMonster(Monster mob)
	{
		//mob.transform.parent = this.transform;
		mob.gameObject.SetActive(false);
		pool.Enqueue(mob); // pool 에 1개 늘어난다.

		// 몬스터 목록관리자에게 1개 삭제
		MonsterListMgr.Inst.DelMonster(mob);
	}

	


}
