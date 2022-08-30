using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnInterval = 3f;
    //[SerializeField] Monster prefabMob = null;   // 몬스터 프리팹

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(processSpawn());
    }

    IEnumerator processSpawn()
	{
        while(true)
		{
            // spawnInterval 만큼 기다리기 
            yield return new WaitForSeconds(spawnInterval);

            // 몬스터를 현재 spawnPoint 기준으로 생성한다.
            //Instantiate(prefabMob, transform.position, Quaternion.identity);
			// 몬스터풀에 몬스터 1개를 생성 요청
			MonsterPool.Inst.CreateMonster(transform.position);
		}
	}

    
}
