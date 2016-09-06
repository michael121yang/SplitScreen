using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitAndDie : MonoBehaviour {
	private Rigidbody2D rb2d; 
	private Collider2D collider;

	public float m_Timer = 0;
	public float m_FadeDelay = 1.0f;
	public float m_FadeTime = 1.0f;
    public Color m_InitialColor;    

	public  bool isChild = false;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		collider = GetComponent<Collider2D> ();
		m_InitialColor = GetComponent<Renderer>().material.color;
		if (isChild) {
			rb2d.constraints = RigidbodyConstraints2D.None;
		}
	}

	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate () {
		if (isChild) {
            m_Timer += Time.deltaTime;

            if (m_FadeTime > 0)
            {
                Color newColor = m_InitialColor;
                newColor.a = 1.0f - Mathf.Clamp01((m_Timer - m_FadeDelay) / m_FadeTime);
                newColor.a *= m_InitialColor.a;
                GetComponent<Renderer>().material.color = newColor;
            }

            if ((m_Timer - m_FadeDelay) >= m_FadeTime)
            {
                Destroy(this.gameObject);
            }
		}
	}

	void OnDestroy() {
	}

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		Debug.Log("did thing");
		foreach (GameObject c in sliceInfo.ChildObjects) {
			c.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
			c.GetComponent<Rigidbody2D>().gravityScale = 1;
			c.GetComponent<Rigidbody2D>().isKinematic = false;
			c.AddComponent(this.GetType());
			c.GetComponent<SplitAndDie>().isChild = true;
			c.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
			c.GetComponent<SplitAndDie>().m_FadeTime= m_FadeTime;
			c.GetComponent<SplitAndDie>().m_FadeDelay = m_FadeDelay;


			Debug.Log(GetComponent<Renderer>().material.color);

			c.GetComponent<Collider2D>().isTrigger = collider.isTrigger;

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
				if (Mathf.Abs((cuts[1] - points[i]).magnitude + (points[i] - cuts[0]).magnitude - (cuts[1] - cuts[0]).magnitude) > .001) {
					if (((points[i].x - cuts[0].x) * (cuts[1].y - cuts[0].y) - (points[i].y - cuts[0].y) * (cuts[1].x - cuts[0].x)) < -0.01) {
						c.GetComponent<Rigidbody2D>().velocity = (cuts[1] - cuts[0]).normalized * GameManager.Instance.cutSpeed * .5f;
					} else {
						c.GetComponent<Rigidbody2D>().velocity = (cuts[0] - cuts[1]).normalized * GameManager.Instance.cutSpeed * .5f;						
					}
				} 
			}

			c.GetComponent<Rigidbody2D>().angularVelocity = (.5f - Random.value)*GameManager.Instance.cutSpeed*2;
		}

			Object.Destroy(gameObject);
	}

	Vector2[] append(Vector2[] array, Vector2 obj) {
		Vector2[] temp2 = array;
		Vector2[] temp = new Vector2[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}
}