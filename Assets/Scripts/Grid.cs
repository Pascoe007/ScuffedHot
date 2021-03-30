using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public TerrainType[] walkableRegions;
	LayerMask walkableMask;
	Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	int penaltyMin = int.MaxValue;
	int penaltyMax = int.MinValue;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        foreach (TerrainType region in walkableRegions)
        {
			walkableMask.value |= region.terrianMask.value;
			
			walkableRegionsDictionary.Add((int)Mathf.Log(region.terrianMask.value, 2), region.terrianPently);

		}
		CreateGrid();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

				int movementPenlty = 0;

                if (walkable)
                {
					Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
					RaycastHit hit;
					if(Physics.Raycast(ray,out hit, 100, walkableMask))
                    {
						walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenlty);
                    }
                }

				grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenlty);
			}
		}
		Blur(3);
	}

	void Blur(int blurSize)
	{
		int searchArea = blurSize * 2 + 1;
		int searchWidth = (searchArea - 1) / 2;

		int[,] HorizontalPass = new int[gridSizeX, gridSizeY];
		int[,] VerticalPass = new int[gridSizeX, gridSizeY];

		for (int y = 0; y < gridSizeY; y++)
		{
			for (int x = -searchWidth; x <= searchWidth; x++)
			{
				int _X = Mathf.Clamp(x, 0, searchWidth);
				HorizontalPass[0, y] += grid[_X, y].movementPenalty;
			}

			for (int x = 1; x < gridSizeX; x++)
			{
				int removeIndex = Mathf.Clamp(x - searchWidth - 1, 0, gridSizeX);
				int addIndex = Mathf.Clamp(x + searchWidth, 0, gridSizeX - 1);

				HorizontalPass[x, y] = HorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
			}
		}

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = -searchWidth; y <= searchWidth; y++)
			{
				int _Y = Mathf.Clamp(y, 0, searchWidth);
				VerticalPass[x, 0] += HorizontalPass[x, _Y];
			}

			int blurPenalty = Mathf.RoundToInt((float)VerticalPass[x, 0] / (searchArea * searchArea));
			grid[x, 0].movementPenalty = blurPenalty;

			for (int y = 1; y < gridSizeY; y++)
			{
				int removeIndex = Mathf.Clamp(y - searchWidth - 1, 0, gridSizeY);
				int addIndex = Mathf.Clamp(y + searchWidth, 0, gridSizeY - 1);

				VerticalPass[x, y] = VerticalPass[x, y - 1] - HorizontalPass[x, removeIndex] + HorizontalPass[x, addIndex];
				blurPenalty = Mathf.RoundToInt((float)VerticalPass[x, y] / (searchArea * searchArea));
				grid[x, y].movementPenalty = blurPenalty;

				if (blurPenalty > penaltyMax)
				{
					penaltyMax = blurPenalty;
				}
				if (blurPenalty < penaltyMin)
				{
					penaltyMin = blurPenalty;
				}
			}
		}

	}



	public List<Node> GetNeighbours(Node node)
	{
        List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromPos(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (grid != null && displayGridGizmos)
		{
			foreach (Node n in grid)
			{

				Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
				Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
			}
		}
	}

	[System.Serializable]
	public class TerrainType
    {
		public LayerMask terrianMask;
		public int terrianPently;
    }
}