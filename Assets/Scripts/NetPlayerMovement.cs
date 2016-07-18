using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetPlayerMovement : MonoBehaviour {
	private float yaw = 0.0f;
	private GameObject playerHead;
	private Transform playerHeadTransform;

	void Start () {
		playerHead = GameObject.Find ("GvrHead");
		playerHeadTransform = playerHead.transform;
	}

	void Update () {
		playerHeadTransform.position = transform.position;
		yaw += Input.GetAxis ("Mouse X") * 2;
		transform.eulerAngles = new Vector3 (0.0f, yaw, 0.0f);
		transform.Translate ((Vector3.forward * Input.GetAxis ("Vertical") + Vector3.right * Input.GetAxis ("Horizontal")) * Time.deltaTime * 10);
	}
}
