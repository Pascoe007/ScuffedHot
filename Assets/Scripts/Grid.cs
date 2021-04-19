using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public bool displayGizmos;
    public LayerMask unWalkable;
    public Vector2 worldSize;
    public float nodeRadius;
    public float nodeDiameter;
    public Terrain[] walkable;
    LayerMask walkableMask;
    Dictionary<int, int> walkableDictionary = new Dictionary<int, int>();
    
    Node[,] grid;
    int gridX, gridY;
    int pMin = int.MaxValue;
    int pMax = int.MinValue;

	[System.Serializable]
	public class Terrain
	{
		public LayerMask terrian;
		public int tPently;
	}
	public int MaxSize
	{
		get
		{
			return gridX * gridY;
		}
	}
	public Node NodeFromPos(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + worldSize.x / 2) / worldSize.x;
		float percentY = (worldPosition.z + worldSize.y / 2) / worldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridX - 1) * percentX);
		int y = Mathf.RoundToInt((gridY - 1) * percentY);
		return grid[x, y];
	}
	public List<Node> GetNeighbours(Node n)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = n.gridX + x;
				int checkY = n.gridY + y;

				if (checkX >= 0 && checkX < gridX && checkY >= 0 && checkY < gridY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}
		return neighbours;
	}
	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
		gridY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
		foreach (Terrain T in walkable)
		{
			walkableMask.value |= T.terrian.value;

			walkableDictionary.Add((int)Mathf.Log(T.terrian.value, 2), T.tPently);

		}
		CreateGrid();
	}
	void CreateGrid()
	{
		grid = new Node[gridX, gridY];
		Vector3 bottomLeft = transform.position - Vector3.right * worldSize.x / 2 - Vector3.forward * worldSize.y / 2;
		for (int x = 0; x < gridX; x++)
		{
			for (int y = 0; y < gridY; y++)
			{
				Vector3 worldPosition = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPosition, nodeRadius, unWalkable));
				int mPenlty = 0;
				if (walkable)
				{
					Ray ray = new Ray(worldPosition + Vector3.up * 50, Vector3.down);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, 100, walkableMask))
					{
						walkableDictionary.TryGetValue(hit.collider.gameObject.layer, out mPenlty);
					}
				}
				grid[x, y] = new Node(walkable, worldPosition, x, y, mPenlty);
			}
		}
		Blur(3);
	}
	void Blur(int blurSize)
	{
		int searchArea = blurSize * 2 + 1;
		int searchWidth = (searchArea - 1) / 2;
		int[,] HorizontalPass = new int[gridX, gridY];
		int[,] VerticalPass = new int[gridX, gridY];
		for (int y = 0; y < gridY; y++)
		{
			for (int x = -searchWidth; x <= searchWidth; x++)
			{
				int _X = Mathf.Clamp(x, 0, searchWidth);
				HorizontalPass[0, y] += grid[_X, y].movementPenalty;
			}
			for (int x = 1; x < gridX; x++)
			{
				int removeIndex = Mathf.Clamp(x - searchWidth - 1, 0, gridX);
				int addIndex = Mathf.Clamp(x + searchWidth, 0, gridX - 1);

				HorizontalPass[x, y] = HorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
			}
		}
		for (int x = 0; x < gridX; x++)
		{
			for (int y = -searchWidth; y <= searchWidth; y++)
			{
				int _Y = Mathf.Clamp(y, 0, searchWidth);
				VerticalPass[x, 0] += HorizontalPass[x, _Y];
			}
			int blurPenalty = Mathf.RoundToInt((float)VerticalPass[x, 0] / (searchArea * searchArea));
			grid[x, 0].movementPenalty = blurPenalty;
			for (int y = 1; y < gridY; y++)
			{
				int removeIndex = Mathf.Clamp(y - searchWidth - 1, 0, gridY);
				int addIndex = Mathf.Clamp(y + searchWidth, 0, gridY - 1);

				VerticalPass[x, y] = VerticalPass[x, y - 1] - HorizontalPass[x, removeIndex] + HorizontalPass[x, addIndex];
				blurPenalty = Mathf.RoundToInt((float)VerticalPass[x, y] / (searchArea * searchArea));
				grid[x, y].movementPenalty = blurPenalty;

				if (blurPenalty > pMax)
				{
					pMax = blurPenalty;
				}
				if (blurPenalty < pMin)
				{
					pMin = blurPenalty;
				}
			}
		}
	}
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 1, worldSize.y));
		if (grid != null && displayGizmos)
		{
			foreach (Node n in grid)
			{ 
				Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(pMin, pMax, n.movementPenalty));
				Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
			}
		}
	}
}
