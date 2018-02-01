using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMouseLock : MonoBehaviour {

	Vector2 mouseAngle;
	Vector2 tiltAngle;
	public float sensitivity = 5.0f;
	public float smoothing = 2.0f;
	GameObject character;

	void Start () {
		character = this.transform.parent.gameObject;
	}
	
	//Camera rotation
	void Update () {
		var md = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		md = Vector2.Scale (md, new Vector2 (sensitivity * smoothing, sensitivity * smoothing));
		tiltAngle.x = Mathf.Lerp (tiltAngle.x, md.x, 1f / smoothing);
		tiltAngle.y = Mathf.Lerp (tiltAngle.y, md.y, 1f / smoothing);
		mouseAngle += tiltAngle;

		mouseAngle.y = Mathf.Clamp (mouseAngle.y, -90f, 90f);

		transform.localRotation = Quaternion.AngleAxis (-mouseAngle.y, Vector3.right);
		character.transform.localRotation = Quaternion.AngleAxis (mouseAngle.x, character.transform.up);
		
	}
}
