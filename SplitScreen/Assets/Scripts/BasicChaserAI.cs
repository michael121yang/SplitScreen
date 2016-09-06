using UnityEngine;
using System.Collections;

public class BasicChaserAI : MonoBehaviour {

	private GameObject player;
	private Rigidbody2D rb2d;
	public bool detected;
	public bool grounded;
	public bool active;

	public LayerMask mask;
	public LayerMask groundMask;
	public float detectDistance;
	public float patrolSpeed;
	public float chaseSpeed;

	public int panic = 1;

	private float width;
	private float height;

	// Use this for initialization
	void Start () {
		GetComponent<Collider2D>().enabled = false;
		rb2d = GetComponent<Rigidbody2D> ();
		rb2d.isKinematic = true;
		player = GameManager.Instance.player;		
		detected = false;
		active = false;

		width = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
		height = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
	}
	
	void OnBecameVisible() {
		GetComponent<Collider2D>().enabled = true;
		rb2d.isKinematic = false;
		active = true;
	}

	void OnBecameInvisible() {
		GetComponent<Collider2D>().enabled = false;
		rb2d.isKinematic = true;
		active = false;
	}

	void FixedUpdate () {
		if (!active) {
			if (rb2d.velocity.magnitude < .1 && rb2d.velocity.magnitude > 0) {
				rb2d.velocity = Vector2.zero;
			} else if (rb2d.velocity.magnitude != 0) {
				rb2d.velocity *= .9f;
			} 
		}
	}

	// Update is called once per frame
	void Update () {
		if (active) {
			bool groundAhead = Physics2D.Raycast((Vector2)transform.position + new Vector2(width*Mathf.Sign(transform.localScale.x)*.6f, 0), new Vector2(0, -1), height*1.5f, groundMask);
			bool obstacleAhead = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, transform.localScale.y * .5f), new Vector2(Mathf.Sign(transform.localScale.x), 0),
													width, groundMask);

			RaycastHit2D groundedHit = Physics2D.BoxCast(transform.position, new Vector2(width, .1f), 
									 					 0, Vector2.down, height*.5f, groundMask);

			grounded = groundedHit;

			// Debug.Log((!groundAhead || obstacleAhead));

			if (grounded && !detected) rb2d.gravityScale = 5;

			if ((!groundAhead || obstacleAhead) && grounded) {
				transform.localScale = new Vector3(-1*transform.localScale.x, transform.localScale.y, transform.localScale.z);
				detected = false;
			}

			if(detected) {
				rb2d.velocity = new Vector2(chaseSpeed * Mathf.Sign(transform.localScale.x), rb2d.velocity.y);
			} else {
				rb2d.velocity = new Vector2(patrolSpeed * Mathf.Sign(transform.localScale.x), rb2d.velocity.y);
			}

			if (grounded) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), detectDistance, mask);
				Collider2D prox = Physics2D.OverlapCircle(transform.position, 1, 1 << LayerMask.NameToLayer("Player"));

				bool found = prox || (hit && hit.collider.gameObject.tag == "Player");

				if (found) {
					if (!detected) {
						rb2d.gravityScale = 1;
						rb2d.velocity = new Vector2(0, 3);
						detected = true;
						transform.localScale = new Vector3(panic*Mathf.Abs(transform.localScale.x)*Mathf.Sign(player.transform.position.x - transform.position.x), 
														   transform.localScale.y, transform.localScale.z);
					}
				}	
			} else if (detected) {
				rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			}
		}
	} 

	void OnCollisionEnter2D(Collision2D col) {
		if (col.collider.gameObject.tag == "Player" && detected) {
			if (player.transform.position.y <= transform.position.y + transform.localScale.y * .5f && 
				player.transform.position.x * transform.localScale.x > transform.position.x * transform.localScale.x) {
				Object.Destroy(player, 2);
				player.GetComponent<PlayerController>().enabled = false;
			}
		}
	}
}
