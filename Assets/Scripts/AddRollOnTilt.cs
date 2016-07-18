using UnityEngine;
using System.Collections;

public class AddRollOnTilt : MonoBehaviour {
	private readonly float tiltLimit = Mathf.Sqrt (2) / 2;
	private bool rollAdded = false;
	private float cosOfplayerHeadZAngle;
	void Update () {
		cosOfplayerHeadZAngle = Mathf.Cos (transform.eulerAngles.z * Mathf.PI / 180);
		if (!rollAdded && cosOfplayerHeadZAngle < tiltLimit) {
			rollAdded = true;
			ScoreManager.AddRollWithCost ();
		}
		if (rollAdded && cosOfplayerHeadZAngle > tiltLimit) 
			rollAdded = false;
	}
}
