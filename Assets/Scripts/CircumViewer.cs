using System.Collections.Generic;
using UnityEngine;

public class CircumViewer : MonoBehaviour
{
    DelaunayViewer delaunayViewer;
    
    private List<GameObject> circumStances;
    
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _triangleWidth = 0.2f;
    [SerializeField] private float _circumcircleWidth = 0.2f;
    [SerializeField] private Color _circleColor = Color.red;

    private bool _drawTrigger;
    
    void Start()
    {
        delaunayViewer = GetComponent<DelaunayViewer>();
        circumStances = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            _drawTrigger = !_drawTrigger;
            if (_drawTrigger)
            {
                DrawCircumcircles(delaunayViewer.GetTriangles());
            }
            else
            {
                ClearCircumcircles();
            }
        }
    }

    private void DrawCircumcircles(Triangle[] triangles)
    {
        foreach (var triangle in triangles)
        {
            DrawCircumcircle(triangle, _circleColor);
        }
    }

    public void DrawCircumcircle(Triangle t, Color color)
    {
        Vector2 center = t.GetCircumcircleCenter();
        float radius = Vector2.Distance(center, t.GetVertices()[0]);
        
        // use line renderer
        LineRenderer lr = new GameObject("Circumcircle").AddComponent<LineRenderer>();
        
        int pointCount = 100;
        lr.positionCount = pointCount + 1;
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pointCount;
            lr.SetPosition(i, new Vector3(
                center.x + Mathf.Cos(angle) * radius, 
                center.y + Mathf.Sin(angle) * radius, 
                0));
        }
        lr.SetPosition(pointCount, lr.GetPosition(0));
        
        // material
        lr.material = _lineMaterial;
        lr.startWidth = _circumcircleWidth;
        lr.endWidth = _circumcircleWidth;
        
        lr.startColor = color;
        lr.endColor = color;
        
        circumStances.Add(lr.gameObject);
    }
    
    public void ClearCircumcircles()
    {
        foreach (var circum in circumStances)
        {
            Destroy(circum);
        }
        circumStances.Clear();
    }

}