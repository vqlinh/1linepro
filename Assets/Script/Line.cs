using UnityEngine;

public class Line : MonoBehaviour
{
    [HideInInspector] public bool filled;

    [SerializeField] public LineRenderer line;
    [SerializeField] public Gradient startColor;
    [SerializeField] public Gradient endColor;

    public void Init(Vector3 start, Vector3 end)
    {
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.colorGradient = startColor;
        filled = false;
    }

    public void ChangedColorLine()
    {
        filled = true;
        line.colorGradient = endColor;
        line.sortingOrder++;

    }
    public void ResetLine()
    {
        filled = false;
        line.colorGradient = startColor;
        line.sortingOrder--;

    }

}
