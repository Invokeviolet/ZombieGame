using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EffData
{
    public AudioClip myClip;
    public ParticleSystem fx;
}

[CreateAssetMenu(fileName = "New ResourceDataObj", menuName ="ScriptableObjects/ResourceDataObj", order = 5)]
public class ResourceDataObj : ScriptableObject
{
    public float PlayerAttacPower = 1;


    public GameObject EffHit;

    public float Value;
    [SerializeField] int intValue;


    public EffData myEffData;
	public EffData[] myEffDataArray;


    public int GetData()
	{
        return intValue;
    }
}
