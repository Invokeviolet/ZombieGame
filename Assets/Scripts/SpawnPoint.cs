using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnInterval = 3f;
    //[SerializeField] Monster prefabMob = null;   // ���� ������

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(processSpawn());
    }

    IEnumerator processSpawn()
	{
        while(true)
		{
            // spawnInterval ��ŭ ��ٸ��� 
            yield return new WaitForSeconds(spawnInterval);

            // ���͸� ���� spawnPoint �������� �����Ѵ�.
            //Instantiate(prefabMob, transform.position, Quaternion.identity);
			// ����Ǯ�� ���� 1���� ���� ��û
			MonsterPool.Inst.CreateMonster(transform.position);
		}
	}

    
}
