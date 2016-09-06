using UnityEngine;
using System.Collections;

public class Boss1AI : MonoBehaviour {
	public int numKills = 0;

	// [HideInInspector] 
	public float maxSpeed = 3f;
	public float speedInc = 0.5f;
	public float jumpForce = 120f;
	public float jumpRatio = 0.65f; //used for holding down to jump higher
	public LayerMask groundMask;
	public GameObject Device;
	public GameObject shadow;
	public Camera Camera;

	private int frameCounter = 0;
	private GameObject player;
	private GameObject playerShadow;

	private bool draw = false;
	public int  reverseCut = 1;

	public bool grounded = false;
	public bool groundAhead = false;
	private Collider2D bottom;
	private Rigidbody2D rb2d; 

	private float m_Timer = 0;
	private float m_ActivateDelay = 1.0f;
	private float m_CutDelay = 1f;
	private float m_recallDelay = 2f;
	private float m_resetTime = 3f;

	private float r_Timer = Mathf.Infinity;
	private float r_recallDelay = 2f;
	private float r_Duration = 2.7f;
	private float r_resetTime = 3.0f;

	private bool r_jump = false;
	private Vector2 r_Direction;

	private Vector2 direction;

	private float width;
	private float height;


	// Use this for initialization
	void Awake () 
	{
		rb2d = GetComponent<Rigidbody2D> ();
		bottom = GetComponent<EdgeCollider2D> ();
	}
	
	void Start () 
	{
		player = GameManager.Instance.player;
		Device = GameManager.Instance.cutter;
		player.GetComponent<PlayerController>().Device = null;
		Device.GetComponent<LineMaker>().user = gameObject;

		width = GetComponent<BoxCollider2D>().size.x * transform.localScale.x;
		height = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
	}

	// Update is called once per frame
	void Update () 
	{	
		grounded = false; groundAhead = false;
		// check if on ground
		grounded = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, .1f), 0, Vector2.down, transform.localScale.y*.5f, groundMask);
	}


	void LateUpdate () {

	}

	void FixedUpdate() 
	{
		rb2d.velocity = Vector2.zero;

		if (Device != null) {
	        m_Timer += Time.deltaTime;

	        if (m_Timer >= m_ActivateDelay) {
	        	m_ActivateDelay = m_resetTime + 1;
	        	m_Timer = 0;
	        } 

			if (m_ActivateDelay < m_resetTime) {
				Device.GetComponent<LineMaker>().RecallCut();
				Device.transform.position = transform.position;
			} else {
		        if (m_Timer >= m_resetTime)
		        {
					if (playerShadow != null)
						Destroy(playerShadow);
		        	m_Timer = 0;
		        	// if (Random.value < .5)
		        		reverseCut *= -1;
		        } else if (m_Timer >= m_recallDelay) {
		        	Device.GetComponent<LineMaker>().RecallCut();
		        } else if (m_Timer >= m_CutDelay) {
		        	if (!GameManager.Instance.cut) {
		            	Device.GetComponent<LineMaker>().MakeCut(reverseCut, direction);
		            }
		        } else {
		        	if (playerShadow == null) {
		        		Vector2 projectedPosition = (Vector2)player.transform.position + new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, 0) * m_CutDelay;
		        		playerShadow = (GameObject)Instantiate(shadow, projectedPosition, Quaternion.identity);
		                Color newColor = playerShadow.GetComponent<Renderer>().material.color;
		                newColor.a *= .5f;
		                playerShadow.GetComponent<Renderer>().material.color = newColor;
		        	}
			 		Vector2 offset = playerShadow.transform.position - transform.position;

					groundAhead = Physics2D.Raycast((Vector2)transform.position + new Vector2(transform.localScale.x*Mathf.Sign(offset.x), 0), 
													new Vector2(0, -1), transform.localScale.y*1.5f, groundMask);

					// if (Mathf.Abs(offset.x) > .1 && groundAhead) {
					// 	rb2d.velocity = new Vector2(Mathf.Sign(offset.x), 0) * maxSpeed;
					// }

					if (offset.magnitude > .6f) {
						// // Debug.Log("doing a thing");	
						float angle = Mathf.Asin(.6f / offset.magnitude) * Mathf.Rad2Deg;

						direction = Rotate(offset, -angle);
						offset = Rotate(direction, 90);

						offset.Normalize();

						Device.transform.position = transform.position + (Vector3)Rotate(offset, 90 * (m_CutDelay - m_Timer) / m_CutDelay) * .6f + new Vector3(0f, 0f, -.5f);

						Device.GetComponent<LineMaker>().DrawLine(reverseCut, Rotate(direction, 90 * (m_CutDelay - m_Timer) / m_CutDelay));
					}       
				}	
	        }
	    } else {
	       	m_Timer = 0;
	    }
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Player" && Device != null) {
			Debug.Log("Device taken");
			col.gameObject.GetComponent<PlayerController>().Device = Device;
			Device.GetComponent<LineMaker>().user = col.gameObject;	
			Device = null;
			if (playerShadow != null) {
				Destroy(playerShadow);
			}

			gameObject.GetComponent<SpriteRenderer>().color = new Color(.8f, .2f, .2f);			
			gameObject.GetComponent<Renderer>().material.color = new Color(.8f, .2f, .2f);		
		}
	}
		
	void Jump()
	{
		if (grounded) {
			Debug.Log("jumping");
			rb2d.velocity = new Vector2(rb2d.velocity.x, 6);
		}
	}

	Vector2 Rotate(Vector2 vector, float degrees) {
		Vector2 v = new Vector2(vector.x, vector.y);

        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		GameObject next = (GameObject)Instantiate(gameObject);
		player.GetComponent<PlayerController>().Device = null;
		next.GetComponent<SpriteRenderer>().color = new Color(1, .65f, 0);
		next.GetComponent<Boss1AI>().numKills += 1;
	}
}
