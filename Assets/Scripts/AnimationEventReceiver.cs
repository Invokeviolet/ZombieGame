using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnAttackEvent();
public delegate void OnAnimEndEvent();
public delegate void OnSkillAttackEvent();

public class AnimationEventReceiver : MonoBehaviour
{
	public OnAttackEvent callbackAttackEvent = null;
	public OnAnimEndEvent callbackAnimEndEvent = null;
	public OnSkillAttackEvent callbackSkillAttackEvent = null;
	//
	// 공격이벤트가 발생했을 때 호출되는 함수
	// (에니메이션 이벤트 이름)
	public void AttackEvent()
	{
		//Debug.Log("## Attack Event");
		if (callbackAttackEvent != null)
			callbackAttackEvent();
	}

	//
	// 애니메이션이 종료될 때 호출되는 이벤트
	public void AnimEndEvent()
	{
		if (callbackAnimEndEvent != null)
			callbackAnimEndEvent();
	}


	public void SkillAttackEvent()
	{
		//Debug.Log("## Skill Attack Event");
		if (callbackSkillAttackEvent != null)
			callbackSkillAttackEvent();

	}
}
