using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public GameObject effectee;
	public Vector2 push_direction;

	private Collider2D top;

	// Use this for initialization
	void Start () {
		top = GetComponent<EdgeCollider2D>();
	
	}
	
	void OnCollisionEnter2D () {
		GetComponent<Rigidbody2D>().velocity = push_direction;
		effectee.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	}
}
