using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerTutorial : MonoBehaviour, IGvrGazeResponder {
	public Image rollPanel,
				 scorePanel,
				 howToImage,
				 pointToFrame;
	public GameObject clone;
	private bool gazeEnteredOnce = false,
				 tookFirstShot = false;
	private Animator anim;
	private GameObject[] siblings;

	void Awake () {
		anim = GetComponent<Animator> ();
		siblings = new GameObject[2];
	}

	public void OnGazeEnter () {
		if (!gazeEnteredOnce) {
			StartCoroutine (ChangeHUDColors (false));
			gazeEnteredOnce = true;
		}
	}

	private IEnumerator ChangeHUDColors (bool waitForShot) {
		if (waitForShot) {
			yield return new WaitForSeconds (1);
			CameraReticle.shotsEnabled = false;
		}
		scorePanel.CrossFadeAlpha (255f, 1f, true);
		yield return new WaitForSeconds (1);
		rollPanel.CrossFadeAlpha (255f, 1f, true);
		yield return new WaitForSeconds (2);
		if (!waitForShot) {
			howToImage.CrossFadeAlpha (255f, 1f, true);
			yield return new WaitForSeconds (1);
			CameraReticle.shotsEnabled = true;
		}
		else {
			scorePanel.CrossFadeAlpha (1f, 1f, true);
			yield return new WaitForSeconds (1);
			rollPanel.CrossFadeAlpha (1f, 1f, true);
			yield return new WaitForSeconds (1);
			pointToFrame.CrossFadeAlpha (255f, 1f, true);
			yield return new WaitForSeconds (3);
			pointToFrame.CrossFadeAlpha (1f, 1f, true);
			yield return new WaitForSeconds (1);
			Destroy (pointToFrame.gameObject);
			SpawnSiblings ();
		}
	}

	public void OnGazeStay () {
	}

	public void OnGazeExit () {
	}

	public void OnGazeTrigger () {
		if (CameraReticle.shotsEnabled) {
			if (!tookFirstShot) {
				tookFirstShot = true;
				Destroy (howToImage.gameObject);
				anim.SetTrigger ("Success");
				StartCoroutine (ChangeHUDColors (true));
			} else {
				anim.SetTrigger ("Success");
				foreach (GameObject sibling in siblings)
					sibling.GetComponent<Animator> ().SetTrigger ("Success");
			}
		}
	}

	private void SpawnSiblings () {
		Vector3 xShift = new Vector3 (2, 0, 0);
		Quaternion turnAround = Quaternion.Euler (new Vector3 (0, 180f, 0));
		GameObject parent = Instantiate (new GameObject (), transform.position, transform.rotation) as GameObject;
		transform.parent = parent.transform;
		gameObject.AddComponent<FamilySentinel> ();
		for (int sibling = 0; sibling < 2; sibling++) {
			siblings [sibling] = ((GameObject) Instantiate (clone, transform.position + xShift, Quaternion.identity));
			siblings [sibling].transform.rotation = turnAround * siblings [sibling].transform.rotation;
			xShift.x = -xShift.x;
			siblings [sibling].GetComponent<Animator> ().Play ("Hello", -1, anim.GetCurrentAnimatorStateInfo (0).normalizedTime);
			siblings [sibling].transform.parent = parent.transform;
			siblings [sibling].AddComponent<FamilySentinel> ();
		}
		parent.AddComponent <Family> ();
		CameraReticle.shotsEnabled = true;
	}
}
