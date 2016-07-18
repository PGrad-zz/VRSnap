using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour, Mover {
	public GameObject target;
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
		terrainBounds = GameObject.FindGameObjectWithTag ("Level").GetComponent<Terrain> ().terrainData.size;
		terrainBounds.x /= 2;
		getMoving ();
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
