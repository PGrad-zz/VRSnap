using UnityEngine;
using System.Collections;

public class GetAnimalsMoving : MonoBehaviour {
	bool tripped = false;

	void OnTriggerEnter (Collider other) {
		if (!tripped && other.name == "Player") {
			EventManager.TriggerEvent ("Start");
			tripped = true;
		}
	}
}
