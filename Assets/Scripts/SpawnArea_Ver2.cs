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
	[Header("[영역정보]")]
	[SerializeField] float width = 1f; // X 축 기준으로 크기
	[SerializeField] float height = 1f; // Z 축 기준으로 크기
	[Header("[몬스터정보]")]
	[SerializeField] float spawnInterval = 3f;
	[SerializeField] SpawnInfo[] SpawnInfos = null;
	//[SerializeField] Monster prefabMob = null;   // 몬스터 프리팹

	// Start is called before the first frame update
	void Start()
	{
		GameDataMgr.Inst.LoadGameData();
	}


	//
	// GameMgr 가 Wave 시작할 때, waveNum 을 입력해서 호출해 준다.
	public void Go(int waveNum)
	{
		StartCoroutine(processSpawn(waveNum));
	}

	//
	// 일정시간 마다 몬스터를 생성(스폰)한다.
	IEnumerator processSpawn(int waveNum)
	{
		for(int i = 0; i < SpawnInfos[waveNum-1].Count; ++i)
		{
			// 몬스터풀에 몬스터 1개를 생성 요청
			Monster mob = MonsterPool.Inst.CreateMonster(getRandomPos());
			mob.SetData(GameDataMgr.Inst.FindMonsterDataBy(SpawnInfos[waveNum - 1].MobLevel));

			// spawnInterval 만큼 기다리기 
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
		// 행렬(위치이동, 회전, 스케일)을 이용해서 랜덤위치의 정확한 값을 계산한다.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);

		Vector3 randomPos = rMat.MultiplyPoint(new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f)));
		randomPos.y = 0.0f;
		return randomPos;
	}
	// 에디터에서만 동작하도록
#if UNITY_EDITOR
	// 기즈모를 그릴 때 호출되는 함수
	private void OnDrawGizmos()
	{
		drawCube(Color.yellow);
	}

	// 기즈모가 선택이 되었을 때 호출되는 함수
	void OnDrawGizmosSelected()
	{
		drawCube(Color.green);
	}

	//
	// 지정된 색상으로 큐브 1개 그리기
	void drawCube(Color drawColor)
	{
		Gizmos.color = drawColor;
		Vector3 size = transform.lossyScale;
		size.x *= width;
		size.z *= height;

		// 위치와 회전과 스케일이 적용된 행렬을 구해서
		// Gizmos 에 적용하면 이후 그리는 Cube는 행렬의 영향(위치이동, 회전, 스케일)을 받는다.
		Matrix4x4 rMat = Matrix4x4.TRS(transform.position, transform.rotation, size);
		Gizmos.matrix = rMat;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}
#endif // UNITY_EDITOR
}
