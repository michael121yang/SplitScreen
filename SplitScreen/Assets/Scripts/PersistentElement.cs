using UnityEngine;
using System.Collections;

public class PersistentElement : MonoBehaviour {

	 private static GameObject instance;

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);

        if (instance == null) {
        	instance = gameObject;
		} else {
	        Debug.Log(instance);
            Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
