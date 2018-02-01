using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

//This script is use to handle the pathfindig for
//each of the objects if there is more than one object

public class PathRequestManager : MonoBehaviour {

	Queue<pathResult> results = new Queue<pathResult> ();

	static PathRequestManager instance;
	PathFinding pathFinding;


	void Awake()
	{
		instance = this;
		pathFinding = GetComponent<PathFinding> ();
	}

	void Update()
	{
		if (results.Count > 0) 
		{
			int itemsInQueue = results.Count;
			lock (results) 
			{
				for (int i = 0; i < itemsInQueue; i++) 
				{
					pathResult result = results.Dequeue ();
					result.callback (result.path, result.isSuccess);
				}
			}
		}
	}

	//request a pathfinding
	public static void RequestPath(PathRequest request)
	{
		ThreadStart threadStart = delegate {
			instance.pathFinding.FindPath (request, instance.FinishProcessingPath);
		};
		threadStart.Invoke ();
	}

	//queue the node when find the path success
	public void FinishProcessingPath(pathResult result)
	{
		lock (results) {
			results.Enqueue (result);
		}
	}
}


//structure for request path
public struct PathRequest
{
	public Vector3 pathStart;
	public Vector3 pathEnd;
	public Action<Vector3[], bool> callback;

	public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
	{
		pathStart = _start;
		pathEnd = _end;
		callback = _callback;
	}
}

public struct pathResult {
	public Vector3[] path;
	public bool isSuccess;
	public Action<Vector3[], bool> callback;

	public pathResult(Vector3[] path, bool isSuccess, Action<Vector3[], bool> _callback)
	{
		this.path = path;
		this.isSuccess = isSuccess;
		this.callback = _callback;
	}
}