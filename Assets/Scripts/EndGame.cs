using UnityEngine;
using System.Collections;

public class EndGame : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.name == "Player")
			ScoreManager.LoseAtEnd ();
	}
}
