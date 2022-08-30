using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PlayerData
{
	public int Level;
	public int Exp;
	public float AttackPower;
	public int MaxHP;
}

public struct MonsterData 
{
	public int Level;
	public string Name;
	public float AttackPower;
	public int MaxHP;
}

//
//
// 게임데이터를 로드하고 검색하는데 사용하는 관리자 클래스
//
public class GameDataMgr
{
	//------------------------------------------------------------------
	// 싱글턴
	#region 싱글턴
	static GameDataMgr inst = null;
    public static GameDataMgr Inst
	{
		get
		{
			if(inst == null)
			{
				inst = new GameDataMgr();
			}
			return inst;
		}
	}
	#endregion //
	//------------------------------------------------------------------

	List<PlayerData> listPlayerData = new List<PlayerData>();
	List<MonsterData> listMonsterData = new List<MonsterData>();


	//
	// 게임데이터를 로드하자
	public void LoadGameData()
	{
		loadPlayerData();
		loadMonsterData();
	}

	//
	// 플레이어 데이터
	void loadPlayerData()
	{
		TextAsset ta = Resources.Load<TextAsset>("PlayerData");

		// 데이터의 모든 라인을 포함하고 있다.
		string[] lines = ta.text.Split("\r\n");
		// 반복문을 1부터 한 이유는 헤더를 제외하기 위해서
		for (int i = 1; i < lines.Length - 1; ++i)
		{
			// 데이터 1줄을 컴마로 구분한다.
			string[] columes = lines[i].Split(',');

			PlayerData playerData = new PlayerData();
			playerData.Level = int.Parse(columes[0]);	// 레벨
			playerData.Exp = int.Parse(columes[1]);		// 경험치 누적값
			playerData.AttackPower = float.Parse(columes[2]);  // 공격력
			playerData.MaxHP = int.Parse(columes[3]);  // max hp

			listPlayerData.Add(playerData);

			//Debug.Log($"몬스터데이터 {i} : Level {mobData.Level} Name {mobData.Name} AP {mobData.AttackPower} MaxHP {mobData.MaxHP}");
		}
	}

	//
	// 몬스터 데이터
	void loadMonsterData()
	{ 
		TextAsset ta = Resources.Load<TextAsset>("MonsterData");

		string[] lines = ta.text.Split("\r\n");
		for (int i = 1; i < lines.Length - 1; ++i)
		{
			// 데이터 1줄을 컴마로 구분한다.
			string[] columes = lines[i].Split(',');

			MonsterData mobData = new MonsterData();
			mobData.Level = int.Parse(columes[0]); // 레벨
			mobData.Name = columes[1].Trim('\"');  // 이름  : "ddd" -> ddd
			mobData.AttackPower = float.Parse(columes[2]);  // 공격력
			mobData.MaxHP = int.Parse(columes[3]);  // max hp

			listMonsterData.Add(mobData);

			//Debug.Log($"몬스터데이터 {i} : Level {mobData.Level} Name {mobData.Name} AP {mobData.AttackPower} MaxHP {mobData.MaxHP}");
		}
	}




	//
	//
	// 데이터 검색하는 함수들..
	public MonsterData FindMonsterDataBy(int level)
	{
		MonsterData mobData = listMonsterData.Find( mobData => mobData.Level == level);
		return mobData;
	}

	public PlayerData FindPlayerDataBy(int level)
	{
		PlayerData playerData = listPlayerData.Find(pData => pData.Level == level);
		return playerData;
	}

	// 경험치를 기준으로 플레이어 데이터 구하기
	public PlayerData FindPlayerDataByExp(int exp)
	{
		for(int i = 0; i < listPlayerData.Count; ++i)
		{
			if (listPlayerData[i].Exp >= exp)
			{
				return listPlayerData[i];
			}
		}

		return listPlayerData[listPlayerData.Count - 1];
	}

}