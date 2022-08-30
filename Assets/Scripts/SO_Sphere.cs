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
		// 0.5�ʸ��� �ѹ��� �浹üũ
		if (Time.time - oldTime < 0.1f)
			return;
		oldTime = Time.time;



		// �� Ÿ���� �� ���ɼ��� ���� Monster ���� ����� ���ϱ�
		Monster[] mobs = FindObjectsOfType<Monster>();
		for (int i = 0; i < mobs.Length; i++)
		{
			if (mobs[i].CheckWithCollider(myCollider))
			{
				// mobs[i]�� ���� SphereCollider �� �浹�� ���̴�.
				//Debug.Log(mobs[i].name);
				mobs[i].SendMessage("TransferDamge", 10f, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
