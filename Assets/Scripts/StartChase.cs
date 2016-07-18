using UnityEngine;
using System.Collections;

public class StartChase : MonoBehaviour {
	bool tripped = false;
	public GameObject predator,
					  prey;

	void OnTriggerEnter (Collider other) {
		if (!tripped && other.name == "Player") {
			(predator = (GameObject)Instantiate (predator, transform.position, transform.rotation)).transform.parent = transform;
			(prey = (GameObject)Instantiate (prey, transform.position + transform.forward * 10 + new Vector3(0,-1f,0), transform.rotation)).transform.parent = transform;
			predator.GetComponent<MoveToTarget> ().target = prey;
			BroadcastMessage ("getMoving");
			tripped = true;
		}
	}
}
