using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public static bool shifted = false;

	// [HideInInspector] 
	public float maxSpeed = 3f;
	public float speedInc = 0.5f;
	public float jumpForce = 120f;
	public float jumpRatio = 0.65f; //used for holding down to jump higher
	public LayerMask groundMask;
	public GameObject Device;
	public Camera Camera;

	private int frameCounter = 0;

	private bool draw = false;
	private int  reverseCut = 1;

	public bool grounded = false;
	private bool pounded = false;
	private Collider2D bottom;
	private Rigidbody2D rb2d; 

	// Use this for initialization
	void Awake () 
	{
		shifted = false;
		GameManager.Instance.player = gameObject;

		rb2d = GetComponent<Rigidbody2D> ();
		bottom = GetComponent<EdgeCollider2D> ();
		grounded = false;
		pounded = false;
	}
	
	void Start () 
	{
		transform.position = GameManager.Instance.playerCheckpoint;
	}

	// Update is called once per frame
	void Update () 
	{	
		// Debug.Log(grounded);
		grounded = false;
		// check if on ground
		grounded = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, .1f), 0, Vector2.down, transform.localScale.y*.5f, groundMask);
		if (grounded) {
			pounded = false;
		}
	}


	void LateUpdate () {
		if (!GameManager.Instance.cut && !GameManager.Instance.transitioning) {
			if (GameManager.Instance.canCut) {
				if (Input.GetMouseButtonDown(0)) {
					draw = true;
					reverseCut = 1;
				} else if (Input.GetMouseButtonDown(1)) {
					draw = true;
					reverseCut = -1;
				}
			}
			// Device.SetActive(true);
			Vector2 mousepos = new Vector2(Camera.ScreenToWorldPoint(Input.mousePosition).x, Camera.ScreenToWorldPoint(Input.mousePosition).y);

			Vector2 off2D = mousepos - new Vector2(transform.position.x, transform.position.y);
			off2D.Normalize();
			Vector3 offset = new Vector3(off2D.x, off2D.y, 0);

			if (Device != null)
				Device.transform.position = transform.position + offset * .6f * frameCounter / 10 + new Vector3(0f, 0f, -.5f);
			if (draw) {
				if (frameCounter < 10) frameCounter++;
			}
			else { 
				frameCounter = 0;
			}
			if ((Input.GetMouseButtonUp(0) && reverseCut == 1 || Input.GetMouseButtonUp(1) && reverseCut == -1) && GameManager.Instance.canCut) {
				draw = false;
			}
		} else {
			// Device.SetActive(false);
			draw = false;
			frameCounter = 0;
		}	
	}

	void FixedUpdate() 
	{
		//find out which direction the player is attempting to move
		float horizontalInput = Input.GetAxisRaw ("Horizontal");

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
				rb2d.AddForce(new Vector2 (3 * Mathf.Sign(horizontalInput) * (maxSpeed - Mathf.Abs(rb2d.velocity.x)), 0f));			
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
		if(Input.GetAxisRaw("Vertical") > 0 && grounded){
			Debug.Log("jump");
			rb2d.velocity = new Vector2(rb2d.velocity.x, 6);
		}
		// ground pound
		else if (Input.GetKeyDown("down") && !grounded && !pounded) {
			pounded = true;
			rb2d.velocity = new Vector2(rb2d.velocity.x, 0);		
			rb2d.AddForce (new Vector2 (0f, -400));
		}
		
		// decay jump
		if (rb2d.velocity.y > 0 && !grounded && Input.GetAxis("Vertical") <= 0){
			rb2d.AddForce (new Vector2 (0f, rb2d.velocity.y * rb2d.velocity.y * -7.5f));
		}
	}

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		foreach (GameObject c in sliceInfo.ChildObjects) {
			c.tag = "Untagged";
		}
		Time.timeScale = .1f;
		Destroy(gameObject, 1);
	}

	void OnDestroy() {
        GameManager.Instance.cut = false;
        GameManager.Instance.transitioning = false;
        GameManager.Instance.canCut = true;
        GameManager.Instance.canRecall = true; 
        Debug.Log("loading");
		Application.LoadLevel(0);
	}
}
