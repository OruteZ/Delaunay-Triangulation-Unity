using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DelaunayTriangulationMaker
{
    private List<Triangle> _triangles;
    private List<Vector2> _points;
    
    public DelaunayTriangulationMaker()
    {
        _triangles = new List<Triangle>();
        _points = new List<Vector2>();
    }
    
    public DelaunayTriangulationMaker(List<Vector2> points)
    {
        _triangles = new List<Triangle>();
        _points = new List<Vector2>();
        
        SetInput(points);
    }
    
    public void SetInput(List<Vector2> points)
    {
        _points = points;
    }

    public List<Triangle> GetDelaunayTriangulation()
    {
        _triangles.Add(CreateSuperTriangle(_points));
        
        Vector2[] superVertices = CreateSuperTriangle(_points).GetVertices();
        
        // 정점을 하나씩 추가하면서, 삼각형을 업데이트 합니다.
        foreach (Vector2 p in _points)
        {
            // 삭제할 삼각형들을 저장할 리스트
            List<Triangle> willRemove = new List<Triangle>();
            
            // 1. 모든 삼각형을 순회하며, 외접원에 이 정점이 속하는지 체크합니다.
            foreach (Triangle t in _triangles)
            {
                // 속한다면, 이 삼각형은 사라져야 합니다.
                if (t.IsPointInsideCircumcircle(p))
                {
                    willRemove.Add(t);
                }
            }
            
            // 2. 삭제해야할 삼각형들을 삭제합니다.
            // 이때, 삼각형의 세 변중 외곽에 해당하는 한 변은 남겨서 새 삼각형을 만들어야 합니다.
            // 외곽에 해당하는 변은, 사라지는 다른 삼각형과 공유하지 않는 변입니다.
            Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>();
            foreach (Triangle t in willRemove)
            {
                foreach (Edge e in t.GetEdges())
                {
                    if (edgeCount.TryAdd(e, 1) is false)
                    {
                        edgeCount[e]++;
                    }
                }
                
                _triangles.Remove(t);
            }
            
            // 3. 새로운 삼각형을 생성합니다.
            foreach (KeyValuePair<Edge, int> pair in edgeCount)
            {
                if (pair.Value == 1)
                {
                    _triangles.Add(new Triangle(pair.Key.a, pair.Key.b, p));
                }
            }
        }
        
        // 마지막으로, 슈퍼 삼각형과 연결된 삼각형들을 삭제합니다.
        foreach (Triangle t in _triangles.ToList())
        {
            if (t.ContainsVertex(superVertices[0]) ||
                t.ContainsVertex(superVertices[1]) || 
                t.ContainsVertex(superVertices[2]))
            {
                _triangles.Remove(t);
            }
        }
        
        return _triangles;
    }

    public static Triangle CreateSuperTriangle(List<Vector2> points)
    {
        //정점들이 없으면 그냥 아무 삼각형 대충 반환
        if (points == null || points.Count == 0)
        {
            return new Triangle();
        }
        
        // X 최대최소, Y 최대최소 탐색
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (Vector2 p in points)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        // 정점들을 감쌀 충분히 큰 삼각형을 생성
        float dx = maxX - minX;
        float dy = maxY - minY;

        Vector2 v1 = new Vector2(minX - 5 * dx, minY - dy * 2);
        Vector2 v2 = new Vector2(minX + 5 * dx, minY - dy * 2);
        Vector2 v3 = new Vector2(minX + dx / 2, maxY + dy * 2);

        return new Triangle(v1, v2, v3);
    } 
}
