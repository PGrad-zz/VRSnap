// This is free and unencumbered software released into the public domain.
// For more information, please refer to <http://unlicense.org/>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handles scene transitions by Transition() and the beginning of the scenes through Start().
public class SceneTransition : MonoBehaviour
{
	public float fadeTime = 2.0f;
	public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
	public Material fadeMaterial = null;

	private List<ScreenFadeControl> fadeControls = new List<ScreenFadeControl>();

	void Start () {
		StartCoroutine (StartSceneTransition ());
	}

	void SetFadersEnabled(bool value)
	{
		foreach (ScreenFadeControl fadeControl in fadeControls)
			fadeControl.enabled = value;
	}

	public void Transition () {
		StartCoroutine (TransitionCR ());
	}

	private IEnumerator TransitionCR () {
		yield return StartCoroutine (EndSceneTransition ());
		SceneManager.LoadSceneAsync (1);
	}

	//Fades in all the cameras to allow for a smooth transition into a scene.
	private IEnumerator StartSceneTransition () {
		// Clean up from last fade
		foreach (ScreenFadeControl fadeControl in fadeControls)
		{
			Destroy(fadeControl);
		}
		fadeControls.Clear();

		// Find all cameras and add fade material to them (initially disabled)
		Camera[] camArray = Camera.allCameras;
		camArray = camArray.Union (camArray.Where (cam => cam.name == "VR Main Camera").First ().GetComponentsInChildren<Camera> ()).ToArray ();
		foreach (Camera c in camArray) {
			var fadeControl = c.gameObject.AddComponent<ScreenFadeControl> ();
			fadeControl.fadeMaterial = fadeMaterial;
			fadeControls.Add (fadeControl);
		}
		yield return StartCoroutine(FadeIn());
	}

	//Fades in all the cameras to allow for a smooth transition out of a scene.
	private IEnumerator EndSceneTransition () {
		// Clean up from last fade
		foreach (ScreenFadeControl fadeControl in fadeControls)
		{
			Destroy(fadeControl);
		}
		fadeControls.Clear();

		foreach (Camera c in Camera.allCameras) {
			var fadeControl = c.gameObject.AddComponent<ScreenFadeControl> ();
			fadeControl.fadeMaterial = fadeMaterial;
			fadeControls.Add (fadeControl);
		}
		yield return StartCoroutine(FadeOut());
	}

	private IEnumerator FadeOut()
	{
		// Derived from OVRScreenFade
		float elapsedTime = 0.0f;
		Color color = fadeColor;
		color.a = 0.0f;
		fadeMaterial.color = color;
		while (elapsedTime < fadeTime)
		{
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
			color.a = Mathf.Clamp01(elapsedTime / fadeTime);
			fadeMaterial.color = color;
		}
	}

	private IEnumerator FadeIn()
	{
		float elapsedTime = 0.0f;
		Color color = fadeMaterial.color = fadeColor;
		while (elapsedTime < fadeTime)
		{
			yield return new WaitForEndOfFrame();
			elapsedTime += Time.deltaTime;
			color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
			fadeMaterial.color = color;
		}
		SetFadersEnabled(false);
	}
}

