using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MoveForward : MonoBehaviour, IMover {
	public float speed;
	private Vector3 motion;
	private bool moving = false,
				 soundPaused = false;
	private Vector3 terrainBounds;
	private Animator animator;
	private AudioSource sound;
	private float upperZlimit;

	void Awake () {
		terrainBounds = GameObject.FindGameObjectWithTag("Level").GetComponent<Terrain> ().terrainData.size;
		terrainBounds.x /= 2;
		upperZlimit = terrainBounds.z + 100;
		animator = GetComponent <Animator> ();
		sound = GetComponent <AudioSource> ();
		if (animator != null)
			animator.enabled = false;
	}

	void Start() {
		EventManager.RegisterEvent ("Start", startMoving);
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
	}

	void Update () {
		if (moving) {
			transform.Translate (Vector3.forward * speed);
			if (Mathf.Abs (transform.position.x) > terrainBounds.x || transform.position.z < 0 || transform.position.z > upperZlimit)
				Destroy (gameObject);
		}
	}

	public void startMoving () {
		moving = true;
		if (animator != null)
			animator.enabled = true;
		if (sound != null)
			sound.Play ();
	}

	public void getMoving () {
		moving = true;
		if (animator != null)
			animator.enabled = true;
		if (sound != null && soundPaused) {
			sound.Play ();
			soundPaused = false;
		}
	}

	public void stopMoving () {
		moving = false;
		if (animator != null)
			animator.enabled = false;
		if (sound != null && sound.isPlaying) {
			sound.Pause ();
			soundPaused = true;
		}
	}
}
