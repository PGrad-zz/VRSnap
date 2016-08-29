//Adapted from here: https://unity3d.com/learn/tutorials/topics/scripting/events-creating-simple-messaging-system
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class EventManager : Singleton <EventManager> {
	private Dictionary <GameObject, UnityEvent> objToEvent;
	private Dictionary <string, UnityEvent> strToEvent;
	private HashSet <GameObject> objsBeenInvoked;

	void Awake () {
		objToEvent = new Dictionary<GameObject, UnityEvent> ();
		strToEvent = new Dictionary<string, UnityEvent> ();
		objsBeenInvoked = new HashSet<GameObject> ();
	}

	public static void RegisterGameObject (GameObject obj, UnityAction listener) {
		if (isPhotogenic (obj)) {
			UnityEvent thisEvent = null;
			if (Instance.objToEvent.TryGetValue (obj, out thisEvent))
				thisEvent.AddListener (listener);
			else {
				thisEvent = new UnityEvent ();
				thisEvent.AddListener (listener);
				Instance.objToEvent.Add (obj, thisEvent);
			}
		}
	}

	public static void RegisterEvent (string eventName, UnityAction listener) {
		UnityEvent thisEvent = null;
		if (Instance.strToEvent.TryGetValue (eventName, out thisEvent)) 
			thisEvent.AddListener (listener);
		else {
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			Instance.strToEvent.Add (eventName, thisEvent);
		}
	}

	public static void DeregisterGameobject (GameObject obj, UnityAction listener) {
		if (Instance == null) return;
		UnityEvent thisEvent = null;
		if (Instance.objToEvent.TryGetValue (obj, out thisEvent))
			thisEvent.RemoveListener (listener);
	}

	public static void DeregisterEvent (string eventName, UnityAction listener) {
		if (Instance == null) return;
		UnityEvent thisEvent = null;
		if (Instance.strToEvent.TryGetValue (eventName, out thisEvent))
			thisEvent.RemoveListener (listener);
	}

	public static void TriggerGameobject (GameObject obj) {
		UnityEvent thisEvent = null;
		if (Instance.objToEvent.TryGetValue (obj, out thisEvent)) 
			thisEvent.Invoke ();
	}

	public static void TriggerEvent (string eventName) {
		UnityEvent thisEvent = null;
		if (Instance.strToEvent.TryGetValue (eventName, out thisEvent))
			thisEvent.Invoke ();
	}

	public static bool HasBeenInvoked (GameObject go) {
		return Instance.objsBeenInvoked.Contains (go);
	}

	public static void NowInvoked (GameObject go) {
		Instance.objsBeenInvoked.Add (go);
	}

	public static bool isPhotogenic (GameObject go) {
		if (go == null)
			return false;
		Collider collider;
		collider = go.GetComponent<Collider> ();
		return collider != null && !collider.isTrigger && go.GetComponent<EventTrigger> () != null;
	}
}
