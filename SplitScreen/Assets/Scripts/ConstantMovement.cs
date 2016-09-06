using UnityEngine;
using System.Collections;

public class ConstantMovement : MonoBehaviour {

	public float x_speed;
	public float y_speed;
	public float resetDistance;
	private int reverse;


	private Vector3 initialPos;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
		reverse = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position += new Vector3(reverse*x_speed*Time.deltaTime, y_speed*Time.deltaTime, 0);
		if ((transform.position - initialPos).magnitude > resetDistance) {
			initialPos = transform.position;
			reverse *= -1;
		}
	}
}
