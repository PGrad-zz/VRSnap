using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MoveForward : MonoBehaviour, Mover {
	public float speed;
	private Vector3 motion;
	private bool moving = false;
	private Vector3 terrainBounds;
	private Animator animator;
	private float upperZlimit;
	void Awake () {
		terrainBounds = GameObject.FindGameObjectWithTag("Level").GetComponent<Terrain> ().terrainData.size;
		terrainBounds.x /= 2;
		upperZlimit = terrainBounds.z + 100;
		animator = GetComponent <Animator> ();
		if (animator != null)
			animator.enabled = false;
	}

	void Start() {
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

	public void getMoving () {
		moving = true;
		if (animator != null)
			animator.enabled = true;
	}

	public void stopMoving () {
		moving = false;
		if (animator != null)
			animator.enabled = false;
	}
}
