using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

 	private static LevelManager instance;

	public GameObject[] rooms;
	public GameObject player;
	public GameObject powerSource;
	public bool destroyed = false;

    public static LevelManager Instance
    {
        get
        {
            if (LevelManager.instance == null)
            {
                instance = GameObject.FindObjectOfType<LevelManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return LevelManager.instance;
        }
    }

	void Awake() {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<LevelManager>();
            DontDestroyOnLoad(instance.gameObject);
        }
        else if (instance != this)
        {
        	Debug.Log("destroying the thing");
        	destroyed = instance.destroyed;
            Destroy(instance.gameObject);
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

		if (destroyed)
			Destroy(powerSource);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject room in rooms) {
			if (Mathf.Abs(player.transform.position.y - room.transform.position.y) < 10)
				room.SetActive(true);
			else 
				room.SetActive(false);
		}
	}
}
