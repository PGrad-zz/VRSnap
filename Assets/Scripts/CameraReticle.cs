/**Adapted from GvrReticle from the Google Cardboard SDK**/

// Copyright 2015 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// Draws a circular reticle in front of any object that the user gazes at.
/// The circle dilates if the object is clickable.
[AddComponentMenu("Scripts/CameraReticle")]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(CameraShot))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(GazeInputModule))]
public class CameraReticle : MonoBehaviour, IGvrGazePointer {
	public static Dictionary<GameObject,bool> mapGOtoFacing;
	public static bool shotsEnabled = false;
	//Reference to CameraShot sibling component
	private CameraShot cameraShot;

	/// Number of segments making the reticle circle.
	private int reticleSegments = 4;

	/// Growth speed multiplier for the reticle/
	public float reticleGrowthSpeed = 8.0f;

	// Private members
	private Material materialComp,
					 focusComp;
	private GameObject targetObj,
					   headCanvas,
					   pointsPanel,
					   pictureFrame;
	private AudioSource snapshot,
						focusSound;
	private Transform headTransform;
	private Vector3 targetLocalPosition;

	// Current inner angle of the reticle (in degrees).
	private float reticleInnerAngle = 0.0f;
	// Current outer angle of the reticle (in degrees).
	private float reticleOuterAngle = 0.5f;
	// Current distance of the reticle (in meters).
	private float reticleDistanceInMeters = 10.0f;

	// Minimum inner angle of the reticle (in degrees).
	private const float kReticleMinInnerAngle = 0.0f;
	// Minimum outer angle of the reticle (in degrees).
	private const float kReticleMinOuterAngle = 0.5f;
	// Maximum inner angle of the reticle (in degrees).
	private const float kReticleMaxInnerAngle = 15f;
	// Maximum outer angle of the reticle (in degrees).
	private const float kReticleMaxOuterAngle = 15.5f;
	// Angle at which to expand the reticle when intersecting with an object when not triggered
	// (in degrees).
	private float kReticleGrowthAngle = 1.5f;

	// Minimum distance of the reticle (in meters).
	private const float kReticleDistanceMin = 0.45f;
	// Maximum distance of the reticle when there is no trigger (in meters).
	private float kReticleDistanceMax = 10.0f;

	// Current inner and outer diameters of the reticle,
	// before distance multiplication.
	private float reticleInnerDiameter = 0.0f;
	private float reticleOuterDiameter = 0.0f;

	private int colliderMultiplier;
	private bool isInteractiveAndIsNotNull = false;
	private bool zoomIn = false;
	private float fieldOfView;
	private bool canZoom = false;

	void Start () {
		cameraShot = gameObject.GetComponentInParent<CameraShot> ();

		CreateReticleVertices();

		materialComp = gameObject.GetComponent<Renderer>().material;

		focusComp = gameObject.GetComponent<MeshRenderer> ().materials [1];

		snapshot = gameObject.GetComponents<AudioSource> () [0]; 

		focusSound = gameObject.GetComponents<AudioSource> () [1];

		headTransform = GameObject.Find ("PlayerHead").GetComponent<GvrHead> ().transform;

		mapGOtoFacing = new Dictionary<GameObject,bool> ();

		headCanvas = GameObject.Find("HeadCanvas");

		if (headCanvas != null) {
			pictureFrame = headCanvas.transform.GetChild (0).gameObject;
			pointsPanel = headCanvas.transform.GetChild (1).gameObject;
		}
	}

	void OnEnable() {
		GazeInputModule.gazePointer = this;
	}

	void OnDisable() {
		if (GazeInputModule.gazePointer == this) {
			GazeInputModule.gazePointer = null;
		}
	}

	void Update() {
		UpdateDiameters();
	}

	/// This is called when the 'BaseInputModule' system should be enabled.
	public void OnGazeEnabled() {

	}

	/// This is called when the 'BaseInputModule' system should be disabled.
	public void OnGazeDisabled() {

	}

	/// Called when the user is looking on a valid GameObject. This can be a 3D
	/// or UI element.
	///
	/// The camera is the event camera, the target is the object
	/// the user is looking at, and the intersectionPosition is the intersection
	/// point of the ray sent from the camera on the object.
	public void OnGazeStart(Camera camera, GameObject targetObject, Vector3 intersectionPosition,
		bool isInteractive) {
		isInteractiveAndIsNotNull = isInteractive && targetObject != null;
		targetObj = targetObject;
		/**if (isInteractiveAndIsNotNull) {
			Collider targetCollider = targetObject.GetComponent<Collider> ();
			colliderMultiplier = (int) (targetCollider.bounds.size.y * targetCollider.transform.localScale.y);
		}*/
		SetGazeTarget(intersectionPosition, EventManager.isPhotogenic (targetObj));
	}

	/// Called every frame the user is still looking at a valid GameObject. This
	/// can be a 3D or UI element.
	///
	/// The camera is the event camera, the target is the object the user is
	/// looking at, and the intersectionPosition is the intersection point of the
	/// ray sent from the camera on the object.
	public void OnGazeStay(Camera camera, GameObject targetObject, Vector3 intersectionPosition,
		bool isInteractive) {
		SetGazeTarget(intersectionPosition, isInteractiveAndIsNotNull);
	}

	/// Called when the user's look no longer intersects an object previously
	/// intersected with a ray projected from the camera.
	/// This is also called just before **OnGazeDisabled** and may have have any of
	/// the values set as **null**.
	///
	/// The camera is the event camera and the target is the object the user
	/// previously looked at.
	public void OnGazeExit(Camera camera, GameObject targetObject) {
		reticleDistanceInMeters = kReticleDistanceMax;
		reticleInnerAngle = kReticleMinInnerAngle;
		reticleOuterAngle = kReticleMinOuterAngle;
		isInteractiveAndIsNotNull = false;
		targetObj = null;
	}

	/// Called when a trigger event is initiated. This is practically when
	/// the user begins pressing the trigger.
	public void OnGazeTriggerStart(Camera camera) {
		// Put your reticle trigger start logic here :)
		//Check to make sure targetObj still exists
		if(EventManager.isPhotogenic (targetObj)) {
			if (targetObj.name == "Gelios_high") {
				canZoom = true;
				Debug.Log ("zoom!");
			} else {
				kReticleGrowthAngle = kReticleGrowthAngle * 10f/*** colliderMultiplier*/;
				focusSound.Play ();
				if (canZoom) {
					zoomIn = true;
					StartCoroutine (zoom (camera.GetComponentsInChildren<Camera> ()));
				}
			}
		}
	}

	public IEnumerator zoom(Camera[] lrCamera) {
		while (zoomIn) {
			foreach (Camera camera in lrCamera) 
				camera.fieldOfView -= 0.6f;
			yield return null;
		}
		foreach (Camera camera in lrCamera)
			camera.fieldOfView = 75f;
	}

	/// Called when a trigger event is finished. This is practically when
	/// the user releases the trigger.
	public void OnGazeTriggerEnd(Camera camera) {
		// Put your reticle trigger end logic here :)
		//Check to make sure targetObj still exists
		//isInteractiveAndIsNotNull &= targetObj != null;
		if (shotsEnabled && EventManager.isPhotogenic(targetObj) && materialComp.GetFloat ("_InnerDiameter") > 0.3f) {
			ClearScreen ();
			snapshot.Play ();
			cameraShot.TakeCameraShot (materialComp.GetFloat ("_OuterDiameter") - materialComp.GetFloat ("_InnerDiameter"));
			ScoreManager.changeScore (scanWithinReticle ());
		} 
		StartCoroutine (resizeDown ());
	}

	private void ClearScreen () {
		EventManager.TriggerEvent ("Pause");
		if (headCanvas != null) {
			headCanvas.SetActive (false);
			pointsPanel.SetActive (false);
			pictureFrame.SetActive (false);
		}
	}

	private void RestoreScreen () {
		if (headCanvas != null) {
			headCanvas.SetActive (true);
			pictureFrame.SetActive (true);
		}
		EventManager.TriggerEvent ("Resume");
	}

	private IEnumerator resizeDown() {
		zoomIn = false;
		shotsEnabled = false;
		yield return new WaitForSeconds (0.25f);
		kReticleGrowthAngle = 1.5f;
		RestoreScreen ();
		shotsEnabled = true;
	}

	public void GetPointerRadius(out float innerRadius, out float outerRadius) {
		float min_inner_angle_radians = Mathf.Deg2Rad * kReticleMinInnerAngle;
		float max_inner_angle_radians = Mathf.Deg2Rad * (kReticleMinInnerAngle + kReticleGrowthAngle);

		innerRadius = 2.0f * Mathf.Tan(min_inner_angle_radians);
		outerRadius = 2.0f * Mathf.Tan(max_inner_angle_radians);
	}

	private void CreateReticleVertices() {
		Mesh mesh = new Mesh();
		gameObject.AddComponent<MeshFilter>();
		GetComponent<MeshFilter>().mesh = mesh;

		int segments_count = reticleSegments;
		int vertex_count = (segments_count+1)*2;

		#region Vertices

		Vector3[] vertices = new Vector3[vertex_count];

		const float kTwoPi = Mathf.PI * 2.0f;
		int vi = 0;
		for (int si = 0; si <= segments_count; ++si) {
			// Add two vertices for every circle segment: one at the beginning of the
			// prism, and one at the end of the prism.
			float angle = ((float)si / (float)(segments_count) + 0.125f) * kTwoPi;

			float x = Mathf.Sin(angle);
			float y = Mathf.Cos(angle);

			vertices[vi++] = new Vector3(x, y, 0.0f); // Outer vertex.
			vertices[vi++] = new Vector3(x, y, 1.0f); // Inner vertex.
		}
		#endregion

		#region Triangles
		int indices_count = (segments_count+1)*3*2;
		int[] indices = new int[indices_count];

		int vert = 0;
		int idx = 0;
		for (int si = 0; si < segments_count; ++si) {
			indices[idx++] = vert+1;
			indices[idx++] = vert;
			indices[idx++] = vert+2;

			indices[idx++] = vert+1;
			indices[idx++] = vert+2;
			indices[idx++] = vert+3;

			vert += 2;
		}
		#endregion

		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateBounds();
		mesh.Optimize();
	}

	private void UpdateDiameters() {
		reticleDistanceInMeters =
			Mathf.Clamp(reticleDistanceInMeters, kReticleDistanceMin, kReticleDistanceMax);

		if (reticleInnerAngle < kReticleMinInnerAngle) {
			reticleInnerAngle = kReticleMinInnerAngle;
		}

		if (reticleOuterAngle < kReticleMinOuterAngle) {
			reticleOuterAngle = kReticleMinOuterAngle;
		}

		float inner_half_angle_radians = Mathf.Deg2Rad * reticleInnerAngle * 0.5f;
		float outer_half_angle_radians = Mathf.Deg2Rad * reticleOuterAngle * 0.5f;

		float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
		float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

		reticleInnerDiameter =
			Mathf.Lerp(reticleInnerDiameter, inner_diameter, Time.deltaTime * reticleGrowthSpeed);
		reticleOuterDiameter =
			Mathf.Lerp(reticleOuterDiameter, outer_diameter, Time.deltaTime * reticleGrowthSpeed);


		focusComp.SetFloat("_OuterDiameter", reticleInnerDiameter * reticleDistanceInMeters);
		focusComp.SetFloat("_DistanceInMeters", reticleDistanceInMeters);
		materialComp.SetFloat("_InnerDiameter", reticleInnerDiameter * reticleDistanceInMeters);
		materialComp.SetFloat("_OuterDiameter", reticleOuterDiameter * reticleDistanceInMeters);
		materialComp.SetFloat("_DistanceInMeters", reticleDistanceInMeters);
	}

	private void SetGazeTarget(Vector3 target, bool interactive) {
		targetLocalPosition = transform.InverseTransformPoint (target);

		reticleDistanceInMeters =
			Mathf.Clamp (targetLocalPosition.z, kReticleDistanceMin, kReticleDistanceMax);
		
		if (interactive) {
			reticleInnerAngle = kReticleMinInnerAngle + kReticleGrowthAngle;
			reticleOuterAngle = kReticleMinOuterAngle + kReticleGrowthAngle;
			if (reticleInnerAngle > kReticleMaxInnerAngle) {
				reticleInnerAngle = kReticleMaxInnerAngle;
			}

			if (reticleOuterAngle > kReticleMaxOuterAngle) {
				reticleOuterAngle = kReticleMaxOuterAngle;
			}
		} else {
			reticleInnerAngle = kReticleMinInnerAngle;
			reticleOuterAngle = kReticleMinOuterAngle;
		}
	}

	private bool isFacing(GameObject targetObject) {
		return Vector3.Angle (targetObject.transform.forward, transform.position - targetObject.transform.position) < 90;
	}

	private bool isFullShot(int hits) {
		const float maxHits = 1800f;
		return (int) ((float)hits / maxHits * 100f) > 50;
	}

	public HashSet<GameObject> scanWithinReticle() {
		mapGOtoFacing.Clear ();
		HashSet<GameObject> uniqueHits = new HashSet<GameObject> ();
		RaycastHit hit = new RaycastHit ();
		GameObject hit_go;
		Vector3 origin = headTransform.position;
		float inverseResolution = 10f;
		Vector3 forward = headTransform.forward,
				up = headTransform.up,
				direction = forward;
		int zSteps = Mathf.FloorToInt (360f / inverseResolution),
			ySteps = 50;
		Quaternion yRotation = Quaternion.Euler(up * (reticleInnerAngle / 70)),
		zRotation = Quaternion.Euler(forward * inverseResolution);
		for(int z = 0; z < ySteps; z++) {
			direction = yRotation * direction;
			for(int x = 0; x < zSteps; x++) {
				direction = zRotation * direction;
				Physics.Raycast (origin, direction, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
				if (hit.transform != null) { 
					hit_go = hit.transform.gameObject;
					if (!uniqueHits.Contains (hit_go) && hit_go.GetComponent<EventTrigger> () != null) {
						mapGOtoFacing.Add (hit_go, isFacing(hit_go));
						uniqueHits.Add (hit_go);
					}
				}
			}
		}
			
		return uniqueHits;
	}
}