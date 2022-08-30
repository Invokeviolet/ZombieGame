using UnityEngine;
using System.Collections;

public class ShadowProj : MonoBehaviour {

	Transform _transform;
	Vector3 _newPos = Vector3.zero;
	// Use this for initialization
	void Start () {
		_transform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		_newPos = _transform.position;
		_newPos.y = 0.0f;
		_transform.position = _newPos;
	}
}
