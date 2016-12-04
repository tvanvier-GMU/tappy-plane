using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
	Rigidbody2D rb;
	public float jumpForce;
	public Vector2 currentVelocity;

	public float rotationTime;
	public Vector3 targetUpRotation;
	public Vector3 targetDownRotation;
	Quaternion targetUpQuaternion;
	Quaternion targetDownQuaternion;

	bool playerdead = false;
	public float deathTimer = 2.0f;
	float timeDead = 0;

	public AudioSource deathSound;
	public AudioSource mainTrack;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();

		//convert targets from Vector3 to Quaternion.
		targetUpQuaternion = Quaternion.Euler(targetUpRotation);
		targetDownQuaternion = Quaternion.Euler(targetDownRotation);
	}
	
	// Update is called once per frame
	void Update () {
		//Just so we can see it!
		currentVelocity = rb.velocity;

		if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && playerdead == false){
			rb.velocity = Vector2.zero;
			rb.AddForce(Vector2.up*jumpForce);
		}

		if(Input.GetKeyDown(KeyCode.F)){
			Death();
		}

		if(playerdead == false){
			if(rb.velocity.y > 0){
				transform.rotation = Quaternion.Lerp(transform.rotation, targetUpQuaternion, rotationTime*Time.deltaTime);
			}

			if(rb.velocity.y <= 0){
				transform.rotation = Quaternion.Lerp(transform.rotation, targetDownQuaternion, rotationTime*Time.deltaTime);
			}
		}

		if(playerdead){
			timeDead += Time.deltaTime;
			if(timeDead >= deathTimer){
				Scene currentScene = SceneManager.GetActiveScene();
				SceneManager.LoadScene(currentScene.buildIndex);
			}
		}
	}

	void Death(){
		//don't want to repeat death twice!
		if(playerdead == false){
			GameObject[] allMovingObjects = GameObject.FindGameObjectsWithTag("MovingObject");
			foreach(GameObject obj in allMovingObjects){
				if(obj.GetComponent<MoveObject>() || obj.GetComponent<ObstacleSpawner>()){
					obj.SendMessage("PlayerDeath");
				}
			}

			rb.constraints = RigidbodyConstraints2D.None;
			rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			playerdead = true;
			deathSound.Play();
			mainTrack.Stop();
		}

	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.collider.CompareTag("TopBound")){
			return;
		}
		Death();
	}

}