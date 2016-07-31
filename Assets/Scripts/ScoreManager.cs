using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
public class ScoreManager : Singleton<ScoreManager> {
	public int score = 0,
			   rolls = 5,
			   maxScore = 100;
	public Text scoreText,
				rollText;
	public Image rollImage,
			     scoreImage;
	public GameObject HUDPanel,
					  WinPanel,
					  RollLosePanel,
					  EndLosePanel,
					  immediateScorePanel;
	private Text immediateScoreText;
	private Transform scoreTransform;
	private Vector3 scoreRotation;
	private int multiplier = 1,
				oldMultiplier = 1,
				picScore = 0,
				picScoreWOMultiplier = 0,
				rollCost = 20;

	protected ScoreManager() {}

	void Start() {
		scoreText.text = string.Format ("{0}/{1}", 0, Instance.maxScore);
		rollText.text = string.Format("x{0}",rolls.ToString());
		immediateScoreText = immediateScorePanel.GetComponent <Text> ();
		scoreTransform = scoreImage.transform;
	}

	public static void AddBonusRoll () {
		Instance.rolls += 1;
	}

	public static void AddRollWithCost () {
		if (Instance.score >= Instance.rollCost) {
			Instance.rolls++;
			Instance.score -= Instance.rollCost;
		}
		RefreshHUD ();
	}

	public static void AddPoints(int hits) {
		Instance.picScore += hits;
	}

	public static void MultiplyScore (int multiplier) {
		Instance.multiplier *= multiplier;
	}

	private static void ApplyMultiplier () {
		Instance.picScoreWOMultiplier = Instance.picScore;
		Instance.oldMultiplier = Instance.multiplier;
		Instance.picScore *= Instance.multiplier;
		Instance.multiplier = 1;
	}

	private static void RemoveRoll() {
		Instance.rollText.text = string.Format ("x{0}", (--Instance.rolls).ToString ());
		if (Instance.rolls < 1) 
			Instance.StartCoroutine (Instance.WaitForShotThenEndGame (false));
	}

	public static void LoseAtEnd () {
		Instance.StartCoroutine (Instance.WaitForShotThenEndGame (false, true));
	}

	private IEnumerator WaitForShotThenEndGame (bool win, bool end = false) {
		yield return new WaitForSeconds (0.1f);
		if (win)
			Instance.WinPanel.SetActive (true);
		else {
			if (end)
				Instance.EndLosePanel.SetActive (true);
			else
				Instance.RollLosePanel.SetActive (true);
		}
		PauseOnTilt.gameOver = true;
		PauseGame ();
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
		Instance.picScore = 0;
	}

	private static void CumulateScore (int oldScore) {
		Instance.score += Instance.picScore;
		Instance.scoreText.text = string.Format ("{0}/{1}", Instance.score.ToString (), Instance.maxScore);
		Instance.scoreTransform.rotation = Quaternion.Euler (Instance.scoreTransform.right * -0.05f * (Instance.score - oldScore) ) * Instance.scoreTransform.rotation;
		if (Instance.score >= Instance.maxScore)
			Instance.StartCoroutine (Instance.WaitForShotThenEndGame (true));
		else
			RemoveRoll ();
	}

	public static void RefreshHUD () {
		Instance.rollText.text = string.Format ("x{0}", Instance.rolls.ToString());
		Instance.scoreText.text = string.Format ("{0}/{1}", Instance.score.ToString(), Instance.maxScore);
		Instance.StartCoroutine (ShowImmediateScore (Instance.picScore));
		ResetPicScore ();
	}

	private static IEnumerator ShowImmediateScore (int picScore) {
		yield return new WaitForSeconds (0.25f);
		Instance.immediateScoreText.text = string.Format ("+{0,2} x {1}", Instance.picScoreWOMultiplier, Instance.oldMultiplier);
		Instance.immediateScorePanel.SetActive (true);
		yield return new WaitForSeconds (1.0f);
		Instance.immediateScorePanel.SetActive (false);
	}
		
	public static bool CanGetBonus (GameObject go) {
		return CameraReticle.mapGOtoFacing [go] && !EventManager.HasBeenInvoked (go);
	}

	public static void changeScore(HashSet<GameObject> hits) {
		int oldScore = Instance.score,
			objScore = 0;
		foreach (GameObject go in hits) {
			if (!DataService.getScore (go, out objScore))
				throw new UnityException ("Object not registered with database, cannot get score!");
			AddPoints (objScore);
			if (CanGetBonus (go)) 
				EventManager.TriggerGameobject (go);
		}
		ApplyMultiplier ();
		CumulateScore (oldScore);
		RefreshHUD ();
	}
}
