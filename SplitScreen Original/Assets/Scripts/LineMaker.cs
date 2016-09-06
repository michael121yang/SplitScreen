using UnityEngine;
using System.Collections;

public class LineMaker : MonoBehaviour {
	private LineRenderer line;

	public float lineLength;
	public LayerMask mask;
	public Camera Camera;
	public float cutSpeed;
	public  GameObject[] objects;
	private GameObject[] removed;

	private bool cut;
	private bool draw;
	private int reverseCut;
	private Vector3[] array;
	private Vector3 direction;

	// Use this for initialization;
	void Awake () {
		line = GetComponent<LineRenderer>();
		line.sortingLayerName = "Foreground";
		line.sortingOrder = 10;
		objects = new GameObject[0];
		// objects[0] = gameObject;
		removed = new GameObject[0];
		array = new Vector3[2];

		GameManager.Instance.cutter = gameObject;

		cut = false;
		draw = false;
		reverseCut = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.canCut) {
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
				draw = true;
			}

			if (cut) {
				line.enabled = true;
				if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && GameManager.Instance.canCut) {
					cut = false; draw = false;
					for (int i = 0; i < objects.Length; i++) {
						Vector3 point = objects[i].transform.position;

						if (((point.x - array[0].x) * (array[1].y - array[0].y) - (point.y - array[0].y) * (array[1].x - array[0].x)) < 0) {
							objects[i].GetComponent<Rigidbody2D>().velocity = -1 * direction.normalized * cutSpeed * reverseCut;
						
						}

					}				
					mergeObjects();
				} 
			} else if (draw) {
				if (Input.GetMouseButtonDown(1)) {
					reverseCut = -1;
				} else if (Input.GetMouseButtonDown(0)) {
					reverseCut = 1;
				}

				line.enabled = true;
				Vector2 mousepos = new Vector2(Camera.ScreenToWorldPoint(Input.mousePosition).x, Camera.ScreenToWorldPoint(Input.mousePosition).y);

				Vector2 off2D = mousepos - new Vector2(transform.position.x, transform.position.y);
				Vector2 dir2D = new Vector2(off2D.y, -off2D.x);

				off2D.Normalize();
				dir2D.Normalize();

				Vector3 offset    = new Vector3(off2D.x, off2D.y, 0);
				direction = new Vector3(dir2D.x, dir2D.y, 0);

				array[0] = transform.position + offset * .5f - direction * lineLength + new Vector3(0f, 0f, -.5f);
				array[1] = transform.position + offset * .5f + direction * lineLength + new Vector3(0f, 0f, -.5f);
				line.SetPositions(array);	

				if(reverseCut == 1) {
					line.SetWidth(.2f, .05f);	
				} else {
					line.SetWidth(.05f, .2f);
				}

				if ((Input.GetMouseButtonUp(0) && reverseCut == 1 || Input.GetMouseButtonUp(1) && reverseCut == -1) && GameManager.Instance.canCut) {

					cut = true; draw = false;

					RaycastHit2D[] hits_forward = new RaycastHit2D[0]; RaycastHit2D[] hits_backward = new RaycastHit2D[0];
					hits_forward  = RaycastAllUnrestricted(hits_forward, array[0],  direction, Mathf.Infinity, mask);
					hits_backward = RaycastAllUnrestricted(hits_backward, array[1], -direction, Mathf.Infinity, mask);

					// Debug.Log(hits_backward.Length);
					for (int i = 0; i < hits_forward.Length; i++) {
						for (int j = 0; j < hits_forward.Length; j++) {
							if (hits_forward[i].point == hits_backward[j].point) {
								// Debug.Log("bingo");
								hits_forward = removeAt(hits_forward, i);
								hits_backward = removeAt(hits_backward, j);
							}
						}
					}					
					// Debug.Log(hits_backward.Length);

					for (int i = 0; i < hits_forward.Length; i++) {
						GameObject obj = hits_forward[i].collider.gameObject;

						for (int j = hits_backward.Length - 1; j >= 0; j--) {
							if (hits_backward[j].collider.gameObject == obj) {
								Vector2[] cuts = obj.GetComponent<Split>().cuts;

								Vector2 hitF = (hits_forward[i].point - new Vector2(obj.transform.position.x, obj.transform.position.y));
								Vector2 hitB = (hits_backward[j].point - new Vector2(obj.transform.position.x, obj.transform.position.y));
								hitF.x /= obj.transform.localScale.x; hitF.y /= obj.transform.localScale.y;
								hitB.x /= obj.transform.localScale.x; hitB.y /= obj.transform.localScale.y;

								cuts = append(cuts, hitF);
								cuts = append(cuts, hitB);
								obj.GetComponent<Split>().cuts = cuts;	
								obj.GetComponent<Split>().cutSpeed = cutSpeed * reverseCut;	
								Debug.Log(hits_forward[i].point + " " + hits_backward[j].point);

								hits_backward = removeAt(hits_backward, j);

								j = -1;


								removeFromObjects(obj);
							}
						}
					}

					for (int i = 0; i < objects.Length; i++) {
						Vector3 point = objects[i].transform.position;

						if (((point.x - array[0].x) * (array[1].y - array[0].y) - (point.y - array[0].y) * (array[1].x - array[0].x)) < 0) {
							objects[i].GetComponent<Rigidbody2D>().velocity = direction.normalized * cutSpeed * reverseCut;
						} 
					}
				}
			} else {
				line.enabled = false;
				draw = false;
			}
		}
	}

	RaycastHit2D[] RaycastAllUnrestricted(RaycastHit2D[] array, Vector2 startpos, Vector2 dir, float dist, LayerMask mask) { 
		Vector2 startPoint;
		if (array.Length == 0) {
			startPoint = startpos;
		} else {
			startPoint = array[array.Length-1].point + dir.normalized * .001f;
		}

		RaycastHit2D hit = Physics2D.Raycast(startPoint, dir, dist, mask);
		if (hit) {
			// Debug.Log("hit! " + hit.point);
			array = append(array, hit);

			return RaycastAllUnrestricted(array, startpos, dir, dist, mask);
		} else {
			return array;
		}
	}

	void LateUpdate() {
		if (GameManager.Instance.canCut) {
			if (cut) {
				if (Input.GetMouseButtonUp(0) && reverseCut == 1 || Input.GetMouseButtonUp(1) && reverseCut == -1) {
					GameManager.Instance.canCut = false;
					GameManager.Instance.time = Time.time;
				}
			} else {
				if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
					GameManager.Instance.canCut = false;
					GameManager.Instance.time = Time.time;
				}
			}
		} else if (Time.time - GameManager.Instance.time > 1) {
			GameManager.Instance.canCut = true;
		}
	}

	void removeFromObjects(GameObject obj) {
		GameObject[] temp  = new GameObject[objects.Length - 1];
		int num = 0;

		for (int i = 0; i < objects.Length; i++) {
			if (objects[i] != obj)  {
				if (i == objects.Length-1 && num < 1) {
					temp = objects;
				}
				temp[i-num] = objects[i];
			}
			else {
				num++;
				removed = append(removed, obj);
			}
		}

		objects = temp;
	}

	void mergeObjects() {
		for(int i = 0; i < removed.Length; i++) {
			objects = append(objects, removed[i]);
		}
		removed = new GameObject[0];
	}

	GameObject[] append(GameObject[] array, GameObject obj) {
		GameObject[] temp2 = array;
		GameObject[] temp = new GameObject[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}

	Vector2[] append(Vector2[] array, Vector2 obj) {
		Vector2[] temp2 = array;
		Vector2[] temp = new Vector2[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}

	RaycastHit2D[] append(RaycastHit2D[] array, RaycastHit2D obj) {
		RaycastHit2D[] temp2 = array;
		RaycastHit2D[] temp = new RaycastHit2D[temp2.Length + 1];
		temp2.CopyTo(temp, 0);
		temp[temp2.Length] = obj;
		return temp;
	}

	void removeInteriorHits(RaycastHit2D[] array1, RaycastHit2D[] array2) {
		for (int i = 0; i < array1.Length; i++) {
			for (int j = 0; j < array2.Length; j++) {
				if (array1[i].point == array2[j].point) {
					// Debug.Log("bingo");
					array1 = removeAt(array1, i);
					array2 = removeAt(array2, j);
				}
			}
		}
	}

	RaycastHit2D[] removeAt(RaycastHit2D[] array, int index) {
		RaycastHit2D[] temp = new RaycastHit2D[array.Length - 1];
		int num = 0;

		for (int i = 0; i < temp.Length; i++) {
			if (i == index) {
				num++;
			}
			temp[i] = array[i + num];
		}
		return temp;
	}
}
