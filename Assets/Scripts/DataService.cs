using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService : Singleton<DataService> {
	static string DatabaseName = "safari.db";
	private SQLiteConnection _connection;

	void Awake () {
		#if UNITY_EDITOR
				var dbPath = string.Format(@"Assets/StreamingAssets/{0}",DatabaseName);
		#else
		        // check if file exists in Application.persistentDataPath
		        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

		        if (!File.Exists(filepath))
		        {
		            Debug.Log("Database not in Persistent path");
		            // if it doesn't ->
		            // open StreamingAssets directory and load the db ->

		#if UNITY_ANDROID 
		            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
		            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
		            // then save to Application.persistentDataPath
		            File.WriteAllBytes(filepath, loadDb.bytes);
		#elif UNITY_IOS
		                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		                // then save to Application.persistentDataPath
		                File.Copy(loadDb, filepath);
		#elif UNITY_WP8
		                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		                // then save to Application.persistentDataPath
		                File.Copy(loadDb, filepath);

		#elif UNITY_WINRT
				var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
				// then save to Application.persistentDataPath
				File.Copy(loadDb, filepath);
		#else
			var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
			// then save to Application.persistentDataPath
			File.Copy(loadDb, filepath);

		#endif

		            Debug.Log("Database written");
		        }

		        var dbPath = filepath;
		#endif
		            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
		        Debug.Log("Final PATH: " + dbPath);     

	}

	public static string[] getChildren(string root) {
		return Instance._connection.Find<Chain> (x => x.root == root).children.Split(new char[] {','} ,int.MaxValue);
	}

	public static string getGroup (string goName) {
		Groups group = Instance._connection.Find<Groups> (g => goName.Contains(g.name));
		if (group != null)
			return group.name; 
		return null;
	}

	public static bool getScore (GameObject go, out int score) {
		Objects pictureObject = Instance._connection.Find<Objects> (g => go.name.Contains(g.name));
		if (pictureObject != null) {
			score = pictureObject.score;
			return true;
		} else {
			score = 0;
			return false;
		}
	}

	public class Chain {
		public string root { get; set; }
		public string children { get; set; }
	}

	public class Groups {
		public string name { get; set; }
	}

	public class Objects {
		public string name { get; set; }
		public int score { get; set; } 
	}
}
