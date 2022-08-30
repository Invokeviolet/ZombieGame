using UnityEngine;
using System.Collections;

public class MaskAlphaAni : MonoBehaviour {
	public float LifeTime = 2.0f;
	public float Speed = 1.0f;

	// Use this for initialization
	void Start () {
		StartCoroutine(animRoutine());
	}
	
	IEnumerator animRoutine()
	{
		yield return new WaitForSeconds(LifeTime);

		float curAlpha = 1.0f;
		bool finished = false;
		while (finished == false)
		{
			curAlpha -= Time.deltaTime * Speed;
			transform.GetComponent<Renderer>().material.SetFloat("_MaskAlpha", curAlpha);

			if (curAlpha <= 0.0f)
				finished = true;

			yield return null;
		}
	}
}
