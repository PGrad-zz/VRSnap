using UnityEngine;
using System.Collections;

public class MoveToTarget : MonoBehaviour, Mover {
	public GameObject target;
	private bool moving = false;
	private NavMeshAgent nvAgent;
	private bool stopped = false;
	private Vector3 terrainBounds;
	private Animator animator;

	// Use this for initialization
	void Awake () {
		nvAgent = GetComponent<NavMeshAgent> ();
		animator = GetComponent <Animator> ();
		if (animator != null)
			animator.enabled = false;
	}

	void Start () {
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

	public void getMoving () {
		if(stopped)
			nvAgent.Resume ();
		moving = true;
		if (animator != null)
			animator.enabled = true;
	}

	public void stopMoving () {
		nvAgent.Stop ();
		moving = false;
		stopped = true;
		if (animator != null)
			animator.enabled = false;
	}
}
