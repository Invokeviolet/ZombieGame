using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
	//=============================================================================
	// 싱글턴
	#region 싱글턴
	static GameMgr instance = null;
    public static GameMgr Inst
	{
		get
		{
            if(instance == null)
			{
				// 먼저 찾아 보고
                instance = FindObjectOfType<GameMgr>();
				// 없다면 새로 생성
                if (instance == null)
                    instance = new GameObject("GameMgr").AddComponent<GameMgr>();
			}
            return instance;
		}
	}
	#endregion // 싱글턴
	//=============================================================================

	[SerializeField] UIManager uiManager = null;


	// Start is called before the first frame update
	void Start()
    {
		StartCoroutine(processGameMgr());
    }


	// 
	// 게임 전체 로직을 관장한다.
	IEnumerator processGameMgr()
	{
		int waveNum = 1;
		while (true)
		{
			// 1. UIManager 에게 웨이브 UI를 출력하도록 요청한다. (그리고 대기~)
			Debug.Log("## Wave UI 연출 시작");
			yield return StartCoroutine(uiManager.processWaveUI(waveNum));

			// 2. 스폰관리자 객체들에게 현재 웨이브의 몬스터를 스폰하도록 요청 (그리고 대기~)
			Debug.Log("## 스포너들에게 스폰 요청");
			SpawnArea_Ver2[] spawner = FindObjectsOfType<SpawnArea_Ver2>();
			foreach(SpawnArea_Ver2 spawnArea in spawner)
			{
				spawnArea.Go(waveNum);
			}


			// 몬스터 가 아직 있다면 
			while (MonsterListMgr.Inst.GetAllCount() > 0)
				yield return null; // 다음 프레임까지 기다리는 것

			// 다음 웨이브
			++waveNum;
		}

	}

}
