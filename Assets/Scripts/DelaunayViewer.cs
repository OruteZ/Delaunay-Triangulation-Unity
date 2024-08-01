using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DelaunayViewer : MonoBehaviour
{
    public void SetPoints(int pointCount)
    {
        _pointCount = pointCount;
    }
    
    public int GetPoints()
    {
        return _pointCount;
    }
    
    [SerializeField] private Vector2 _size = new Vector2(10, 10);
    [SerializeField] private int _pointCount = 10;
    
    [SerializeField] private GameObject _dotPrefab;
    
    [Header("Line Renderer Options")]
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private float _triangleWidth = 0.2f;
    [SerializeField] private float _circumcircleWidth = 0.2f;
    
    [Header("Colors")]
    [SerializeField] private Material enableDot;
    [SerializeField] private Material disableDot;
    
    //====================================================================================================
    
    
    Dictionary<Triangle, GameObject[]> triangleView = new Dictionary<Triangle, GameObject[]>();
    Dictionary<Triangle, GameObject[]> circumcircleView = new Dictionary<Triangle, GameObject[]>();
    Dictionary<Edge, GameObject[]> edgeView = new Dictionary<Edge, GameObject[]>();
    
    // Start is called before the first frame update

    private List<GameObject> _dots;
    IEnumerator<bool> visualizingTriangulation;

    private bool startTrigger = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && startTrigger is false)
        {
            _dots = CreatePoints(GeneratePoints());
            visualizingTriangulation = VisualizingTriangulation(_dots);
            startTrigger = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && startTrigger)
        {
            visualizingTriangulation.MoveNext();
        }

        if (Input.GetKeyDown(KeyCode.Return) && startTrigger)
        {
            visualizingTriangulation.MoveNext();
            bool ret = visualizingTriangulation.Current;
            
            int cnt = 1;
            while (!ret)
            {
                if (!visualizingTriangulation.MoveNext()) break;
                ret = visualizingTriangulation.Current;
                cnt++;
            }
            
            Debug.Log(cnt);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    private void Reset()
    {
        foreach (var pair in triangleView)
        {
            foreach (GameObject go in pair.Value)
            {
                Destroy(go);
            }
        }
        
        foreach (var pair in circumcircleView)
        {
            foreach (GameObject go in pair.Value)
            {
                Destroy(go);
            }
        }
        
        foreach (var pair in edgeView)
        {
            foreach (GameObject go in pair.Value)
            {
                Destroy(go);
            }
        }
        
        foreach (GameObject go in _dots)
        {
            Destroy(go);
        }
        
        triangleView.Clear();
        circumcircleView.Clear();
        edgeView.Clear();
        _dots.Clear();
        
        visualizingTriangulation = null;
        startTrigger = false;
    }

    private List<Vector2> GeneratePoints()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < _pointCount; i++)
        {
            float x = Random.Range(-_size.x, _size.x);
            float y = Random.Range(-_size.y, _size.y);
            points.Add(new Vector2(x, y));
        }

        return points;
    }

    private List<GameObject> CreatePoints(List<Vector2> points)
    {
        var dots = new List<GameObject>();
        
        foreach (Vector2 p in points)
        {
            GameObject go = Instantiate(_dotPrefab, p, Quaternion.identity);
            dots.Add(go);
            
            go.GetComponent<Renderer>().material = disableDot;
        }
        
        return dots;
    }

    public void DrawTriangles(List<Triangle> triangles, Color color)
    {
        foreach (Triangle t in triangles)
        {
            DrawTriangle(t, color);
        }
    }

    public void DrawTriangle(Triangle t, Color color, int priority = 0)
    {
        if (triangleView.TryGetValue(t, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
        
        foreach (Edge e in t.GetEdges())
        {
            // use line renderer
            LineRenderer lr = new GameObject("Edge").AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPositions(new Vector3[]
            {
                e.a, e.b
            });
            
            // material
            lr.material = _lineMaterial;
            lr.startWidth = _triangleWidth;
            lr.endWidth = _triangleWidth;
            
            lr.startColor = color;
            lr.endColor = color;
            
            // set priority
            lr.sortingOrder = priority;
            
            if(triangleView.ContainsKey(t) is false)
            {
                triangleView.Add(t, new GameObject[] {lr.gameObject});
            }
            else
            {
                triangleView[t] = triangleView[t].Append(lr.gameObject).ToArray();
            }
        }
    }
    
    public void RemoveTriangle(Triangle t)
    {
        if (triangleView.TryGetValue(t, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
    }

    public void DrawCircumcircles(List<Triangle> triangles, Color color)
    {
        if (circumcircleView.TryGetValue(triangles[0], out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
        
        foreach (Triangle t in triangles)
        {
            Vector2 center = t.GetCircumcircleCenter();
            float radius = Vector2.Distance(center, t.GetVertices()[0]);
            
            // use line renderer
            LineRenderer lr = new GameObject("Circumcircle").AddComponent<LineRenderer>();
            
            int pointCount = 100;
            lr.positionCount = pointCount;
            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * Mathf.PI * 2 / pointCount;
                lr.SetPosition(i, new Vector3(center.x + Mathf.Cos(angle) * radius, center.y + Mathf.Sin(angle) * radius, 0));
            }
            
            // material
            lr.material = _lineMaterial;
            lr.startWidth = _circumcircleWidth;
            lr.endWidth = _circumcircleWidth;
            
            
            lr.startColor = color;
            lr.endColor = color;
            
            if(circumcircleView.ContainsKey(t) is false)
            {
                circumcircleView.Add(t, new GameObject[] {lr.gameObject});
            }
            else
            {
                circumcircleView[t] = circumcircleView[t].Append(lr.gameObject).ToArray();
            }
        }
    }

    public void DrawCircumcircle(Triangle t, Color color)
    {
        if (circumcircleView.TryGetValue(t, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
        
        Vector2 center = t.GetCircumcircleCenter();
        float radius = Vector2.Distance(center, t.GetVertices()[0]);
        
        // use line renderer
        LineRenderer lr = new GameObject("Circumcircle").AddComponent<LineRenderer>();
        
        int pointCount = 100;
        lr.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pointCount;
            lr.SetPosition(i, new Vector3(center.x + Mathf.Cos(angle) * radius, center.y + Mathf.Sin(angle) * radius, 0));
        }
        
        // material
        lr.material = _lineMaterial;
        lr.startWidth = _circumcircleWidth;
        lr.endWidth = _circumcircleWidth;
        
        lr.startColor = color;
        lr.endColor = color;
        
        if(circumcircleView.ContainsKey(t) is false)
        {
            circumcircleView.Add(t, new GameObject[] {lr.gameObject});
        }
        else
        {
            circumcircleView[t] = circumcircleView[t].Append(lr.gameObject).ToArray();
        }
    }
    
    public void RemoveCircumcircle(Triangle t)
    {
        if (circumcircleView.TryGetValue(t, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
    }
    
    public void DrawEdges(List<Edge> edges, Color color)
    {
        foreach (Edge e in edges)
        {
            DrawEdge(e, color);
        }
    }

    private void DrawEdge(Edge edge, Color color)
    {
        if (edgeView.TryGetValue(edge, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
        
        // use line renderer
        LineRenderer lr = new GameObject("Edge").AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[]
        {
            edge.a, edge.b
        });
        
        // material
        lr.material = _lineMaterial;
        lr.startWidth = _triangleWidth;
        lr.endWidth = _triangleWidth;
        
        lr.startColor = color;
        lr.endColor = color;
        
        if(edgeView.ContainsKey(edge) is false)
        {
            edgeView.Add(edge, new GameObject[] {lr.gameObject});
        }
        else
        {
            edgeView[edge] = edgeView[edge].Append(lr.gameObject).ToArray();
        }
    }
    
    public void RemoveEdge(Edge edge)
    {
        if (edgeView.TryGetValue(edge, out var goList))
        {
            foreach (GameObject go in goList)
            {
                Destroy(go);
            }
        }
    }

    public IEnumerator<bool> VisualizingTriangulation(List<GameObject> dots)
    {
        List<Triangle> result = new ();

        List<Vector2> points = dots.Select(dot => dot.transform.position).Select(dummy => (Vector2)dummy).ToList();
        result.Add(DelaunayTriangulation.CreateSuperTriangle(points));
        
        Vector2[] superVertices = result[0].GetVertices();

        // draw 3 vertices of super triangle
        DrawTriangle(result[0], Color.green);

        yield return false;
        
        
        // 정점을 하나씩 추가하면서, 삼각형을 업데이트 합니다.
        for (int i = 0; i < dots.Count; i++)
        {
            GameObject dot = dots[i];
            Vector2 p = new Vector2(
                dot.transform.position.x,
                dot.transform.position.y
            );
            dot.GetComponent<Renderer>().material = enableDot;

            yield return false;
            
            
            // 삭제할 삼각형들을 저장할 리스트
            List<Triangle> willRemove = new List<Triangle>();

            // 1. 모든 삼각형을 순회하며, 외접원에 이 정점이 속하는지 체크합니다.
            foreach (Triangle t in result)
            {
                DrawTriangle(t, Color.yellow, 2);
                DrawCircumcircle(t, Color.black);

                yield return false;

                RemoveCircumcircle(t);
                // 속한다면, 이 삼각형은 사라져야 합니다.
                if (t.IsPointInsideCircumcircle(p))
                {
                    willRemove.Add(t);
                    
                    // draw red triangle
                    DrawTriangle(t, Color.red, 1);
                }
                else
                {
                    DrawTriangle(t, Color.green,0);
                }

                yield return false;
            }

            // 2. 삭제해야할 삼각형들을 삭제합니다.
            // 이때, 삼각형의 세 변중 외곽에 해당하는 한 변은 남겨서 새 삼각형을 만들어야 합니다.
            // 외곽에 해당하는 변은, 사라지는 다른 삼각형과 공유하지 않는 변입니다.
            Dictionary<Edge, int> edgeCount = new Dictionary<Edge, int>();
            foreach (Triangle t in willRemove)
            {
                bool isShared = true;
                foreach (Edge e in t.GetEdges())
                {
                    if (edgeCount.TryAdd(e, 1) is false)
                    {
                        edgeCount[e]++;
                    }
                    else
                    {
                        isShared = false;
                    }
                }

                result.Remove(t);
                RemoveTriangle(t);
            }

            // 3. 새로운 삼각형을 생성합니다.
            foreach (KeyValuePair<Edge, int> pair in edgeCount)
            {
                if (pair.Value == 1)
                {
                    result.Add(new Triangle(pair.Key.a, pair.Key.b, p));
                    DrawTriangle(result.Last(), Color.green);
                }
                
            }

            yield return true;
        }

        // 마지막으로, 슈퍼 삼각형과 연결된 삼각형들을 삭제합니다.
        yield return false;
        
        foreach (Triangle t in result.ToList())
        {
            if (t.ContainsVertex(superVertices[0]) ||
                t.ContainsVertex(superVertices[1]) || 
                t.ContainsVertex(superVertices[2]))
            {
                
                DrawTriangle(t, Color.red, 1);
                yield return false;
                
                result.Remove(t);
                RemoveTriangle(t);
            }
        }
        
        yield return true;
    }
}
