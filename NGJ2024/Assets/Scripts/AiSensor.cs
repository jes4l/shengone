using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    public float distance = 3;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = new Color(1, 0, 0, 0.8f);
    public int scanFrequency = 10;
    public LayerMask layers;
    public List<GameObject> Objects = new List<GameObject>();
    public LayerMask OcculsionLayers;
    Collider[] colliders = new Collider[50];
    int count;
    float scanInterval;
    float scanTimer;

    Mesh mesh;

    public struct Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Vector3 WorldLocation { get; set; }
        public string Direction { get; set; }
        public Location(int x, int y, Vector3 worldLocation, string direction)
        {
            X = x;
            Y = y;
            WorldLocation = worldLocation;
            Direction = direction;
        }
    }
    List<Location> locations = new List<Location>
    {

    };
    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            scan();
        }
    }


    private void scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                Objects.Add(obj);

            }
        }

        if (count > 0)
        {
            return;
        }
    }
    Mesh createWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        //Left Side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //Right Side 
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {

            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;
            //Far Side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //Top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //Bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topCenter;
            currentAngle += deltaAngle;
        }


        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public bool IsInSight(GameObject obj)
    {

        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < -height || direction.y > height)
        {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            return false;
        }

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, OcculsionLayers))
        {
            return false;
        }
        return true;
    }
    private void OnValidate()
    {
        mesh = createWedgeMesh();

    }



    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Vector3 pos = transform.position;
            pos.y = pos.y - 0.5f;
            Gizmos.DrawMesh(mesh, pos, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);

        for (int i = 0; i < count; ++i)
        {
            if (colliders[i] != null)
                Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);

        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects)
        {

            if (obj != null)
                Gizmos.DrawSphere(obj.transform.position, 0.2f);

        }
    }
}
