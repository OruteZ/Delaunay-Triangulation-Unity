using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    private Vector2 a, b, c;
    
    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    
    public Vector2[] GetVertices()
    {
        return new Vector2[] {a, b, c};
    }
    
    public bool ContainsVertex(Vector2 vertex)
    {
        return a == vertex || b == vertex || c == vertex;
    }

    public bool ContainsEdge(Edge edge)
    {
        return edge.Equals(new Edge(a, b)) || edge.Equals(new Edge(b, c)) || edge.Equals(new Edge(c, a));
    }

    public Edge[] GetEdges()
    {
        return new Edge[]
        {
            new Edge(a, b),
            new Edge(b, c),
            new Edge(c, a)
        };
    }

    /// <summary>
    /// Get Circumcircle's center of this triangle
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCircumcircleCenter()
    {
        float d = 2 * (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
        
        float ux = ((Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2)) * (b.y - c.y) +
                    (Mathf.Pow(b.x, 2) + Mathf.Pow(b.y, 2)) * (c.y - a.y) +
                    (Mathf.Pow(c.x, 2) + Mathf.Pow(c.y, 2)) * (a.y - b.y)) / d;
        float uy = ((Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2)) * (c.x - b.x) +
                    (Mathf.Pow(b.x, 2) + Mathf.Pow(b.y, 2)) * (a.x - c.x) +
                    (Mathf.Pow(c.x, 2) + Mathf.Pow(c.y, 2)) * (b.x - a.x)) / d;

        return new Vector2(ux, uy);
    }

    
    /// <summary>
    ///  Check if the point is inside the circumcircle of this triangle
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsPointInsideCircumcircle(Vector2 point)
    {
        Vector2 center = GetCircumcircleCenter();
        float radius = Vector2.Distance(center, a);
        float distance = Vector2.Distance(center, point);
        return distance < radius;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Triangle triangle = (Triangle)obj;
        
        // triangle  have every edges
        var edges = GetEdges();
        foreach (Edge edge in edges)
        {
            if (!triangle.ContainsEdge(edge))
            {
                return false;
            }
        }

        return true;
    }
}