using UnityEngine;
using System.Collections;

public class Jump : SpecialMoment {
	private Rigidbody rb;
	private bool jumped = false;
	private Collider mCollider;

	void Awake () {
		rb = GetComponent<Rigidbody> ();
		mCollider = gameObject.GetComponent<MeshCollider> ();
		mCollider.enabled = false;
	}

	void Start () {
		EventManager.RegisterGameobject (gameObject, catchIt);
	}

	public void OnTriggerEnter (Collider other) {
		if (!jumped && other.name == "Player") {
			transform.parent = other.transform;
			ShowSpecial ();
		}
	}

	protected override void ShowSpecial () {
		base.ShowSpecial ();
		jumpIn ();
	}

	private void jumpIn () {
		gameObject.GetComponent<MeshCollider> ().enabled = true;
		gameObject.GetComponent<Rigidbody> ().useGravity = true;
		rb.AddForce (Vector3.right * 300 + Vector3.up * 100 + Vector3.forward * -500);
		jumped = true;
		StartCoroutine (croak ());
	}

	private IEnumerator croak () {
		yield return new WaitForSeconds (1.0f);
		GetComponents<AudioSource> () [0].Play ();
		yield return new WaitForSeconds (3.0f);
		HideSpecial ();
	}

	protected override void HideSpecial () {
		jumpOut ();
		base.HideSpecial ();
	}

	private void jumpOut () {
		rb.AddForce (Vector3.up * 300 + Vector3.right * 1000);
		GetComponents<AudioSource> () [1].Play ();
	}

	public void catchIt () {
		ScoreManager.MultiplyScore (2);
	}
}
