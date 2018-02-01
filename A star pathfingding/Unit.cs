using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public Transform target;

	Vector3[] path;
	int targetIndex;
	FieldOfView fow;
	//==================================================
	//Speeds
	public float speed = 0.5f;
	public float delayChaseSpeed = 0.8f;
	float currentSpeed;
	//==================================================
	//Time Delay
	float waitTime = 0;
	public float dissapearDelay = 30;
	public float reappearDelay = 30;
	public float chaseDelay = 5;
	public float backToPatrolDelay = 5;
	//==================================================
	//For the way points
	public GameObject[] waypoints;
	int num = 0;
	int currentNum;
	//==================================================

	//==================================================
	//The pursuit Point
	public GameObject pursuitPoint;
	//==================================================

	//==================================================
	// The AI States
	public enum AIStates
	{
		patrol, 
		chase,
		dissapear,
		reappear,
		pursuit,
	}

	public AIStates aiStates;
	//==================================================

	void Awake()
	{
		fow = GetComponent<FieldOfView> ();
		currentSpeed = speed;
		aiStates = AIStates.patrol;
	}

	void Update()
	{		
		switch (aiStates) {
		case AIStates.patrol:
			PatrolBehavior ();
			break;
		case AIStates.chase:
			ChaseBehavior ();
			break;
		case AIStates.dissapear:
			DissapearBehavior ();
			break;
		case AIStates.reappear:
			ReappearBehavior ();
			break;
		case AIStates.pursuit:
			PursuitBehavior ();
			break;
		}
		//PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));
	}

	//The patrol state
	void PatrolBehavior()
	{
		//resquest a path to waypoint
		PathRequestManager.RequestPath (new PathRequest (transform.position, waypoints[num].transform.position, OnPathFound));

		currentNum = num;
		float idealDistance = 5.0f;

		float distance = Vector3.Distance (transform.position, waypoints [num].transform.position);
		if (distance <= idealDistance) 
		{//When reach a point, avoidi going to the same point, then request another path to another point
			num = Random.Range (0, waypoints.Length);
			if (num != currentNum) {
				PathRequestManager.RequestPath (new PathRequest (transform.position, waypoints [num].transform.position, OnPathFound));
			}
		}
		if (fow.targetFound == true) 
		{//If found target, reset the dissapear countdown, and go into chase state
			waitTime = 0;
			CancelInvoke ();
			aiStates = AIStates.chase;
		} 
		else if (fow.targetFound == false) 
		{//If no target found, after the delay, it dissapear
			waitTime += Time.deltaTime;
			if (waitTime >= dissapearDelay) {
				waitTime = 0;
				aiStates = AIStates.dissapear;
			}
		}
	}

	//Chase state
	void ChaseBehavior()
	{//Request a poth to chase the player
		PathRequestManager.RequestPath (new PathRequest (transform.position, target.position, OnPathFound));

		if (fow.targetFound == false) 
		{	//if the player go out of field of view range, accelerate, then if the player
			//still not in range, change into pursuit state
			currentSpeed = delayChaseSpeed;
			Invoke ("InvokeChase", chaseDelay);
		}
	}

	void InvokeChase()
	{
		currentSpeed = speed;
		aiStates = AIStates.pursuit;
	}

	//DissapearBehavior
	void DissapearBehavior()
	{	//If no target found in this duration, it dissapear from the environment,
		//then reappear again
		transform.position = new Vector3 (100, 0, 100);
		waitTime += Time.deltaTime;
		if (waitTime >= reappearDelay) {
			waitTime = 0;
			aiStates = AIStates.reappear;
		}
	}

	void InvokeDisappear()
	{
		DissapearBehavior ();
	}

	//Reappear state(action)
	void ReappearBehavior()
	{	//After a delay, the enemy reappear on a random waypoint
		int random = Random.Range (0, waypoints.Length);
		transform.position = waypoints[random].transform.position;
		if(random != currentNum)
		{
		aiStates = AIStates.patrol;
		}
	}

	//pursuit state
	void PursuitBehavior ()
	{	//Request the path to go the front if the player
		PathRequestManager.RequestPath (new PathRequest (transform.position, pursuitPoint.transform.position, OnPathFound));
		if (fow.targetFound == true) 
		{
			aiStates = AIStates.chase;
		}
		else if (fow.targetFound == false) 
		{
			waitTime += Time.deltaTime;

			if (waitTime >= backToPatrolDelay) {
				waitTime = 0;
				aiStates = AIStates.patrol;
			}
		}
	}

	//When the path successfully generated, follow the path
	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful) 
		{
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	//Make sure this unit follow the path nodes that requested
	IEnumerator FollowPath()
	{
		Vector3 currentWayPoint = path [0];

		while(true)
		{
			if (transform.position == currentWayPoint) 
			{
				targetIndex++;
				if (targetIndex >= path.Length) 
				{
					yield break;
				}

				currentWayPoint = path [targetIndex];
			}
		

			transform.position = Vector3.MoveTowards (transform.position, currentWayPoint, currentSpeed);

			//Rotate AI to look forward 
			var targetRotation = Quaternion.LookRotation (currentWayPoint - transform.position);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, 10*Time.deltaTime);

			yield return null;
		}
	}

	//Draw Lines(grids) of the path
	public void OnDrawGizmos()
	{
		if (path != null) 
		{
			for (int i = targetIndex; i < path.Length; i++) 
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else
					Gizmos.DrawLine (path [i - 1], path [i]);
			}
		}
	}
		

}

