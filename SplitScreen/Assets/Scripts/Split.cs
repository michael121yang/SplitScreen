using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Split : MonoBehaviour {
	private Rigidbody2D rb2d; 
	private Collider2D collider;

	public  GameObject parent;
	public 	GameObject child;
	public  bool 	   isRight = false;
	public  Vector3    direction;
	public 	Vector2    originalPos;

	private bool collapsing;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		collider = GetComponent<Collider2D> ();
		collapsing = false;	
		originalPos = transform.localPosition;
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate () {
		Vector2 difference = (Vector2)transform.localPosition - originalPos;

		if (parent != null) {
			if(isRight) {
				if ((Vector2)transform.localPosition == originalPos && !collapsing) {
					rb2d.velocity = direction * GameManager.Instance.cutSpeed;
				}
			}

			if (!GameManager.Instance.cut && !collapsing) {
				if (isRight) {
					rb2d.velocity = -1 * direction * GameManager.Instance.cutSpeed;
				}
				collapsing = true;	
			}

			if (collapsing && rb2d.velocity.magnitude < .1f) {
				if (isRight) {
					Object.Destroy(gameObject);
					rb2d.velocity = Vector2.zero;
					transform.localPosition = originalPos;
				} else {
					Object.Destroy(gameObject, .9f);
				}
			} 
		} else {
			if (isRight) {
				if ((Vector2)transform.localPosition == originalPos) {
					rb2d.velocity = GameManager.Instance.direction * GameManager.Instance.cutSpeed;
				} else {
					rb2d.velocity = - GameManager.Instance.direction * GameManager.Instance.cutSpeed;
				}			

				isRight = false;
			}
		}

		if (rb2d.velocity.magnitude < .1) {
			rb2d.velocity = Vector2.zero;
			if (!GameManager.Instance.cut)
				transform.localPosition = originalPos;
		} else {
			rb2d.velocity *= .9f;
		} 
	}

	void OnDestroy() {
		if (parent != null) {
			parent.SetActive(true);
			parent.GetComponent<Split>().isRight = false;		
		}
	}

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		Debug.Log("sliced");
		direction = new Vector3((sliceInfo.SliceExitWorldPosition.x - sliceInfo.SliceEnterWorldPosition.x), 
								(sliceInfo.SliceExitWorldPosition.y - sliceInfo.SliceEnterWorldPosition.y), 0f);
		direction.Normalize();		

		foreach (GameObject c in sliceInfo.ChildObjects) {
			c.AddComponent(this.GetType());
			c.GetComponent<Split>().parent  = gameObject;
			c.GetComponent<Split>().child   = null;
			c.GetComponent<Split>().direction = direction;
			c.GetComponent<Split>().isRight = false;		

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
				if (((points[i].x - cuts[0].x) * (cuts[1].y - cuts[0].y) - (points[i].y - cuts[0].y) * (cuts[1].x - cuts[0].x)) < 0) {
					if (Mathf.Abs((cuts[1] - points[i]).magnitude + (points[i] - cuts[0]).magnitude - (cuts[1] - cuts[0]).magnitude) > .001) {
						c.GetComponent<Split>().isRight = true;
					}
				} 
			}

			c.GetComponent<Collider2D>().isTrigger = collider.isTrigger;
		}
	}

	Vector2[] append(Vector2[] array, Vector2 obj) {
		Vector2[] temp2 = array;
		Vector2[] temp = new Vector2[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}
}