using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour, IMover {
	public GameObject target;
	public float maxSpeed;
	private NavMeshAgent nvAgent;
	private bool moving = false;
	private Vector3 terrainBounds;

	// Use this for initialization
	void Start () {
		nvAgent = GetComponent<NavMeshAgent> ();
		EventManager.RegisterEvent ("StartPlayerMove", startMoving);
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
	}

	void Update () {
		if (moving)
			nvAgent.SetDestination (target.transform.position);
	}

	public void startMoving () {
		moving = true;
		StartCoroutine (rampUpSpeed ());
	}

	public IEnumerator rampUpSpeed () {
		while (nvAgent.speed < maxSpeed) {
			yield return new WaitForSeconds (0.1f);
			nvAgent.speed += 0.1f;
		}
	}

	public void getMoving () {
		moving = true;
		nvAgent.Resume ();	
	}

	public void stopMoving () {
		nvAgent.Stop ();
		moving = false;
	}
}
