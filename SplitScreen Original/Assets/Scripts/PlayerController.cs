using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

	public static bool shifted = false;

	// [HideInInspector] 
	private bool attemptingJump = false;

	public float maxSpeed = 3f;
	public float speedInc = 0.5f;
	public float jumpForce = 120f;
	public float jumpRatio = 0.65f; //used for holding down to jump higher
	public Transform groundCheckL;
	public Transform groundCheckR;
	public LayerMask groundMask;

	public bool grounded = false;
	private bool pounded = false;
	private Collider2D bottom;
	private Rigidbody2D rb2d; 


	// Use this for initialization
	void Awake () 
	{
		shifted = false;

		rb2d = GetComponent<Rigidbody2D> ();
		bottom = GetComponent<EdgeCollider2D> ();
		grounded = false;
		pounded = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if on ground
		bool oldground = grounded;
		grounded = bottom.IsTouchingLayers(groundMask);
		if (grounded) {
			pounded = false;
			attemptingJump = false;
			if (!oldground) {
				rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
			}
		}
	}
		
	void FixedUpdate() 
	{
		//find out which direction the player is attempting to move
		float horizontalInput = Input.GetAxisRaw ("Horizontal");

		// V key for fun and profit
		if (Input.GetKeyDown(KeyCode.V)) {
			Debug.Log("CHEATING :D");
			rb2d.velocity = new Vector2(rb2d.velocity.x + (6*Math.Sign(horizontalInput)), rb2d.velocity.y);
		}

		//stop walking quickly rather than skid stop
		if (horizontalInput != Mathf.Sign(rb2d.velocity.x) && Mathf.Abs(rb2d.velocity.x) <= maxSpeed) {
			if (rb2d.velocity.x != 0) {
				rb2d.velocity = new Vector2 (0f, rb2d.velocity.y);
			}
		}

		// force based movement system if input detected
		if (horizontalInput != 0) {
			// increase speed to maximum
			if (Mathf.Abs(rb2d.velocity.x) <= maxSpeed) {
				rb2d.AddForce(new Vector2 (5 * Mathf.Sign(horizontalInput) * (maxSpeed - Mathf.Abs(rb2d.velocity.x)), 0f));			
			}
			// decrease speed to maximum 
			else if (Mathf.Sign(horizontalInput) != Mathf.Sign(rb2d.velocity.x)) {
				rb2d.AddForce(new Vector2 (3 * Mathf.Sign(horizontalInput) * (Mathf.Abs(rb2d.velocity.x) - maxSpeed + 1), 0f));
			}
		}

		Jump ();
	}
		
	void Jump()
	{
		// initial jump
		if(Input.GetAxis("Vertical") > 0 && grounded && attemptingJump == false){
			rb2d.velocity = new Vector2(rb2d.velocity.x, 6);
			attemptingJump = true;
		}
		// ground pound
		else if (Input.GetKeyDown("down") && !grounded && !pounded) {
			attemptingJump = false;
			pounded = true;
			Debug.Log("groundpounding");
			rb2d.velocity = new Vector2(rb2d.velocity.x, 0);		
			rb2d.AddForce (new Vector2 (0f, -400));
		}
		
		// decay jump
		if (rb2d.velocity.y > 0 && !grounded && Input.GetAxis("Vertical") <= 0){
			attemptingJump = false;
			rb2d.AddForce (new Vector2 (0f, rb2d.velocity.y * rb2d.velocity.y * -7.5f));
		}
	}


}
