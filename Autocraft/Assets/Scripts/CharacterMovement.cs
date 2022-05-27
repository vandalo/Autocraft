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

    [SerializeField] private float m_movementSpeed = 1.0f;

    private void Awake()
    {
        m_mouseInput = new MouseInput();
        m_camera = Camera.main;
        m_pathFinding = gameObject.GetComponent<AstarPathFindingComponent>();
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
        }
    }
}
