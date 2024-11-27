using UnityEngine;

public class BezierLine : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 controlPoint;

    private LineRenderer lineRenderer;

    void Awake()
    {
        // Khởi tạo LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawBezierCurve()
    {
        int lineCount = 100; // Số điểm trên đường cong Bezier
        lineRenderer.positionCount = lineCount;

        // Tính các điểm trên đường cong Bezier
        for (int i = 0; i < lineCount; i++)
        {
            float t = i / (float)(lineCount - 1);
            Vector3 pointOnCurve = GetBezierPoint(t);
            lineRenderer.SetPosition(i, pointOnCurve);
        }
    }

    // Hàm tính toán điểm trên đường cong Bezier (công thức đường Bezier bậc 2)
    Vector3 GetBezierPoint(float t)
    {
        // Tính toán điểm trên đường cong Bezier: P(t) = (1 - t)^2 * A + 2(1 - t)t * B + t^2 * C
        Vector3 point = Mathf.Pow(1 - t, 2) * startPoint + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * endPoint;
        return point;
    }
}
