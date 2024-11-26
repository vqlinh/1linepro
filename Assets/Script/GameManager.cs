using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Line linePrefab;
    [SerializeField] private Point pointPrefab;
    [SerializeField] private List<Level> levels;
    [SerializeField] private LineRenderer LineDraw;

    private Canvas canvas;
    private GameObject finger;
    private Level currentLevel;
    private int startIndex = 0;
    private GameObject previousWave;
    private List<GameObject> listWave;
    private bool fingerMoving = false;
    public GameObject waveFormPrefabs;
    public List<GameObject> lineDraws;

    private int currentId;
    private int numberHint;
    private bool isFinished;
    private GameObject panelWin;
    private GameObject panelShop;
    private Point startPoint, endPoint;
    private TextMeshProUGUI txtNumberHint;
    private Dictionary<int, Point> points;
    private Dictionary<Vector2Int, Line> lines;
    List<Line> lineList;
    private int numberLevel;
    private TextMeshProUGUI lv;
    private int numberSelect;

    private void Awake()
    {
        FrameRate();
        UiStart();

        lineList = new List<Line>();

        lineDraws = new List<GameObject>();
        listWave = new List<GameObject>();
 
        isFinished = false;
        points = new Dictionary<int, Point>();
        lines = new Dictionary<Vector2Int, Line>();
        LineDraw.gameObject.SetActive(false);
        currentId = -1;

        numberSelect = PlayerPrefs.GetInt("SelectedLevel");
        numberLevel = PlayerPrefs.GetInt("CompletedLevel", 0);
        numberHint = PlayerPrefs.GetInt("NumberHint", 5);
        Level levelStart = levels[numberSelect];
        LevelStart(levelStart);
        UpdateHint();
    }

    #region FrameRate
    void FrameRate()
    {
        int refreshRate = (int)Screen.currentResolution.refreshRateRatio.numerator;

        if (refreshRate >= 120)
        {
            Application.targetFrameRate = 120;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        QualitySettings.vSyncCount = 0;

    }
    #endregion

    public void UiStart()
    {
        panelWin = GameObject.Find("CompleteLevel");
        panelShop = GameObject.Find("PanelShop");
        panelWin.SetActive(false);
        panelShop.SetActive(false);
        finger = GameObject.Find("Finger");
        finger.SetActive(false);
        lv = GameObject.Find("LevelNumber").GetComponent<TextMeshProUGUI>();
        txtNumberHint = GameObject.Find("TxtNumberHint").GetComponent<TextMeshProUGUI>();
        canvas = GameObject.Find("CanvasWaveForm").GetComponent<Canvas>();
    }


    public void UpdateHint()
    {
        numberHint = PlayerPrefs.GetInt("NumberHint", 5);
        txtNumberHint.text = numberHint.ToString();
    }

    public void Hint()
    {

        if (!isFinished)
        {
            AudioManager.Instance.AudioButtonClick();
            if (fingerMoving) return;
            fingerMoving = true;
            numberHint--;
            PlayerPrefs.SetInt("NumberHint", numberHint);
            PlayerPrefs.Save();
            txtNumberHint.text = numberHint.ToString();

            if (numberHint < 0)
            {
                numberHint = 0;
                txtNumberHint.text = numberHint.ToString();

                PlayerPrefs.SetInt("NumberHint", numberHint);
                PlayerPrefs.Save();
                panelShop.SetActive(true);
            }
            else 
            {
                StartCoroutine(MoveFingerWithHint());
            }
        }
    }
    private IEnumerator MoveFingerWithHint()
    {
        finger.SetActive(true);
        finger.transform.position = currentLevel.Points[Mathf.Min(startIndex, currentLevel.Points.Count - 1)];
        int endIndex = Mathf.Min(startIndex + 4, currentLevel.Lines.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            Vector2Int line = currentLevel.Lines[i];
            Vector3 startPosition = points[line.x].Position;
            Vector3 endPosition = points[line.y].Position;

            // Di chuyển đến startPosition
            finger.transform.position = startPosition;
            yield return null; // Có thể bỏ qua nếu không cần delay

            // Di chuyển đến endPosition
            yield return finger.transform.DOMove(endPosition, 1f).SetEase(Ease.Linear).WaitForCompletion();
        }

        startIndex = endIndex;

        // Hiệu ứng scale
        yield return finger.transform.DOScale(0.8f, 0.2f).SetLoops(2, LoopType.Yoyo).WaitForCompletion();

        finger.SetActive(false);
        fingerMoving = false;

        if (startIndex >= currentLevel.Lines.Count)
            startIndex = 0;
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
        currentLevel = level;
        lv.text = (levels.IndexOf(level) + 1).ToString();
    }

    public void NextLevel()
    {
        AudioManager.Instance.AudioButtonClick();
        numberSelect++;
        if (numberSelect > numberLevel) numberLevel++;
        else return;
        if (numberLevel == -1 || numberLevel == levels.Count - 1) return;
        Level NextLevel = levels[numberLevel];
        ClearPreviousLevel();
        ClearWaveForm();
        LevelStart(NextLevel);
        startIndex = 0;
        if (numberLevel >= numberSelect)
        {
            PlayerPrefs.SetInt("CompletedLevel", numberLevel);
            PlayerPrefs.Save();
        }
    }

    public void Replay()
    {
        //SceneManager.LoadScene("GamePlay");
        if (!isFinished)
        {
            AudioManager.Instance.AudioButtonClick();
            ClearPreviousLevel();
            ClearWaveForm();
            LevelStart(currentLevel);
        }

    }

    private void ClearWaveForm()
    {
        foreach (var wave in listWave)
        {
            Destroy(wave);
        }
        listWave.Clear();
    }

    private void ClearPreviousLevel()
    {
        startPoint = null;
        endPoint = null;
        currentId = -1;
        isFinished = false;
        foreach (var point in points.Values)
        {
            Destroy(point.gameObject);
        }
        points.Clear();

        foreach (var line in lines.Values)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();
    }

    public void Undo()
    {
        if (!isFinished && lineList.Count > 0)
        {
            AudioManager.Instance.AudioButtonClick();
            Line latestFilledLine = lineList.LastOrDefault(line => line.filled);
            if (latestFilledLine != null)
            {
                latestFilledLine.ResetLine();
                startPoint = null;
                endPoint = null;
                currentId = -1;
            }
        }
    }

    private void Update()
    {
        if (isFinished) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit) return;
            startPoint = hit.collider.gameObject.GetComponent<Point>();
            LineDraw.gameObject.SetActive(true);
            LineDraw.positionCount = 2;
            LineDraw.SetPosition(0, startPoint.Position);
            LineDraw.SetPosition(1, startPoint.Position);
        }
        else if (Input.GetMouseButton(0) && startPoint != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit) endPoint = hit.collider.gameObject.GetComponent<Point>();
            LineDraw.SetPosition(1, mousePos2D);
            if (startPoint == endPoint || endPoint == null) return;
            if (IsConnectLine())
            {
                currentId = endPoint.Id;
                lines[new Vector2Int(startPoint.Id, endPoint.Id)].ChangedColorLine();
                lineList.AddRange(lines.Values);
                startPoint = endPoint;
                LineDraw.SetPosition(0, startPoint.Position);
                LineDraw.SetPosition(1, startPoint.Position);
                WaveForm(startPoint.Position);
                UiManager.Instance.MediumVib();
                AudioManager.Instance.AudioPointTouch();
            }
            else if (IsEndConnect())
            {
                currentId = endPoint.Id;
                lines[new Vector2Int(startPoint.Id, endPoint.Id)].ChangedColorLine();
                lineList.AddRange(lines.Values);

                CheckToWin();
                startPoint = endPoint;
                LineDraw.SetPosition(0, startPoint.Position);
                LineDraw.SetPosition(1, startPoint.Position);
                WaveForm(startPoint.Position);
                UiManager.Instance.MediumVib();
                AudioManager.Instance.AudioPointTouch();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            LineDraw.gameObject.SetActive(false);
            startPoint = null;
            endPoint = null;
            CheckToWin();
        }
    }

    private bool IsConnectLine()
    {
        if (currentId != -1) return false;
        Vector2Int line = new Vector2Int(startPoint.Id, endPoint.Id);
        if (!lines.ContainsKey(line)) return false;
        return true;
    }

    private bool IsEndConnect()
    {
        if (currentId != startPoint.Id) return false;

        Vector2Int line = new Vector2Int(endPoint.Id, startPoint.Id);
        if (lines.TryGetValue(line, out Line result))
        {
            if (result == null || result.filled) return false;
        }
        else return false;

        return true;
    }

    private void WaveForm(Vector3 position)
    {
        if (waveFormPrefabs != null)
        {
            if (previousWave != null)
            {
                previousWave.SetActive(false);
            }
            GameObject waveForm = Instantiate(waveFormPrefabs, canvas.transform);
            waveForm.transform.position = position;
            previousWave = waveForm;
            listWave.Add(waveForm);
        }
        if (isFinished)
        {
            for (int i = 0; i < listWave.Count; i++)
            {
                GameObject gameObject = listWave[i];
                gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator ShowUiGameFinish()
    {
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.AudioWin();
        yield return new WaitForSeconds(1f);
        panelWin.SetActive(true);
    }

    private void CheckToWin()
    {
        foreach (var item in lines)
        {
            if (!item.Value.filled) return;
        }
        isFinished = true;
        StartCoroutine(ShowUiGameFinish());
    }

    public void ButtonClick()
    {
        AudioManager.Instance.AudioButtonClick();
    }
}
