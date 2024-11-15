using UnityEngine;
using TMPro;
using UnityEditor;

public class GameManager : Singleton<GameManager>
{
    [Header("Control Elements")] public GameObject temporaryLetterPrefab;
    public GridManager gridManager;
    public RectTransform _letterSelector;

    [Space(15)] [Header("UI Elements")] [SerializeField]
    private RectTransform[] NextLvlPanel;

    [SerializeField] private RectTransform[] GamePanel;
    [SerializeField] private TextMeshProUGUI lvltext;
    [SerializeField] private TextMeshProUGUI winText;

    public readonly LevelManager _lvlManager = new();
    [SerializeField] private RectTransform outGamePanel;
    [SerializeField] private CircleLayout _circleLayout;
    private WordManager _wordManager;

    private void Start()
    {
        Application.targetFrameRate = 120;
        _wordManager = new WordManager(gridManager, _letterSelector, temporaryLetterPrefab);
        LoadMenu();
    }

    private bool IsOpened = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsOpened)
            {
                TweenManager.Instance.GrowToOriginalSize(outGamePanel);
                IsOpened = true;
            }
            else
            {
                CloseOutPanel();
            }
        }
    }


    public void CloseOutPanel()
    {
        TweenManager.Instance.ShrinkToMinimum(outGamePanel);
        IsOpened = false;
    }

    public void OutGameButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public void BackButton()
    {
        ShrinkPanel(NextLvlPanel, () =>
        {
            GrowPanel(GamePanel, () =>
            {
                ActiveWaitingAnim();
                _lvlManager.SaveLvlSettings();
                StartLevel();
            });
        });
    }


    private void LoadMenu()
    {
        StartLevel();
        lvltext.text = _lvlManager.currentLvl.ToString();
        GrowPanel(GamePanel, () => ActiveWaitingAnim());
    }

    private void LoadMenuWin()
    {
        lvltext.text = _lvlManager.currentLvl.ToString();
        ShrinkPanel(NextLvlPanel, () =>
        {
            TweenManager.Instance.AnimateWinText(winText, () =>
            {
                GrowPanel(GamePanel, () =>
                {
                    StartLevel();
                    ActiveWaitingAnim();
                });
            });
        });
    }

    public void LoadLevel()
    {
        lvltext.text = _lvlManager.currentLvl.ToString();
        _lvlManager.LoadLvlSettings();
        ShrinkPanel(GamePanel, () => { GrowPanel(NextLvlPanel); });
    }


    private void ShrinkPanel(RectTransform[] panels, System.Action onComplete = null)
    {
        int shrinkAnimCount = 0;

        foreach (var item in panels)
        {
            TweenManager.Instance.ShrinkToMinimum(item, () =>
            {
                shrinkAnimCount++;
                if (shrinkAnimCount == panels.Length)
                {
                    onComplete?.Invoke();
                }
            });
        }
    }

    private void GrowPanel(RectTransform[] panels, System.Action onComplete = null)
    {
        int growAnimCount = 0;

        foreach (var item in panels)
        {
            TweenManager.Instance.GrowToOriginalSize(item, () =>
            {
                growAnimCount++;
                if (growAnimCount == panels.Length)
                {
                    onComplete?.Invoke();
                }
            });
        }
    }

    private void ActiveWaitingAnim() => TweenManager.Instance.AnimateCircleObjectsLoop(GamePanel);


    private void OnApplicationQuit()
    {
        _lvlManager.SaveLvlSettings();
    }

    public void ShuffleLetters() => SpawnOrShuffleLetters(); //TODO Shuffle anim


    private async void StartLevel()
    {
        await _lvlManager.LoadLevel();
        await _lvlManager.LoadLvlSettings();
        lvltext.text = _lvlManager.currentLvl.ToString();
        gridManager.GenerateGirdForLvl(_lvlManager.GetLevelData());
        _wordManager.SetLevelData(_lvlManager.GetLevelData());
        gridManager.LoadOpenedWords(_lvlManager.GetLevelSettings());

        SpawnOrShuffleLetters();
    }

    public async void CheckIfWin()
    {
        if (_lvlManager.GetLevelData().words.Count != _lvlManager.GetLevelSettings().wordsData.Count) return;
        _lvlManager.GetLevelSettings().wordsData.Clear();
        _lvlManager.SaveLvlSettings();
        _lvlManager.currentLvl += 1;

        await _lvlManager.LoadLevel();
        if (_lvlManager.GetLevelData() == null)
        {
            CompleteAllLVL();
        }
        else
        {
            LoadMenuWin();
        }
    }

    private void CompleteAllLVL()
    {
        _lvlManager.currentLvl -= 1;
        lvltext.text = _lvlManager.currentLvl.ToString();
        ShrinkPanel(NextLvlPanel, () =>
        {
            winText.text = "Congratulation!\nYou Complete all levels";
            TweenManager.Instance.AnimateWinText(winText, () =>
            {
                winText.text = "Win";
                GrowPanel(GamePanel, () =>
                {
                    StartLevel();
                    ActiveWaitingAnim();
                    _lvlManager.LoadLevel();
                    SpawnOrShuffleLetters();
                });
            });
        });
    }

    public void CheckIfWordIsCorrect(string word) =>
        _wordManager.CheckIfWordIsCorrect(word, _lvlManager.GetLevelData().words);

    private void SpawnOrShuffleLetters() => _circleLayout.SpawnLetters(_lvlManager.GetLetterCount());
}