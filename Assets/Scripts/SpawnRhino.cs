using UnityEngine;
using System.Collections;

public class SpawnRhino : MonoBehaviour, Mover {
	public GameObject rhino;
	public float maxPlayerSpeed = 10;
	private NavMeshAgent rhinoNvAgent;
	private bool moving = false,
				 triggered = false;
	private NavMeshAgent playerNVagent;

	// Use this for initialization
	void Start () {
		EventManager.RegisterEvent ("Move", getMoving);
		EventManager.RegisterEvent ("Stop", stopMoving);
	}

	void Update () {
		if (moving && playerNVagent.speed < maxPlayerSpeed) {
			rhinoNvAgent.speed += 0.1f;
			playerNVagent.speed += 0.4f;
		}
	}
		
	public void getMoving () {
		if (triggered)
			moving = true;
	}

	public void stopMoving () {
		moving = false;
	}

	void OnTriggerEnter (Collider other) {
		if (!triggered && other.name == "Player") {
			(rhino = (GameObject) Instantiate (rhino, transform.position + new Vector3 (0,5,0), Quaternion.identity)).transform.parent = transform;
			rhino.GetComponent<MoveToTarget> ().target = other.gameObject;
			rhinoNvAgent = rhino.GetComponent<NavMeshAgent> ();
			playerNVagent = other.gameObject.GetComponent<NavMeshAgent> ();
			triggered = true;
			BroadcastMessage ("getMoving");
		}
	}
}
