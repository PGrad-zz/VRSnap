using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : MonoBehaviour {
	NetworkClient myClient;
	NetworkManager manager;

	void Start () {
		manager = GetComponent<NetworkManager> ();
		if (manager != null) {
			if (Application.platform != RuntimePlatform.OSXEditor)
				SetupClient ();
			else {
				SetupServer ();
				SetupLocalClient ();
			}
		}
	}

	//Create a server and listen on a port
	public void SetupServer() {
		manager.StartServer ();
	}

	// Create a client and connect to the server port
	public void SetupClient()
	{
		myClient = manager.StartClient ();
		myClient.RegisterHandler(MsgType.Connect, OnConnected);     
		ClientScene.Ready (myClient.connection);
	}

	// Create a local client and connect to the local server
	public void SetupLocalClient()
	{
		myClient = ClientScene.ConnectLocalServer();
		ClientScene.Ready (myClient.connection);
		ClientScene.AddPlayer (0);
	}

	// client function
	public void OnConnected(NetworkMessage netMsg)
	{
		Debug.Log ("Connected");
	}
}
