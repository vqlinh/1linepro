using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Level",menuName ="Level")]
public class Level : ScriptableObject
{
    public List<Vector4> Points;
    public List<Vector2Int> Lines;
    public List<BezierLineData> BezierLines;
}

[System.Serializable]
public class BezierLineData
{
    public int pointA;  // Chỉ số điểm A
    public int pointB;  // Chỉ số điểm B
    public Vector3 controlPoint;  // Điểm điều khiển cho đường cong Bezier
}
