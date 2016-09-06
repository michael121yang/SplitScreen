using UnityEngine;
using System.Collections;

public class Split : MonoBehaviour {
	private Rigidbody2D rb2d; 
	private PolygonCollider2D collider;

	public Vector2[] cuts;
	private int[] cutLocations;
	public float cutSpeed;

	public  GameObject parent;
	public 	GameObject child;
	public  bool 	   isRight = false;
	public  Vector3    direction;
	public  Camera 	   Camera;

	private GameObject cutter;
	private bool collapsing;
	private Vector2[] originalMesh;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		collider = GetComponent<PolygonCollider2D> ();
		collapsing = false;

		cutter = GameManager.Instance.cutter;

		if (parent == null) {
			GameObject[] list = cutter.GetComponent<LineMaker>().objects;
			GameObject[] temp2 = list;
			GameObject[] temp = new GameObject[temp2.Length + 1];

			temp2.CopyTo(temp, 0);
			temp[temp2.Length] = gameObject;

			cutter.GetComponent<LineMaker>().objects = temp;
		}
		originalMesh = new Vector2[collider.points.Length];
		collider.points.CopyTo(originalMesh, 0);
	}

	// Update is called once per frame
	void Update () {
		if (cuts.Length > 0) {
			splitObject();
		}

		if (parent == null) {
		 	if (collapsing) {
				collider.points = originalMesh;
				GetComponent<ColliderToMesh>().goTime = true;
				collapsing = false;				
			}
		} else if (child != null) {
			if (collapsing && rb2d.velocity.magnitude == 0) {
				parent.GetComponent<Split>().collapsing = true;
				Object.Destroy(gameObject, .1f);
			} 			
		} else {
			if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && GameManager.Instance.canCut) {
				if(isRight) {
					rb2d.velocity = -1 * direction.normalized * cutSpeed;
				}
				collapsing = true;
			}

			if (collapsing && rb2d.velocity.magnitude == 0) {
				parent.GetComponent<Split>().collapsing = true;
				Object.Destroy(gameObject, .1f);
			} 
		}
	}

	void FixedUpdate () {
		if (rb2d.velocity.magnitude < .1 && rb2d.velocity.magnitude > 0) {
			rb2d.velocity = Vector2.zero;
		} else if (rb2d.velocity.magnitude != 0) {
			rb2d.velocity *= .9f;
		} 
	}

	// cut object along current cuts coordinates
	void splitObject() {
		cutLocations = new int[cuts.Length];

		// accomodate for scale
		bool previousWasLeft = false;
		int numCrossings = 0;
		int rightStart = 0; int rightEnd = 0;
		int pointsLength = collider.points.Length;

		Vector2 point = collider.points[pointsLength-1];

		// for(int i = 0; i < cuts.Length; i++) {
		// 	cuts[i].x /= transform.localScale.x; cuts[i].y /= transform.localScale.y;
		// }

		point = collider.points[pointsLength-1];
		previousWasLeft= (((point.x - cuts[0].x) * (cuts[1].y - cuts[0].y) - (point.y - cuts[0].y) * (cuts[1].x - cuts[0].x)) > 0);

		// Calculate new polygon colliders
		Vector2[] leftPoints = new Vector2[2]; Vector2[] rightPoints= new Vector2[2];

		// sort out each point in original collider based on orientation relative to line
		for (int i = 0; i < pointsLength; i++) {
			point = collider.points[i];

			if (numCrossings < 2) {
				bool isLeft = ((point.x - cuts[0].x) * (cuts[1].y - cuts[0].y) - (point.y - cuts[0].y) * (cuts[1].x - cuts[0].x)) > 0;

				if (previousWasLeft != isLeft) {
					//do stuff
					for (int j = 0; j < cuts.Length; j++) {
						Debug.Log(((cuts[j]-point).normalized - (collider.points[(pointsLength + i - 1)%pointsLength] - point).normalized).magnitude);
						if (((cuts[j]-point).normalized - (collider.points[(pointsLength + i - 1)%pointsLength] - point).normalized).magnitude < .001) {
							Debug.Log(cuts[j].x * transform.localScale.x + " " + cuts[j].y *transform.localScale.y);
							cutLocations[j] = i;

							if(j == 0 || j == 1) {
								numCrossings++;

								if (isLeft) {
									leftPoints[1] = cuts[j];
									rightPoints[0] = cuts[j];
									rightEnd = i;
								}
								else {
									leftPoints[0] = cuts[j];
									rightPoints[1] = cuts[j];							
									rightStart = i;
								}
							}
						}
					}
				}
				previousWasLeft = isLeft;
			} 
		}

		Debug.Assert(numCrossings == 2, numCrossings + " was actual amount");


		// calculate direction of cut
		direction = new Vector3((cuts[1].x - cuts[0].x) * transform.localScale.x, 
								(cuts[1].y - cuts[0].y) * transform.localScale.y, 0f);

		clearFirstCuts();

		// create new objects
		GameObject right = (GameObject)Instantiate(gameObject, transform.position, transform.rotation);

		// make new objects link back to old one
		right.GetComponent<Split>().parent  = this.gameObject;
		right.GetComponent<Split>().child   = null;
		right.GetComponent<Split>().isRight = true;		
		right.GetComponent<Split>().cuts 	= new Vector2[0];	

		child  = right;
		isRight = false;		

		// find points in right collider
		while (rightStart > rightEnd) {
			rightEnd += pointsLength;
		}

		for (int i = rightStart; i < rightEnd; i++) {
			rightPoints = append(rightPoints, collider.points[i % pointsLength]);
			for (int j = 0; j < cutLocations.Length; j += 2) {
				if (i == cutLocations[j]) {
					right.GetComponent<Split>().cuts = append(append(right.GetComponent<Split>().cuts, cuts[j]), cuts[j+1]);	
					clearTwoCuts(j);
					j = cutLocations.Length;
				}	
			}
		}

		// find points in left collider
		while (rightEnd > rightStart) {
			rightStart += pointsLength;
		}

		for (int i = rightEnd; i < rightStart; i++) {
			leftPoints = append(leftPoints, collider.points[i % pointsLength]);

		}

		// move if no other cuts to be made on child
		if (right.GetComponent<Split>().cuts.Length == 0) {
			right.GetComponent<Rigidbody2D>().velocity = direction.normalized * cutSpeed;
		}

		// make new objects have new colliders
		right.GetComponent<PolygonCollider2D>().points = rightPoints;
		collider.points = leftPoints;

		// remesh
		GetComponent<ColliderToMesh>().goTime = true;
	}

	Vector2[] append(Vector2[] array, Vector2 obj) {
		Vector2[] temp2 = array;
		Vector2[] temp = new Vector2[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}

	void clearFirstCuts() {
		Vector2[] temp = cuts;
		cuts = new Vector2[temp.Length - 2];
		int[] tempL = cutLocations;
		cutLocations = new int[tempL.Length - 2];

		for (int i = 0; i < cuts.Length; i++) {
			cuts[i] = temp[i+2];
			cutLocations[i] = tempL[i+2];
		}
	}

	void clearTwoCuts(int a) {
		Vector2[] temp = cuts;
		cuts = new Vector2[temp.Length - 2];
		int[] tempL = cutLocations;
		cutLocations = new int[tempL.Length - 2];

		int num = 0;

		for (int i = 0; i < cuts.Length; i++) {
			if (i == a) {
				num++;
			} else if (i == a+1) {
				num++;
			} else {
				cuts[i] = temp[i+num];
				cutLocations[i] = tempL[i+num];

			}
		}
	}

	float squareDistance(Vector2 a, Vector2 b) {
		return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
	}
}