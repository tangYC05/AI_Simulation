using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour {

	Grid grid;

	void Awake()
	{
		grid = GetComponent<Grid> ();
	}

	//===================================================================================
	//Find the path to target
	public void FindPath (PathRequest request, Action<pathResult>callback)
	{
		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Vector3[] wayPoints = new Vector3[0];
		bool pathSuccess = false;

		//The startpoint and  the target point
		NodeClass startNode = grid.NodeFromWorldPoint (request.pathStart);
		NodeClass targetNode = grid.NodeFromWorldPoint (request.pathEnd);

		if (startNode.walkable && targetNode.walkable) {

			Heap<NodeClass> openSet = new Heap<NodeClass> (grid.MaxSize);
			HashSet<NodeClass> closedSet = new HashSet<NodeClass> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				NodeClass node = openSet.removeFirst ();

				closedSet.Add (node);

				if (node == targetNode) {
					sw.Stop ();
					//print ("Path Found: " + sw.ElapsedMilliseconds + "ms");
					pathSuccess = true;
					RetracePath (startNode, targetNode);
					break;
				}
				//store the node that is more cheaper into the open set
				foreach (NodeClass neighbour in grid.GetNeighnours(node)) {
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = node.gCost + GetDistance (node, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = node;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						} else
							openSet.UpdateItem (neighbour);
					}
				}
			}
		}

		if (pathSuccess) 
		{
			wayPoints = RetracePath (startNode, targetNode);
			pathSuccess = wayPoints.Length > 0;
		}
		callback(new pathResult(wayPoints, pathSuccess, request.callback));}
	//==========================================================================================

	//if it reach the end node, recalculate the node from start to target
	Vector3[] RetracePath(NodeClass startNode, NodeClass endNode)
	{
		List<NodeClass> path = new List<NodeClass>();
		NodeClass currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		//only retrace when the direction changed
		Vector3[] wayPoints = SimplifyPath (path);
		Array.Reverse (wayPoints);
		return wayPoints;

	}

	//the simplfied path to be retraced
	Vector3[] SimplifyPath(List<NodeClass> path)
	{
		List<Vector3> wayPoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) 
		{
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) 
			{
				wayPoints.Add (path [i].worldPosition);
			}
			directionOld = directionNew;

		}
		return wayPoints.ToArray();
	}
		
	//get the distance between two nodes
	int GetDistance (NodeClass nodeA, NodeClass nodeB)
	{
		int disX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int disY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (disX > disY) 
		{
			return 14 * disY + 10 * (disX - disY);
		}
			return 14 * disX + 10 * (disY - disX);
	}
}
