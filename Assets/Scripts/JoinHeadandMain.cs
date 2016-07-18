using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class JoinHeadandMain : NetworkBehaviour {
	string[] children;
	GameObject childGO;
	// Update is called once per frame
	void Start () {
		NetworkTransformChild[] ntc = GetComponents <NetworkTransformChild> ();
		children = new string[] {"GvrMain", "GvrHead"};
		for (int child = 0; child < 2; child++) {
			childGO = GameObject.Find (children [child]);
			childGO.transform.parent = transform;
			ntc [child].target = childGO.transform;
			ntc [child].enabled = true;
		}
		GameObject.Find ("VR Main Camera").GetComponent<Camera> ().enabled = true;
	}
	/**void Start () {
		children = new string[] {"GvrMain", "PlayerHead"};
	}
	void Update () {
		if (Network.peerType == NetworkPeerType.Disconnected && notJoined) {
			for (int child = 0; child < 2; child++) {
				childGO = GameObject.Find (children [child]);
				childGO.transform.parent = transform;
				gameObject.AddComponent<NetworkTransformChild> ().target = childGO.transform;
				gameObject.GetComponents<NetworkTransformChild> () [child].enabled = true;
			}
			GameObject.Find ("VR Main Camera").GetComponent<Camera> ().enabled = true;
			notJoined = false;
		}
	}*/
}
