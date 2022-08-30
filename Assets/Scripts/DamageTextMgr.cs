using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextMgr : MonoBehaviour
{
	//----------------------------------------------------------------------
	// 싱글턴 객체로 만들자.
	static DamageTextMgr instance;
	public static DamageTextMgr Inst
	{
		get
		{
			if(instance == null)
			{
				// 하이라키(Hiearchy)에 존재하는 것이 있는지 먼저 찾아보고
				instance = FindObjectOfType<DamageTextMgr>();
				// 없으면 새로 다시 만들고
				if(instance == null)
					instance = new GameObject("DamageTextMgr").AddComponent<DamageTextMgr>();
			}
			return instance;
		}
	}
	//----------------------------------------------------------------------

	Canvas canvas = null;
	private void Awake()
	{
		canvas = FindObjectOfType<Canvas>();
	}







	[SerializeField] Text prefabDamageText = null; // 데미지텍스트 프리팹


	// 데미지숫자를 출력하도록 요청을 받으면
	// 프리팹을 참조해서 1개 객체를 생성합니다.
	public void AddText(float damageValue, Vector3 outputPos, Vector3 offsetPos)
	{
		// 입력된 outputPos는 3D 공간 상의 좌표이므로
		// 이 것을 2D UI 공간 상의 좌표로 변환이 필요하다.

		// 1. 카메라 기준으로 화면 좌표계로 변경
		Vector3 screenPos = Camera.main.WorldToScreenPoint(outputPos + offsetPos);

		Text instTxt = Instantiate(prefabDamageText, screenPos, Quaternion.identity, canvas.transform);
		instTxt.text = ((int)damageValue).ToString();
	}
    
}
