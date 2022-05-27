using UnityEngine;

public class AstarNode
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public AstarNode m_parent { get; set; }
    public Vector3Int m_position { get; set; }

    public AstarNode(Vector3Int position)
    {
        m_position = position;
    }
}
