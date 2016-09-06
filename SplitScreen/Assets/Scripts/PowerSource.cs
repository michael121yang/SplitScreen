using UnityEngine;
using System.Collections;

public class PowerSource : MonoBehaviour {

	public bool isChild;
	public bool isShard;
	public int numCuts = 0;

	private float m_Timer = 0;
	private float m_FadeDelay = .1f;
	private float m_FadeTime = .5f;

	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		if (isChild) {
			if (rb2d.velocity.magnitude < .1 && rb2d.velocity.magnitude > 0) {
				rb2d.velocity = Vector2.zero;
			} else if (rb2d.velocity.magnitude != 0) {
				rb2d.velocity *= .5f;
			}

	        m_Timer += Time.deltaTime;

	        if ((m_Timer - m_FadeDelay) >= m_FadeTime)
	        {
	        	if (numCuts < 5) {
	        		rb2d.isKinematic = false;
					SpriteSlicer2D.ExplodeSprite(gameObject, 1, 20);
				} else { 
		            Destroy(this.gameObject);
		        }
	        }
	    }
	}

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		if (!isChild) {
			GetComponent<Collider2D>().enabled = false;
			GetComponent<Renderer>().enabled = false;
			Destroy(gameObject, 3);
			LevelManager.Instance.destroyed = true;
		}
		foreach (GameObject c in sliceInfo.ChildObjects) {
			c.AddComponent(this.GetType());
			c.GetComponent<PowerSource>().isChild = true;
			c.GetComponent<PowerSource>().numCuts = numCuts + 1;

			if (numCuts == 0) {
				Vector3 direction = new Vector3((sliceInfo.SliceExitWorldPosition.x - sliceInfo.SliceEnterWorldPosition.x), 
										(sliceInfo.SliceExitWorldPosition.y - sliceInfo.SliceEnterWorldPosition.y), 0f);
				direction.Normalize();	

				Vector2[] points = c.GetComponent<PolygonCollider2D>().points;
				Vector2[] cuts = new Vector2[2];
				cuts[0] = GameManager.Instance.cutStart;
				cuts[1] = GameManager.Instance.cutEnd;

				for(int j = 0; j < cuts.Length; j++) {	
					cuts[j] -= (Vector2)transform.position;
					cuts[j].x /= transform.localScale.x;
					cuts[j].y /= transform.localScale.y;
				}				

				for (int i = 0; i < points.Length; i++) {
					if (((points[i].x - cuts[0].x) * (cuts[1].y - cuts[0].y) - (points[i].y - cuts[0].y) * (cuts[1].x - cuts[0].x)) < -0.01) {
						if (Mathf.Abs((cuts[1] - points[i]).magnitude + (points[i] - cuts[0]).magnitude - (cuts[1] - cuts[0]).magnitude) > .001) {
							c.GetComponent<Rigidbody2D>().velocity = direction * GameManager.Instance.cutSpeed;					
						}
					} 
				}
			}
		}
	}
}
