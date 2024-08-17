using TMPro;
using UnityEngine;

public class VertexCountSetter : MonoBehaviour
{
    public DelaunayViewer delaunayViewer;
    public TMP_InputField inputField;

    public void UpdateInputField()
    {
        inputField.text = delaunayViewer.GetPoints().ToString();
    }
    
    public void SetVertexCount(string text)
    {
        if (int.TryParse(text, out int vertexCount))
        {
            delaunayViewer.SetPoints(vertexCount);
        }
        
        UpdateInputField();
    }
}