using UnityEngine;
using System.Collections;

public class WorldBounds : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameManager.Instance.player;
	}
	
	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			Object.Destroy(player, 1);
		}
	}


	// Update is called once per frame
	void Update () {
	
	}
}
