using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This function is use to divide the world space into grids for path finding

public class Grid : MonoBehaviour {

	public bool displayGridGizmoz;

	public float size = 0.5f;

	public Transform player;
	public LayerMask unwalkableMask;
	public LayerMask wayPoints;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	NodeClass[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);
		CreateGrid ();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	//==========================================================================================
	//This function is use to divide the world space and 
	//get the point of each grids that is not on the layer mask
	void CreateGrid()
	{
		grid = new NodeClass[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++) 
		{
			for (int y = 0; y < gridSizeY; y++) 
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				Vector3 box_size = new Vector3(size, size, size);
				Quaternion someVect = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
				bool walkable = !(Physics.CheckBox (worldPoint, box_size, someVect, unwalkableMask));
				grid [x, y] = new NodeClass (walkable, worldPoint, x, y);
			}
		}
	}
	//=========================================================================================

	//=========================================================================================
	//Calculate the surrounding nodes for more efficient/lower cost path
	public List<NodeClass> GetNeighnours(NodeClass node)
	{
		List<NodeClass> neighbours = new List<NodeClass>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0) 
					continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) 
					{
						neighbours.Add (grid [checkX, checkY]);
					}
				}
		}
		return neighbours;
	}
	//========================================================================================
	
	//========================================================================================
	//Return the nodes from the grid created
	public NodeClass NodeFromWorldPoint(Vector3 worldPosition)
	{
		//Calculate how fsr the grid is by percentage
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		//get and return the axis of the grid
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x, y];
	}
	//=======================================================================================


	//=======================================================================================
	//Draw Grids
	void OnDrawGizmos ()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, 1, gridWorldSize.y));
			
		if (grid != null && displayGridGizmoz) {
				NodeClass playerNode = NodeFromWorldPoint (player.position);
				foreach (NodeClass n in grid) {
				Gizmos.color = (n.walkable)?Color.white:Color.red;
					Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
	//========================================================================================
}


