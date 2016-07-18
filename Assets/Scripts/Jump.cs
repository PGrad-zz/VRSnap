using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour {
	private Rigidbody rb;
	private bool jumped = false;
	private float initY;
	private Collider collider;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		initY = transform.position.y;
		EventManager.RegisterGameobject (gameObject, catchIt);
		collider = gameObject.GetComponent<MeshCollider> ();
		collider.enabled = false;
	}

	public void OnTriggerEnter (Collider other) {
		if (!jumped && other.name == "Player") {
			transform.parent = other.transform;
			jumpIn ();
		}
	}

	private void jumpIn () {
		gameObject.GetComponent<MeshCollider> ().enabled = true;
		gameObject.GetComponent<Rigidbody> ().useGravity = true;
		rb.AddForce (Vector3.right * 325 + Vector3.up * 100 + Vector3.forward * -550);
		jumped = true;
		StartCoroutine (croak ());
	}

	private IEnumerator croak () {
		yield return new WaitForSeconds (1.0f);
		GetComponents<AudioSource> () [0].Play ();
		yield return new WaitForSeconds (3.0f);
		jumpOut ();
	}

	private void jumpOut () {
		rb.AddForce (Vector3.right * 300 + Vector3.up * 300 + Vector3.forward * -1000);
		GetComponents<AudioSource> () [1].Play ();
	}

	public void catchIt () {
		ScoreManager.MultiplyScore (2);
	}
}
