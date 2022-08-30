using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Sphere : MonoBehaviour
{
	SphereCollider myCollider = null;
	float oldTime = 0f;

	private void Awake()
	{
		myCollider = GetComponent<SphereCollider>();

	}
	// Start is called before the first frame update
	void Start()
	{

	}


	// Update is called once per frame
	void Update()
	{
		// 0.5초마다 한번씩 충돌체크
		if (Time.time - oldTime < 0.1f)
			return;
		oldTime = Time.time;



		// 내 타겟이 될 가능성이 높은 Monster 들의 목록을 구하기
		Monster[] mobs = FindObjectsOfType<Monster>();
		for (int i = 0; i < mobs.Length; i++)
		{
			if (mobs[i].CheckWithCollider(myCollider))
			{
				// mobs[i]와 나의 SphereCollider 가 충돌한 것이다.
				//Debug.Log(mobs[i].name);
				mobs[i].SendMessage("TransferDamge", 10f, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
