using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
	[Header("[영역정보]")]
	[SerializeField] float width = 1f; // X 축 기준으로 크기
	[SerializeField] float height = 1f; // Z 축 기준으로 크기
	[Header("[몬스터정보]")]
	[SerializeField] float spawnInterval = 3f;
	[SerializeField] int mobLevel = 1;
	//[SerializeField] Monster prefabMob = null;   // 몬스터 프리팹

	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(processSpawn());
	}

	IEnumerator processSpawn()
	{
		// 랜덤하게 설정된 위치를 저장하는 변수
		Vector3 randomPos = Vector3.zero;
		while (true)
		{
			// spawnInterval 만큼 기다리기 
			yield return new WaitForSeconds(spawnInterval);

			// 영역 내에서 임의의 위치를 계산합니다.
			randomPos.x = transform.position.x + Random.Range(-width * 0.5f, width * 0.5f);
			randomPos.z = transform.position.z + Random.Range(-height * 0.5f, height * 0.5f);

			// 몬스터를 현재 spawnPoint 기준으로 생성한다.
			// 몬스터풀에 몬스터 1개를 생성 요청
			Monster mob = MonsterPool.Inst.CreateMonster(randomPos);
			mob.SetData(GameDataMgr.Inst.FindMonsterDataBy(mobLevel));
		}
	}


	// 에디터에서만 동작하도록
#if UNITY_EDITOR
	// 기즈모를 그릴 때 호출되는 함수
	private void OnDrawGizmos()
	{
		drawCube(Color.white);
	}

	// 기즈모가 선택이 되었을 때 호출되는 함수
	void OnDrawGizmosSelected()
	{
		drawCube(Color.cyan);
	}

	//
	// 지정된 색상으로 큐브 1개 그리기
	void drawCube(Color drawColor)
	{
		Gizmos.color = drawColor;
		Vector3 size = Vector3.one;
		size.x *= width;
		size.z *= height;
		Gizmos.DrawCube(transform.position, size);
	}
#endif // UNITY_EDITOR
}
