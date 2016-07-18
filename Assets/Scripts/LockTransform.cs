using UnityEngine;
using System.Collections;

public class LockTransform : MonoBehaviour {
	private Transform parent;
	private float initY;
	void Start () {
		parent = transform.parent;
		initY = transform.position.y;
	}
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x, initY, transform.position.z);
		transform.eulerAngles = new Vector3 (0,parent.eulerAngles.y,0);
	}
}
