using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnInfo
{
	public int MobLevel = 1;
	public int Count = 3;
}

public class SpawnArea_Ver2 : MonoBehaviour
{
	[Header("[��������]")]
	[SerializeField] float width = 1f; // X �� �������� ũ��
	[SerializeField] float height = 1f; // Z �� �������� ũ��
	[Header("[��������]")]
	[SerializeField] float spawnInterval = 3f;
	[SerializeField] SpawnInfo[] SpawnInfos = null;
	//[SerializeField] Monster prefabMob = null;   // ���� ������

	// Start is called before the first frame update
	void Start()
	{
		GameDataMgr.Inst.LoadGameData();
	}


	//
	// GameMgr �� Wave ������ ��, waveNum �� �Է��ؼ� ȣ���� �ش�.
	public void Go(int waveNum)
	{
		StartCoroutine(processSpawn(waveNum));
	}

	//
	// �����ð� ���� ���͸� ����(����)�Ѵ�.
	IEnumerator processSpawn(int waveNum)
	{
		for(int i = 0; i < SpawnInfos[waveNum-1].Count; ++i)
		{
			// ����Ǯ�� ���� 1���� ���� ��û
			Monster mob = MonsterPool.Inst.CreateMonster(getRandomPos());
			mob.SetData(GameDataMgr.Inst.FindMonsterDataBy(SpawnInfos[waveNum - 1].MobLevel));

			// spawnInterval ��ŭ ��ٸ��� 
			yield return new WaitForSeconds(spawnInterval);
		}
	}





	// 
	// 
	Vector3 getRandomPos()
	{
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.z *= height;
		// ���(��ġ�̵�, ȸ��, ������)�� �̿��ؼ� ������ġ�� ��Ȯ�� ���� ����Ѵ�.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);

		Vector3 randomPos = rMat.MultiplyPoint(new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));
		randomPos.y = 0.0f;
		return randomPos;
	}
	// �����Ϳ����� �����ϵ���
#if UNITY_EDITOR
	// ����� �׸� �� ȣ��Ǵ� �Լ�
	private void OnDrawGizmos()
	{
		drawCube(Color.yellow);
	}

	// ����� ������ �Ǿ��� �� ȣ��Ǵ� �Լ�
	void OnDrawGizmosSelected()
	{
		drawCube(Color.green);
	}

	//
	// ������ �������� ť�� 1�� �׸���
	void drawCube(Color drawColor)
	{
		Gizmos.color = drawColor;
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.z *= height;

		// ��ġ�� ȸ���� �������� ����� ����� ���ؼ�
		// Gizmos �� �����ϸ� ���� �׸��� Cube�� ����� ����(��ġ�̵�, ȸ��, ������)�� �޴´�.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);
		Gizmos.matrix = rMat;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
#endif // UNITY_EDITOR
}
