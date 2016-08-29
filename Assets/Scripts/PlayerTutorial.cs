using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class PlayerTutorial : MonoBehaviour, IGvrGazeResponder {
	public Image howToPause,
				 howToImage,
				 pointToFrame;
	public GameObject clone,
					  HUDPanel;
	private bool gazeEnteredOnce = false,
				 tookFirstShot = false,
			     took2ndShot = false;
	private Animator anim;
	private GameObject parent;
	private GameObject[] siblings;
	private Graphic[] HUDgraphics;
	private AudioSource hey;

	void Awake () {
		anim = GetComponent<Animator> ();
		siblings = new GameObject[2];
		hey = GetComponent<AudioSource> ();
	}

	void Start () {
		EventManager.RegisterEvent ("StartPlayerMove", () => Destroy (parent));
		HUDgraphics = HUDPanel.GetComponentsInChildren<Graphic> ().Union (HUDPanel.GetComponents<Graphic> ()).ToArray ();
		hideHUDPanel ();
	}

	void hideHUDPanel () {
		foreach (Graphic graphic in HUDgraphics)
			graphic.enabled = false;
	}

	void fadeInHUDPanel () {
		foreach (Graphic graphic in HUDgraphics) {
			graphic.enabled = true;
			graphic.CrossFadeAlpha (0.1f, 0f, true);
			graphic.CrossFadeAlpha (1f, 1f, true);
		}
		CameraReticle.shotsEnabled = true;
	}

	public void OnGazeEnter () {
		if (!gazeEnteredOnce) {
			StartCoroutine (ChangeHUDColors (false));
			gazeEnteredOnce = true;
			hey.Play ();
		}
	}

	private IEnumerator ChangeHUDColors (bool waitForShot) {
		if (waitForShot) {
			yield return new WaitForSeconds (2);
			CameraReticle.shotsEnabled = false;
			pointToFrame.CrossFadeAlpha (255f, 1f, true);
			yield return new WaitForSeconds (3);
			pointToFrame.CrossFadeAlpha (1f, 1f, true);
			yield return new WaitForSeconds (1);
			Destroy (pointToFrame.gameObject);
			fadeInHUDPanel ();
			hey.Play ();
		} else {
			yield return new WaitForSeconds (1);
			howToImage.CrossFadeAlpha (255f, 1f, true);
			yield return new WaitForSeconds (1);
			CameraReticle.shotsEnabled = true;
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
			} else if (!took2ndShot) {
				took2ndShot = true;
				anim.SetTrigger ("Success");
				StartCoroutine (SpawnSiblings ());
			} else {
				anim.SetTrigger ("Success");
				foreach (GameObject sibling in siblings)
					sibling.GetComponent<Animator> ().SetTrigger ("Success");
			}
		}
	}

	private IEnumerator SpawnSiblings () {
		yield return new WaitForSeconds (2);
		Vector3 xShift = new Vector3 (2, 0, 0);
		Quaternion turnAround = Quaternion.Euler (new Vector3 (0, 180f, 0));
		parent = Instantiate (new GameObject (), transform.position, transform.rotation) as GameObject;
		transform.parent = parent.transform;
		gameObject.AddComponent<FamilySentinel> ().lead = true;
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
		hey.Play ();
	}
}
