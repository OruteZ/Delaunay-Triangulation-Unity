using UnityEngine;

public struct Edge
{
    public Vector2 a, b;

    public Edge(Vector2 a, Vector2 b)
    {
        this.a = a;
        this.b = b;
    }
    
    public bool Equals(Edge edge)
    {
        return (a == edge.a && b == edge.b) || (a == edge.b && b == edge.a);
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        Edge edge = (Edge)obj;
        return Equals(edge);
    }
    
    public override int GetHashCode()
    {
        return a.GetHashCode() ^ b.GetHashCode();
    }
}