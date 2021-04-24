using System.Collections.Generic;
using UnityEngine;

public class HoleMovement : MonoBehaviour
{
    [Header("Header mesh")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshCollider meshCollider;

    [Header("Hole vertices radius")]
    [SerializeField] private Vector2 moveLimits;
    [SerializeField] private float radius;
    [SerializeField] private Transform holeCenter;

    [Space]
    [SerializeField] float moveSpeed;

    private Mesh mesh;
    private List<int> holeVertices;
    private List<Vector3> offsets;
    private int holeVerticesCount;

    private float x, y;
    private Vector3 touch, targetPosition;

    private void Start()
    {
        Game.isMoving = false;
        Game.isGameover = false;
        holeVertices = new List<int>();
        offsets = new List<Vector3>();

        mesh = meshFilter.mesh;

        FindHoleVertices();
    }

    private void Update()
    {
        Game.isMoving = Input.GetMouseButton(0);

        if(!Game.isGameover && Game.isMoving)
        {
            // Move hole center
            MoveHole();
            // Update hole vertices
            UpdateHoleVerticesPosition();
        }
    }

    private void MoveHole()
    {
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        touch = Vector3.Lerp(
            holeCenter.position,
            holeCenter.position + new Vector3(x, 0f, y),
            Time.deltaTime * moveSpeed);

        targetPosition = new Vector3(
            Mathf.Clamp(touch.x, -moveLimits.x, moveLimits.x),
            touch.y,
            Mathf.Clamp(touch.z, -moveLimits.y, moveLimits.y)
            );

        holeCenter.position = targetPosition;
    }

    private void UpdateHoleVerticesPosition()
    {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < holeVerticesCount; i++)
        {
            vertices[holeVertices[i]] = holeCenter.position + offsets[i];
        }

        // update mesh
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    private void FindHoleVertices()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            float distance = Vector3.Distance(holeCenter.position, mesh.vertices[i]);

            if(distance < radius)
            {
                holeVertices.Add(i);
                offsets.Add(mesh.vertices[i] - holeCenter.position);
            }
        }
        holeVerticesCount = holeVertices.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(holeCenter.position, radius);
    }
}
