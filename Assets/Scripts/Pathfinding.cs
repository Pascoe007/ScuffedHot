using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour
{
    Grid grid;
    PathReqManager manager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        manager = GetComponent<PathReqManager>();
    }

    public void StartPathFinding(Vector3 sPos, Vector3 tPos)
    {
        StartCoroutine(PathFinding(sPos, tPos));
    }

    Vector3[] RetracePath(Node _sNode, Node eNode)
    {
        List<Node> _path = new List<Node>();
        Node _cNode = eNode;
        while(_cNode != _sNode)
        {
            
            _path.Add(_cNode);
            _cNode = _cNode.parent;
        }
        Vector3[] finalPath = Simlify(_path);
        Array.Reverse(finalPath);
        return finalPath;
    }


    Vector3[] Simlify(List<Node> _path)
    {
        List<Vector3> pathPoints = new List<Vector3>();
        Vector2 oldDir = Vector2.zero;

        for (int i = 1; i < _path.Count; i++)
        {
            Vector2 newDir = new Vector2(_path[i - 1].gridX - _path[i].gridX, _path[i - 1].gridY - _path[i].gridY);
            if (oldDir != newDir)
            {
                pathPoints.Add(_path[i].worldPosition);
            }
            oldDir = newDir;
        }
        return pathPoints.ToArray();
    }


    int Distance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);

        if (dx > dy)
            return 14 * dy + 10 * (dx - dy);
        return 14 * dx + 10 * (dy - dx);
    }

    IEnumerator PathFinding(Vector3 _sPos, Vector3 _tPos)
    {

        bool pathSuccess = false;
        Vector3[] path = new Vector3[0];

        Node sNode = grid.NodeFromPos(_sPos);
        Node tNode = grid.NodeFromPos(_tPos);

        sNode.parent = sNode;

        if (sNode.walkable && tNode.walkable)
        {

            Heap<Node> openHeap = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedHashSet = new HashSet<Node>();

            openHeap.Add(sNode);

            while (openHeap.Count > 0)
            {

                Node cNode = openHeap.RemoveFirst();
                closedHashSet.Add(cNode);


                if (cNode == tNode)
                {
                    pathSuccess = true;
                    break;
                }
                foreach (Node n in grid.GetNeighbours(cNode))
                {
                    if (closedHashSet.Contains(n) || !n.walkable)
                    {
                        continue;
                    }
                    int costToN = cNode.gCost + Distance(cNode, n) * n.movementPenalty;

                    if (!openHeap.Contains(n) || n.gCost > costToN)
                    {
                        n.gCost = costToN;
                        n.hCost = Distance(n, tNode);
                        n.parent = cNode;

                        if (!openHeap.Contains(n))
                        {
                            openHeap.Add(n);
                        }
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
        {
            path = RetracePath(sNode, tNode);
            pathSuccess = path.Length > 0;
        }
        manager.FinishedProcessingPath(path, pathSuccess);
    }

}