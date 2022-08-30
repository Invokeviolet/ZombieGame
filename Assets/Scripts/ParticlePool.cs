using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
	// 싱글턴으로 만들자
	#region 싱글톤
	static ParticlePool instance = null;
	public static ParticlePool Inst
	{
		get
		{
			if (instance == null)
			{
				// 하이러키에 존재하는 게임오브젝트라면 먼저 찾아본다.
				instance = FindObjectOfType<ParticlePool>();
				if (instance == null)
					instance = new GameObject("ParticlePool").AddComponent<ParticlePool>();
			}
			return instance;
		}
	}
	#endregion // Singleton
	//-----------------------------------------------------------------------------

	public GameObject prefabParticle;

	ObjectPool<GameObject> pool;
    public IObjectPool<GameObject> Pool
	{
		get
		{
			if (pool == null)
				pool = new ObjectPool<GameObject>(onCreateFunc, onGet, onRelease, onDestroy);
			return pool;
		}
	}

	//---------------------------------------------------------------
	// 유니티 엔진에서 제공하는 ObjectPool 의 생성과정에서 필요한 함수들 정의
	#region ObjectPool 함수

	// 객체를 생성할 때는 호출되는 함수
	GameObject onCreateFunc()
	{
		GameObject inst = Instantiate(prefabParticle);
		return inst;
	}

	// GameOjbect 한개를 활성화해서 전달해 준다.
	void onGet(GameObject obj)
	{
		obj.SetActive(true);
	}

	// 객체 한개를 비활성화해서 전달해 주는 함수
	void onRelease(GameObject obj)
	{
		obj.SetActive(false); ;
	}

	// 객체 한개를 삭제
	void onDestroy(GameObject obj)
	{
		Destroy(obj);
	}
	#endregion //
	//---------------------------------------------------------------


}
