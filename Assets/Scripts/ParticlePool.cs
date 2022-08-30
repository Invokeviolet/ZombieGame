using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
	// �̱������� ������
	#region �̱���
	static ParticlePool instance = null;
	public static ParticlePool Inst
	{
		get
		{
			if (instance == null)
			{
				// ���̷�Ű�� �����ϴ� ���ӿ�����Ʈ��� ���� ã�ƺ���.
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
	// ����Ƽ �������� �����ϴ� ObjectPool �� ������������ �ʿ��� �Լ��� ����
	#region ObjectPool �Լ�

	// ��ü�� ������ ���� ȣ��Ǵ� �Լ�
	GameObject onCreateFunc()
	{
		GameObject inst = Instantiate(prefabParticle);
		return inst;
	}

	// GameOjbect �Ѱ��� Ȱ��ȭ�ؼ� ������ �ش�.
	void onGet(GameObject obj)
	{
		obj.SetActive(true);
	}

	// ��ü �Ѱ��� ��Ȱ��ȭ�ؼ� ������ �ִ� �Լ�
	void onRelease(GameObject obj)
	{
		obj.SetActive(false); ;
	}

	// ��ü �Ѱ��� ����
	void onDestroy(GameObject obj)
	{
		Destroy(obj);
	}
	#endregion //
	//---------------------------------------------------------------


}
