using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script generates the field of view
//And check if anything enters the field of view with certain leyar mask
public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[Range(0,360)]
	public float viewAngle;
	public bool targetFound = false;

	public LayerMask targetMask; 
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	void Start()
	{
		StartCoroutine("FindTargetsWithDelay", 0.2f);
	}

	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true) 
		{
			yield return new WaitForSeconds (delay);
			FindVisibleTargets ();
		}
	}

	//Find any target with the target mark within the field of view
	void FindVisibleTargets()
	{
		visibleTargets.Clear ();
		Collider[] targetInVisionRadius = Physics.OverlapSphere (transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetInVisionRadius.Length; i++)
		{
			Transform target = targetInVisionRadius [i].transform;
			Vector3 directToTarget = (target.position - transform.position).normalized;
	
			if (Vector3.Angle (transform.forward, directToTarget) < viewAngle / 2) {

				float distanceToTarget = Vector3.Distance (transform.position, target.position);

				if (!Physics.Raycast (transform.position, directToTarget, distanceToTarget, obstacleMask)) 
					{
					//if there is target found, add in as a visible target
					targetFound = true;
					visibleTargets.Add (target);
					}
				}
		}

		//if the target goes out of range
		if (targetInVisionRadius.Length == 0) 
		{
			targetFound = false;
		}
	}

	//Make sure the angle of the view is at world angle
	public Vector3 directFromAngle (float angleInDegree, bool angleIsGlobal)
	{
	if (!angleIsGlobal)
	{
			angleInDegree += transform.eulerAngles.y;
	}
		return new Vector3 (Mathf.Sin (angleInDegree * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegree * Mathf.Deg2Rad));
	}
}
