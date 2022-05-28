using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Collision,
    Wallkable
}

public class AstarPathFindingComponent : MonoBehaviour
{
    private TileType m_tileType;

    [SerializeField] private Tilemap tilemap;

    private HashSet<AstarNode> m_openList;
    private HashSet<AstarNode> m_closedList;
    private Dictionary<Vector3Int, AstarNode> m_allNodes = new Dictionary<Vector3Int, AstarNode>();
    private Vector3Int m_startPosition;
    private Vector3Int m_goalPosition;
    private AstarNode m_currentNode;
    private Stack<Vector3Int> m_path;
    private bool m_hasPath;

    public void Initialize(Vector3Int _start, Vector3Int _goal)
    {
        if (Defines.myInstance.EnableDebug)
        {
            AstarDebugger.myInstance.Reset();
        }
        m_startPosition = _start;
        m_goalPosition = _goal;
        m_currentNode = GetNode(m_startPosition);
        m_openList = new HashSet<AstarNode>();
        m_closedList = new HashSet<AstarNode>();
        m_openList.Clear();
        m_closedList.Clear();
        if (m_path != null)
        {
            m_path.Clear();
        }
        m_path = null;
        m_hasPath = false;
        //Add start to the open list
        m_openList.Add(m_currentNode);

        StartCoroutine("Algorithm");
    }

    IEnumerator Algorithm()
    {
        while (m_openList.Count > 0 && m_path == null)
        {
            List<AstarNode> neighbors = FindNeighbors(m_currentNode.m_position);
            CheckNeighbors(neighbors, m_currentNode);
            UpdateCurrentTile(ref m_currentNode);

            m_path = GeneratePath();
            yield return null;
        }
        m_hasPath = true;
        AstarDebugger.myInstance.CreateTiles(m_openList, m_closedList, m_startPosition, m_goalPosition, m_path);
    }

    private List<AstarNode> FindNeighbors(Vector3Int _parentPosition)
    {
        List<AstarNode> neighbors = new List<AstarNode>();

        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                Vector3Int neighborPos = new Vector3Int(_parentPosition.x - x, _parentPosition.y - y, _parentPosition.z);
                if (y != 0 || x != 0)
                {
                    if (neighborPos != m_startPosition && tilemap.GetTile(neighborPos))
                    {
                        neighbors.Add(GetNode(neighborPos));
                    }
                }
            }
        }
        return neighbors;
    }

    private void CheckNeighbors(List<AstarNode> _neighbors, AstarNode _currentNode)
    {
        for (int i = 0; i < _neighbors.Count; ++i)
        {
            AstarNode neighbor = _neighbors[i];
            int gScore = GetGScore(_neighbors[i].m_position, _currentNode.m_position);

            if (m_openList.Contains(neighbor))
            {
                if (m_currentNode.G + gScore < neighbor.G)
                {
                    CalculateValues(m_currentNode, neighbor, gScore);
                }
            }
            else if (!m_closedList.Contains(neighbor))
            {
                CalculateValues(m_currentNode, neighbor, gScore);
                m_openList.Add(neighbor);
            }
        }
    }

    private void CalculateValues(AstarNode _parent, AstarNode _neighbor, int _cost)
    {
        _neighbor.m_parent = _parent;

        _neighbor.G = _parent.G + _cost;
        _neighbor.H = (Mathf.Abs(_neighbor.m_position.x - m_goalPosition.x) + Mathf.Abs(_neighbor.m_position.y - m_goalPosition.y)) * 10;
        _neighbor.F = _neighbor.G + _neighbor.H;
    }

    private int GetGScore(Vector3Int _neighbor, Vector3Int _currentNode)
    {
        int gScore = 0;
        int x = _currentNode.x - _neighbor.x;
        int y = _currentNode.y - _neighbor.y;

        if (Mathf.Abs(x - y) % 2 == 1)
        {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }

        return gScore;
    }

    private void UpdateCurrentTile(ref AstarNode m_currentNode)
    {
        m_openList.Remove(m_currentNode);

        m_closedList.Add(m_currentNode);

        if (m_openList.Count > 0)
        {
            m_currentNode = m_openList.OrderBy(x => x.F).First();
        }
    }

    private AstarNode GetNode(Vector3Int _position)
    {
        if (m_allNodes.ContainsKey(_position))
        {
            return m_allNodes[_position];
        }
        else
        {
            AstarNode node = new AstarNode(_position);
            m_allNodes.Add(_position, node);
            return node;
        }
    }

    private Stack<Vector3Int> GeneratePath()
    {
        if (m_currentNode.m_position == m_goalPosition)
        {
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();

            while (m_currentNode.m_position != m_startPosition)
            {
                finalPath.Push(m_currentNode.m_position);
                m_currentNode = m_currentNode.m_parent;
            }
            return finalPath;
        }
        return null;
    }

    public bool HasPath()
    {
        return m_hasPath;
    }

    public Stack<Vector3Int> GetPath()
    {
        if (HasPath())
        {
            return m_path;
        }
        return new Stack<Vector3Int>();
    }
}
