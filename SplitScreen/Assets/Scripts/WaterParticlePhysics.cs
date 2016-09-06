using UnityEngine;
using System.Collections;

public class WaterParticlePhysics : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// gameObject.layer = LayerMask.NameToLayer("WaterCollider");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Here we handle the collision events with another particles, in this example water+lava= water-> gas
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.tag=="Player"){ 
			Debug.Log("WATER COLLISION");
			other.gameObject.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity);
		}
	}
}
