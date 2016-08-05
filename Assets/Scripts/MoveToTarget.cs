using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour, IMover {
	public GameObject target;
	private bool moving = false,
				 stopped = false,
				 soundPaused = false;
	private NavMeshAgent nvAgent;
	private Vector3 terrainBounds;
	private Animator animator;
	private AudioSource sound;

	// Use this for initialization
	void Awake () {
		nvAgent = GetComponent<NavMeshAgent> ();
		animator = GetComponent <Animator> ();
		sound = GetComponent <AudioSource> ();
		if (animator != null)
			animator.enabled = false;
	}

	void Start () {
		EventManager.RegisterEvent ("Start", startMoving);
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
		terrainBounds = GameObject.FindGameObjectWithTag ("Level").GetComponent<Terrain> ().terrainData.size;
		terrainBounds.x /= 2;
	}

	// Update is called once per frame
	void Update () {
		if (moving) {
			if (target == null || Mathf.Abs (transform.position.x) > terrainBounds.x || transform.position.z < 0 || transform.position.z > terrainBounds.z)
				Destroy (gameObject);
			if (target != null)
				nvAgent.SetDestination (target.transform.position);
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
		if(stopped)
			nvAgent.Resume ();
		moving = true;
		if (animator != null)
			animator.enabled = true;
		if (sound != null && soundPaused) {
			sound.Play ();
			soundPaused = false;
		}
	}

	public void stopMoving () {
		nvAgent.Stop ();
		moving = false;
		stopped = true;
		if (animator != null)
			animator.enabled = false;
		if (sound != null && sound.isPlaying) {
			sound.Pause ();
			soundPaused = true;
		}
	}
}
