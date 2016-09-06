using UnityEngine;
using System.Collections;

public enum CameraMode {
    follow, scenes
}

public class CameraController : MonoBehaviour {
	public GameObject arrow;
	public CameraMode CAMERA_MODE;

	private GameObject player;
	private Vector3 moving;
	private float speed;
	private float horizontal;
	private float vertical;

	private Vector2 playerEnter;
	private float timer;

	// Use this for initialization
	void Start () {
		player = GameManager.Instance.player;
		playerEnter = player.transform.position;
		moving = Vector3.zero;
		speed = .5f;
	}
	
	// Update is called once per frame
	void Update () {
		horizontal = Camera.main.orthographicSize * Screen.width / Screen.height * .95f;
		vertical   = Camera.main.orthographicSize * .95f;

		if (CAMERA_MODE == CameraMode.follow)
			transform.position = player.transform.position + new Vector3(0, 0, -5);
		else if (CAMERA_MODE == CameraMode.scenes) {
			if (moving == Vector3.zero) {
				Time.timeScale = 1;
				if (Time.time - timer > 2)
					GameManager.Instance.playerCheckpoint = player.transform.position;

				Vector3 offset = player.transform.position - transform.position;
				if (Mathf.Abs(offset.x) > horizontal) {
					timer = Time.time; playerEnter = player.transform.position;
					moving = transform.position + new Vector3(horizontal * 2 * Mathf.Sign(offset.x), 0, 0);
				}
				if (Mathf.Abs(offset.y) > vertical) {
					timer = Time.time; playerEnter = player.transform.position;
					transform.position = transform.position + new Vector3(0, vertical * 2 * Mathf.Sign(offset.y), 0);
				}
			} else {
				Time.timeScale = 0; 
				transform.position += speed * (moving - transform.position).normalized;
				if ((transform.position - moving).magnitude < speed) {
					transform.position = moving;
					moving = Vector3.zero;
				}
			}
		}

		if (GameManager.Instance.canRecall && GameManager.Instance.cut) {
			arrow.GetComponent<SpriteRenderer>().color = new Color(.5f, .5f, 1);
		} else {
			arrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
		}

		arrow.SetActive(false);
		if (GameManager.Instance.cut) {
			Vector3[] array = new Vector3[2];
			array[0] = GameManager.Instance.cutStart;
			array[1] = GameManager.Instance.cutEnd;

			Vector2 DeviceLocation = GameManager.Instance.cutter.transform.position;

			Vector2 dirToLine = (DeviceLocation - (Vector2)transform.position).normalized;

			if (DeviceLocation.x < transform.position.x - horizontal || DeviceLocation.x > transform.position.x + horizontal ||
				DeviceLocation.y < transform.position.y - vertical   || DeviceLocation.y > transform.position.y + vertical) {

				// if (((transform.position.x - array[0].x) * (array[1].y - array[0].y) - (transform.position.y - array[0].y) * (array[1].x - array[0].x)) < 0) {
				// 	dirToLine *= -1;
				// }
				arrow.SetActive(true);

				RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + 50 * dirToLine, -dirToLine, Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"));
				arrow.transform.position = hit.point - dirToLine * 1f;
				if (dirToLine.x > 0) {
					arrow.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan(dirToLine.y / dirToLine.x) * 180 / Mathf.PI + 180);
				} else {
					arrow.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan(dirToLine.y / dirToLine.x) * 180 / Mathf.PI);
				}
			}
		}
	}

	void FixedUpdate () {
		// Debug.Log("fixed up");

	}
}
