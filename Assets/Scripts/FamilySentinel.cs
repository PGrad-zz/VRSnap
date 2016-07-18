using UnityEngine;
using System.Collections;

public class FamilySentinel : MonoBehaviour {
	public bool notSearched = true;
	void Start () {
		EventManager.RegisterGameobject (gameObject, Alert);
	}

	public void Alert () {
		if (notSearched)
			transform.parent.gameObject.GetComponent<Family> ().searchFamily ();
	}

	public void Searched () {
		notSearched = false;
	}

	public void Open () {
		notSearched = true;
	}
}
