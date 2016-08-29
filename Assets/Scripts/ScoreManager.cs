using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : Singleton<ScoreManager> {
	public int specials = 0,
			   maxSpecials = 5;
	public Text scoreText;
	public GameObject HUDPanel,
					  TutorialPanel,
					  WinPanel,
					  LosePanel,
					  immediateScorePanel;
	private bool firstTutorialShot = true;
	private Text immediateScoreText;
	private Vector3 scoreRotation;
	private int specialsInPic = 0,
				immediateSpecials = 0;

	protected ScoreManager() {}

	void Start() {
		scoreText.text = string.Format ("{0}/{1}", 0, Instance.maxSpecials);
		immediateScoreText = immediateScorePanel.GetComponentInChildren <Text> ();
		EventManager.RegisterEvent ("StartPlayerMove", () => {
			specials = 0;
			maxSpecials = 5;
			RefreshHUD ();
		});
	}

	public static void AddSpecial () {
		Instance.specialsInPic++;
	}

	public static void LoseAtEnd () {
		Instance.StartCoroutine (Instance.WaitForShotThenEndGame (false, true));
	}

	private IEnumerator WaitForShotThenEndGame (bool win, bool end = false) {
		yield return new WaitForSeconds (0.1f);
		if (GameStartManager.isGameStarted ()) {
			if (win)
				Instance.WinPanel.SetActive (true);
			else
				Instance.LosePanel.SetActive (true);
			PauseOnTilt.gameOver = true;
			PauseGame ();
		} else {
			StartCoroutine (fadeInTutorialEnd ());
			RefreshHUD ();
		}
	}

	private IEnumerator fadeInTutorialEnd () {
		Graphic[] tutorialGraphics = TutorialPanel.GetComponentsInChildren<Graphic> ().Union (TutorialPanel.GetComponents<Graphic> ()).ToArray ();
		foreach (Graphic tutorialGraphic in tutorialGraphics) 
			tutorialGraphic.CrossFadeAlpha (0.01f, 0f, true);
		yield return new WaitForSeconds (3);
		Instance.TutorialPanel.SetActive (true);
		foreach (Graphic tutorialGraphic in tutorialGraphics) 
			tutorialGraphic.CrossFadeAlpha (1f, 1f, true);
	}

	public static void PauseGame () {
		EventManager.TriggerEvent ("Stop");
		CameraReticle.shotsEnabled = false;
	}

	public static void ResumeGame () {
		EventManager.TriggerEvent ("Move");
		CameraReticle.shotsEnabled = true;
	}

	private static void ResetPicScore () {
		Instance.specialsInPic = 0;
	}

	private static void CumulateScore (int oldScore) {
		Instance.immediateSpecials = Instance.specialsInPic;
		Instance.specials += Instance.specialsInPic;
		Instance.scoreText.text = string.Format ("{0}/{1}", Instance.specials.ToString (), Instance.maxSpecials);
		if (Instance.specials >= Instance.maxSpecials)
			Instance.StartCoroutine (Instance.WaitForShotThenEndGame (true));
	}

	public static void RefreshHUD () {
		Instance.scoreText.text = string.Format ("{0}/{1}", Instance.specials.ToString(), Instance.maxSpecials);
		if (!Instance.firstTutorialShot)
			Instance.StartCoroutine (ShowImmediateScore (Instance.specialsInPic));
		else {
			Instance.firstTutorialShot = false;
			PauseOnTilt.occupied = true;
		}
		ResetPicScore ();
	}

	private static IEnumerator ShowImmediateScore (int picScore) {
		yield return new WaitForSeconds (0.25f);
		Instance.immediateScoreText.text = string.Format ("+{0}", Instance.immediateSpecials);
		Instance.immediateScorePanel.SetActive (true);
		yield return new WaitForSeconds (1.0f);
		Instance.immediateScorePanel.SetActive (false);
	}
		
	public static bool CanGetBonus (GameObject go) {
		return !EventManager.HasBeenInvoked (go);
		//CameraReticle.mapGOtoFacing [go] && 
	}

	public static void changeScore(HashSet<GameObject> hits) {
		int oldScore = Instance.specials;
		foreach (GameObject go in hits) {
			if (CanGetBonus (go)) 
				EventManager.TriggerGameobject (go);
		}
		CumulateScore (oldScore);
		RefreshHUD ();
	}
}
