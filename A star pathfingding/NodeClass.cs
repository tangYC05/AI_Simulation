using System.Collections;
using UnityEngine;

public class NodeClass : IHeapItem<NodeClass>{

	//This is the clas for the node
	//Having the gcost, hcost, fcost
	//And the position of it on the world

	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;

	public NodeClass parent;
	int heapIndex;

	public NodeClass(bool _walkable, Vector3 _worldPosition, int _girdX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPosition;
		gridX = _girdX;
		gridY = _gridY;
	}


	//calculate the fcost
	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}
		
	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	//compare the fcost of nodes
	public int CompareTo(NodeClass nodeToCompare)
	{
		int compare = fCost.CompareTo (nodeToCompare.fCost);
		if (compare == 0) 
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}

		return -compare;
	}
}
