using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineMaker : MonoBehaviour {
	private LineRenderer line;
	private SpriteRenderer Device;
	private Collider2D collider;

	public GameObject user;

	private float prevDistance;

	private RecallMode RECALL_MODE;
	private LineMode   LINE_MODE; 

	public float lineLength;
	public LayerMask mask;
	public Camera Camera;
	public float cutSpeed;

	public Material RightLine;
	public Material LeftLine;
	// public GameObject Device;

	public  GameObject[] objects;
	private GameObject[] removed;

	private bool draw;
	private bool didCut;
	private int reverseCut;
	private Vector3[] array;
	private Vector3 direction;

	private float increment = 0;

	// Use this for initialization;
	bool CheckRecall() {
		if (RECALL_MODE == RecallMode.proximity) {
			if (Physics2D.OverlapCircle(transform.position, 1, 1 << LayerMask.NameToLayer("Player"))) {
				GameManager.Instance.canRecall = true;				
			} else {
				GameManager.Instance.canRecall = false;
			}
		} else if (RECALL_MODE == RecallMode.line) {
			if (Physics2D.CircleCast(array[0], 1, array[1]-array[0], (array[1]-array[0]).magnitude, 1 << LayerMask.NameToLayer("Player"))) {
				GameManager.Instance.canRecall = true;								
			} else {
				GameManager.Instance.canRecall = false;
			}
		} else if (RECALL_MODE == RecallMode.any) {
			GameManager.Instance.canRecall = true;
		} else if (RECALL_MODE == RecallMode.inactive) {
			GameManager.Instance.canRecall = false;
		}

		return GameManager.Instance.canRecall && GameManager.Instance.canCut && GameManager.Instance.cut && !didCut;
	}


	void Awake () {
		GameManager.Instance.cutter = gameObject;
	}

	void Start () {
		Device = GetComponent<SpriteRenderer>();
		Device.enabled = false;
		collider = GetComponent<BoxCollider2D>();
		line = GetComponent<LineRenderer>();
		line.sortingLayerName = "Foreground";
		line.sortingOrder = 0;
		objects =  GameObject.FindGameObjectsWithTag("cuttableLevel");
		removed = new GameObject[0];
		array = new Vector3[2];

		RECALL_MODE = GameManager.Instance.RECALL_MODE;
		LINE_MODE	= GameManager.Instance.LINE_MODE;

		draw = false;
		didCut = false;
		reverseCut = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (user != null)
			GetComponent<Light>().color = user.GetComponent<SpriteRenderer>().color;

		if (user != null && user.tag == "Player") {
			if (RECALL_MODE == RecallMode.any) 
				GameManager.Instance.canRecall = true;
			else {
				if (GameManager.Instance.canRecall && GameManager.Instance.cut) {
					gameObject.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1);
				} else {
					gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
				}
			}

			if (GameManager.Instance.canCut) {
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
					draw = true;
				}

				if (GameManager.Instance.cut) {
					if (LINE_MODE == LineMode.active) line.enabled = true;
					Device.enabled = true;
					collider.enabled = true;
					gameObject.layer = LayerMask.NameToLayer("Uncuttables");

					CheckRecall();

					if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))) {
						RecallCut();
					} 
				} else if (draw) {
					if (Input.GetMouseButtonDown(1)) {
						reverseCut = -1;
					} else if (Input.GetMouseButtonDown(0)) {
						reverseCut = 1;
					}

					if (LINE_MODE == LineMode.active) line.enabled = true;
					Device.enabled = true;
					collider.enabled = false;
					LayerMask.NameToLayer("UI");
					Vector2 mousepos = new Vector2(Camera.ScreenToWorldPoint(Input.mousePosition).x, Camera.ScreenToWorldPoint(Input.mousePosition).y);

					Vector2 off2D = mousepos - (Vector2)user.transform.position;
					Vector2 dir2D = new Vector2(off2D.y, -off2D.x);

					dir2D.Normalize();

					DrawLine(reverseCut, new Vector3(dir2D.x, dir2D.y, 0));

					line.material.SetTextureScale("_MainTex", new Vector2((array[1] - array[0]).magnitude * 2, 1));

					if ((Input.GetMouseButtonUp(0) && reverseCut == 1 || Input.GetMouseButtonUp(1) && reverseCut == -1) && GameManager.Instance.canCut) {
						MakeCut(reverseCut, direction);
					}
				} else {
					Device.enabled = false;
					collider.enabled = false;
					line.enabled = false;
					gameObject.layer = LayerMask.NameToLayer("UI");
					draw = false;
				}
			} 
		}

		Vector3[] temp = new Vector3[2];

		temp[0] = (Vector3)array[0] + direction * increment;
		temp[1] = (Vector3)array[1] + direction * increment;

		line.SetPositions(temp);
	}

	public void MakeCut(int rc, Vector3 dir) {
		didCut = true;

		DrawLine(rc, dir);

		collider.enabled = true;

		draw = false; 
		GameManager.Instance.cutStart = array[0]; GameManager.Instance.cutEnd = array[1];
		GameManager.Instance.direction = direction;
		GameManager.Instance.cutOrigin = transform.position;
		GameManager.Instance.cutSpeed = cutSpeed * reverseCut;

		if (LINE_MODE == LineMode.active) {
			List<SpriteSlicer2DSliceInfo> sliceInfo =  new List<SpriteSlicer2DSliceInfo>();

			SpriteSlicer2D.SliceAllSprites(array[0], array[1], false, ref sliceInfo, mask);

			for (int i = 0; i < objects.Length; i++) {
				Vector3 point = objects[i].transform.position;

				if (((point.x - array[0].x) * (array[1].y - array[0].y) - (point.y - array[0].y) * (array[1].x - array[0].x)) < 0) {
					objects[i].GetComponent<Split>().isRight = true;
				} 
			}
		}
	}

	public void RecallCut() {
		if (CheckRecall()) {
			draw = false; didCut = true;
			if (LINE_MODE == LineMode.active) {
				for (int i = 0; i < objects.Length; i++) {
					Vector3 point = objects[i].transform.position;
					if (((point.x - array[0].x) * (array[1].y - array[0].y) - (point.y - array[0].y) * (array[1].x - array[0].x)) < 0) {
						objects[i].GetComponent<Split>().isRight = true;
					}
				}	
			}			
		}
	}

	public void DrawLine(int rc, Vector3 dir) {
		dir.Normalize();

		Device.enabled = true;

		direction = dir;
		reverseCut = rc;

		if (LINE_MODE == LineMode.active) line.enabled = true;

		array[0] = transform.position - dir * (lineLength);
		array[1] = transform.position + dir * (lineLength);

		RaycastHit2D hit1 = Physics2D.Raycast(transform.position, -dir, lineLength, 1 << LayerMask.NameToLayer("Blockers"));
		RaycastHit2D hit2 = Physics2D.Raycast(transform.position,  dir, lineLength, 1 << LayerMask.NameToLayer("Blockers"));

		if(hit1) array[0] = (Vector3)hit1.point - dir + new Vector3(0f, 0f, -.5f);	
		if(hit2) array[1] = (Vector3)hit2.point + dir + new Vector3(0f, 0f, -.5f);

		line.SetPositions(array);	

		if (reverseCut == 1) {
			line.material = RightLine;
		} else {
			line.material = LeftLine;
		}

		line.material.SetTextureScale("_MainTex", new Vector2((array[1] - array[0]).magnitude * 2, 1));
	}

	void FixedUpdate() {
		increment += .01f * reverseCut;
		increment = increment % 1;

		if (GameManager.Instance.transitioning) {
				collider.enabled = true;
				collider.isTrigger = true;
				gameObject.transform.position += (user.transform.position - gameObject.transform.position).normalized * (user.transform.position - gameObject.transform.position).magnitude * .2f;
				if ((gameObject.transform.position - user.transform.position).magnitude < prevDistance) {
					prevDistance = (gameObject.transform.position - user.transform.position).magnitude;
				} else {
					gameObject.transform.position = user.transform.position + (gameObject.transform.position - user.transform.position).normalized	* prevDistance * .5f;	
				}
		} else {
			prevDistance = 	Mathf.Infinity;
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (user.tag == "Player") {
			if (GameManager.Instance.transitioning && col.GetComponent<Collider2D>().gameObject.tag == "Player") {
				collider.enabled = false;
				collider.isTrigger = false;
				Device.enabled = false;
			}
		}
	}

	void LateUpdate() {
			if (GameManager.Instance.canCut) {
				if (didCut) {
					GameManager.Instance.cut = !GameManager.Instance.cut;
					if (!GameManager.Instance.cut)
						GameManager.Instance.transitioning = true;
					GameManager.Instance.canCut = false;
					GameManager.Instance.time = Time.time;
					didCut = false;				
				}
			} else if (Time.time - GameManager.Instance.time > 1) {
				GameManager.Instance.canCut = true;
				GameManager.Instance.transitioning = false;
				if (!GameManager.Instance.cut)
					Device.enabled = false;
					collider.enabled = false;
					gameObject.layer = LayerMask.NameToLayer("UI");
			}
	}
}
