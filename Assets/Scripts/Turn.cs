using UnityEngine;
using System.Collections;

public class Turn : MonoBehaviour {
	public float xDegree = 0,
				 yDegree = 0,
				 zDegree = 0;
	private Quaternion appliedRotation;
	private bool turning = false;

	// Update is called once per frame
	void Start () {
		EventManager.RegisterEvent ("Move", startTurning);
		EventManager.RegisterEvent ("Stop", stopTurning);
		appliedRotation = Quaternion.Euler (Vector3.right * xDegree + Vector3.up * yDegree + Vector3.forward * zDegree);
	}

	void Update () {
		if (turning)
			transform.rotation = appliedRotation * transform.rotation;
	}

	void startTurning () {
		turning = true;
	}

	void stopTurning () {
		turning = false;
	}
}
