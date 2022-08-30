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
// ���ӵ����͸� �ε��ϰ� �˻��ϴµ� ����ϴ� ������ Ŭ����
//
public class GameDataMgr
{
	//------------------------------------------------------------------
	// �̱���
	#region �̱���
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
	// ���ӵ����͸� �ε�����
	public void LoadGameData()
	{
		loadPlayerData();
		loadMonsterData();
	}

	//
	// �÷��̾� ������
	void loadPlayerData()
	{
		TextAsset ta = Resources.Load<TextAsset>("PlayerData");

		// �������� ��� ������ �����ϰ� �ִ�.
		string[] lines = ta.text.Split("\r\n");
		// �ݺ����� 1���� �� ������ ����� �����ϱ� ���ؼ�
		for (int i = 1; i < lines.Length - 1; ++i)
		{
			// ������ 1���� �ĸ��� �����Ѵ�.
			string[] columes = lines[i].Split(',');

			PlayerData playerData = new PlayerData();
			playerData.Level = int.Parse(columes[0]);	// ����
			playerData.Exp = int.Parse(columes[1]);		// ����ġ ������
			playerData.AttackPower = float.Parse(columes[2]);  // ���ݷ�
			playerData.MaxHP = int.Parse(columes[3]);  // max hp

			listPlayerData.Add(playerData);

			//Debug.Log($"���͵����� {i} : Level {mobData.Level} Name {mobData.Name} AP {mobData.AttackPower} MaxHP {mobData.MaxHP}");
		}
	}

	//
	// ���� ������
	void loadMonsterData()
	{ 
		TextAsset ta = Resources.Load<TextAsset>("MonsterData");

		string[] lines = ta.text.Split("\r\n");
		for (int i = 1; i < lines.Length - 1; ++i)
		{
			// ������ 1���� �ĸ��� �����Ѵ�.
			string[] columes = lines[i].Split(',');

			MonsterData mobData = new MonsterData();
			mobData.Level = int.Parse(columes[0]); // ����
			mobData.Name = columes[1].Trim('\"');  // �̸�  : "ddd" -> ddd
			mobData.AttackPower = float.Parse(columes[2]);  // ���ݷ�
			mobData.MaxHP = int.Parse(columes[3]);  // max hp

			listMonsterData.Add(mobData);

			//Debug.Log($"���͵����� {i} : Level {mobData.Level} Name {mobData.Name} AP {mobData.AttackPower} MaxHP {mobData.MaxHP}");
		}
	}




	//
	//
	// ������ �˻��ϴ� �Լ���..
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

	// ����ġ�� �������� �÷��̾� ������ ���ϱ�
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