using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//This is the player controller script

	public float speed = 0.2f;
	public float speedUp = 0.5f;
	private float gravity = -9.8f;
	CharacterController playerCtrlr;

	public LayerMask pursuitPoint;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		playerCtrlr = GetComponent<CharacterController> ();
	}

	//This is the first person player controller
	void Update () {

		float deltaX = Input.GetAxis ("Horizontal")* speed;
		float deltaY = Input.GetAxis ("Vertical") * speed;

		Vector3 movement = new Vector3 (deltaX, gravity, deltaY);
		playerCtrlr.Move (transform.TransformDirection(movement));

		if (Input.GetKeyDown ("escape"))
			Cursor.lockState = CursorLockMode.None;

		if (Input.GetKeyDown (KeyCode.LeftShift))
			speed = speedUp;

		if (Input.GetKeyUp (KeyCode.LeftShift))
			speed = 0.2f;	


		var projectedLookDirection = Vector3.ProjectOnPlane (transform.forward, Vector3.up);
		transform.rotation =  Quaternion.LookRotation (projectedLookDirection);
		
	}

}

