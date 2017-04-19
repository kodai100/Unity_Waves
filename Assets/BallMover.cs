using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MonoBehaviour {

    public Vector3 center;
    public float diameter;

	// Use this for initialization
	void Start () {
		
	}

    float time = 0;

	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        transform.position = center + new Vector3(diameter * Mathf.Sin(2 * time), 0, diameter * Mathf.Sin(time));
    }
}
