using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{
    public Tilemap map;
    MouseInput m_mouseInput;
    Camera m_camera;
    private Vector3 m_destination;
    private AstarPathFindingComponent m_pathFinding;

    Stack<Vector3Int> m_pathToFollow;
    private Vector3 m_tempDestinationTilePosition;
    private bool m_hasNewPathRequest;

    [SerializeField] private float m_movementSpeed = 1.0f;

    private void Awake()
    {
        m_mouseInput = new MouseInput();
        m_camera = Camera.main;
        m_pathFinding = gameObject.GetComponent<AstarPathFindingComponent>();
        m_pathToFollow = new Stack<Vector3Int>();
        m_hasNewPathRequest = true;
    }

    private void OnEnable()
    {
        if (m_mouseInput != null)
        {
            m_mouseInput.Enable();
        }
    }

    private void OnDisable()
    {
        if (m_mouseInput != null)
        {
            m_mouseInput.Disable();
        }
    }

    void Start()
    {
        m_destination = transform.position;
        m_mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
    }

    void Update()
    {
        if (m_hasNewPathRequest && m_pathFinding.HasPath())
        {
            m_pathToFollow = m_pathFinding.GetPath();
            if (m_pathToFollow.Count > 0)
            {
                m_tempDestinationTilePosition = map.CellToWorld(m_pathToFollow.Pop());
            }
            m_hasNewPathRequest = false;
        }
        if (Vector3.Distance(transform.position, m_destination) > 0.1)
        {
            Move();
        }
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_tempDestinationTilePosition, m_movementSpeed * Time.deltaTime);
        if (m_pathToFollow.Count > 0 && Vector3.Distance(transform.position, m_tempDestinationTilePosition) < 0.1)
        {
            m_tempDestinationTilePosition = map.CellToWorld(m_pathToFollow.Pop());
        }
    }

    private void MouseClick()
    {
        Vector3 mousePosition = m_mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition.z = m_camera.transform.position.z * (-1);
        mousePosition = m_camera.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        //We make sure we click inside the map
        if (map.HasTile(gridPosition))
        {
            Vector3Int startPosition = map.WorldToCell(transform.position);
            m_destination = mousePosition;
            m_pathFinding.Initialize(startPosition, gridPosition);
            m_pathToFollow.Clear();
            m_hasNewPathRequest = true;
        }
    }
}
