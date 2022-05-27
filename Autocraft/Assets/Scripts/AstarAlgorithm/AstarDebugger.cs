using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarDebugger : MonoBehaviour
{
    private static AstarDebugger instance;

    public static AstarDebugger myInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AstarDebugger>();
            }
            return instance;
        }
    }

    [SerializeField] private Grid m_grid;
    [SerializeField] private Tilemap m_tilemap;
    [SerializeField] private Tile m_tile;
    [SerializeField] private Color m_openColor;
    [SerializeField] private Color m_closedColor;
    [SerializeField] private Color m_pathColor;
    [SerializeField] private Color m_currentColor;
    [SerializeField] private Color m_startColor;
    [SerializeField] private Color m_goalColor;

    public void Reset()
    {
        m_tilemap.ClearAllTiles();
    }

    public void CreateTiles(HashSet<AstarNode> _openList, HashSet<AstarNode> _closedList, Vector3Int _start, Vector3Int _goal, Stack<Vector3Int> _path = null)
    {
        foreach (AstarNode node in _openList)
        {
            ColorTile(node.m_position, m_openColor);
        }

        foreach (AstarNode node in _closedList)
        {
            ColorTile(node.m_position, m_closedColor);
        }

        if (_path != null)
        {
            foreach(Vector3Int pos in _path)
            {
                if (pos != _start && pos != _goal)
                {
                    ColorTile(pos, m_pathColor);
                }
            }
        }

        ColorTile(_start, m_startColor);
        ColorTile(_goal, m_goalColor);
    }

    public void ColorTile(Vector3Int _position, Color _color)
    {
        m_tilemap.SetTile(_position, m_tile);
        m_tilemap.SetTileFlags(_position, TileFlags.None);
        m_tilemap.SetColor(_position, _color);
    }
}
