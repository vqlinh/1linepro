using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;


public class RfHolder : Singleton<RfHolder>
{
    [SerializeField] private Line linePrefab;
    [SerializeField] private Point pointPrefab;
    [SerializeField] private List<Level> levels;
    private Dictionary<Vector2Int, Line> lines;
    private Dictionary<int, Point> points;
    private GameObject panelLevel;
    public TextMeshProUGUI txtNumberLv;
    public GameObject panelMessageLocked;
    private int startIndex = 0;
    private bool fingerMoving = false;
    private GameObject finger;
    private Level levelStart;
    private void Start()
    {
        finger = GameObject.Find("Finger");
        panelLevel = GameObject.Find("PanelLevel");
        finger.SetActive(false);
        panelLevel.SetActive(false);
        lines = new Dictionary<Vector2Int, Line>();
        points = new Dictionary<int, Point>();
        levelStart = levels[0];
        LevelStart(levelStart);
        txtNumberLv.text = (LevelButton.Instance.nextLevel+1).ToString();
        InvokeRepeating("Hint", 1f, 2f);
    }

    private void LevelStart(Level level)
    {
        for (int i = 0; i < level.Points.Count; i++)
        {
            Vector4 posData = level.Points[i];
            Vector3 spawnPos = new Vector3(posData.x, posData.y, posData.z);
            int id = (int)posData.w;
            points[id] = Instantiate(pointPrefab);
            points[id].Init(spawnPos, id);
        }

        for (int i = 0; i < level.Lines.Count; i++)
        {
            Vector2Int normal = level.Lines[i];
            Vector2Int reversed = new Vector2Int(normal.y, normal.x);
            Line spawnLine = Instantiate(linePrefab);
            lines[normal] = spawnLine;
            lines[reversed] = spawnLine;
            spawnLine.Init(points[normal.x].Position, points[normal.y].Position);
        }
    }

    public void Hint()
    {
        if (fingerMoving) return;
        fingerMoving = true;
        finger.SetActive(true);
        Sequence sequence = DOTween.Sequence();

        finger.transform.position = levelStart.Points[Mathf.Min(startIndex, levelStart.Points.Count - 1)];

        int endIndex = Mathf.Min(startIndex + 4, levelStart.Lines.Count);
        for (int i = startIndex; i < endIndex; i++)
        {
            Vector2Int line = levelStart.Lines[i];
            Vector3 startPosition = points[line.x].Position;
            Vector3 endPosition = points[line.y].Position;
            sequence.Append(finger.transform.DOMove(startPosition, 0));
            sequence.Append(finger.transform.DOMove(endPosition, 0.6f).SetEase(Ease.Linear));
        }
        startIndex = endIndex;
        sequence.Append(finger.transform.DOScale(0.8f, 0.2f).SetLoops(2, LoopType.Yoyo));
        sequence.AppendCallback(() =>
        {
            finger.SetActive(false);
            fingerMoving = false;
        });

        if (startIndex >= levelStart.Lines.Count) startIndex = 0;
    }
    public void ButtonClick()
    {
        AudioManager.Instance.AudioButtonClick();
    }
}
