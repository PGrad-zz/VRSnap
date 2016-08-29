using UnityEngine;
using System.Collections;

public class FamilySentinel : MonoBehaviour, ISpecialInvoked {
	public bool notSearched = true,
				lead = false;
	void Start () {
		EventManager.RegisterGameObject (gameObject, Alert);
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

	public void SpecialIsInvoked () {
		EventManager.NowInvoked (gameObject);
	}
}
