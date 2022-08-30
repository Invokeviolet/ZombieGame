using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
	[Header("[��������]")]
	[SerializeField] float width = 1f; // X �� �������� ũ��
	[SerializeField] float height = 1f; // Z �� �������� ũ��
	[Header("[��������]")]
	[SerializeField] float spawnInterval = 3f;
	[SerializeField] int mobLevel = 1;
	//[SerializeField] Monster prefabMob = null;   // ���� ������

	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(processSpawn());
	}

	IEnumerator processSpawn()
	{
		// �����ϰ� ������ ��ġ�� �����ϴ� ����
		Vector3 randomPos = Vector3.zero;
		while (true)
		{
			// spawnInterval ��ŭ ��ٸ��� 
			yield return new WaitForSeconds(spawnInterval);

			// ���� ������ ������ ��ġ�� ����մϴ�.
			randomPos.x = transform.position.x + Random.Range(-width * 0.5f, width * 0.5f);
			randomPos.z = transform.position.z + Random.Range(-height * 0.5f, height * 0.5f);

			// ���͸� ���� spawnPoint �������� �����Ѵ�.
			// ����Ǯ�� ���� 1���� ���� ��û
			Monster mob = MonsterPool.Inst.CreateMonster(randomPos);
			mob.SetData(GameDataMgr.Inst.FindMonsterDataBy(mobLevel));
		}
	}


	// �����Ϳ����� �����ϵ���
#if UNITY_EDITOR
	// ����� �׸� �� ȣ��Ǵ� �Լ�
	private void OnDrawGizmos()
	{
		drawCube(Color.white);
	}

	// ����� ������ �Ǿ��� �� ȣ��Ǵ� �Լ�
	void OnDrawGizmosSelected()
	{
		drawCube(Color.cyan);
	}

	//
	// ������ �������� ť�� 1�� �׸���
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
