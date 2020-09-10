using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelController : MonoBehaviour
{
    [BoxGroup("Buttons"), SerializeField] private Button m_restartMenuButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_continueButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_tapToPlayButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_restartIngameButton;

    [BoxGroup("References"), SerializeField] private Animator m_winnerAnimator;
    [BoxGroup("References"), SerializeField] private Animator m_defeatAnimator;
    [Space]
    [BoxGroup("Scroll Bar References"), SerializeField] private Slider m_progressBar;
    [BoxGroup("Scroll Bar References"), SerializeField] private Text m_currentLevelText;
    [BoxGroup("Scroll Bar References"), SerializeField] private Image m_fill;
    [BoxGroup("Scroll Bar References"), SerializeField] private Color[] m_progressBarColors;

    private GameObject[] m_listOfLevels;
    private PlayerInstance m_playerInstance;
    private WinnerZone m_winnerZone;

    private Vector3 m_winnerZonePosition;

    private int m_currentLevel;
    private int m_currentSpawnedLevelID;

    private float m_progressBarMaxValue;

    private void Awake()
    {
        AnalyticsManager.Initialize();

        m_playerInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInstance>();

        m_tapToPlayButton.onClick.AddListener(StartRun);
        m_restartIngameButton.onClick.AddListener(RestartLevel);
        m_restartMenuButton.onClick.AddListener(RestartLevel);
        m_continueButton.onClick.AddListener(RestartLevel);

        SetupLevelsArray();
    }

    //private void Update()
    //{
    //    TimeControl.AutoNormalizeTime();
    //}

    private void Start()
    {
        StartCoroutine(TimeControl.NormalizeTime(0f));

        TimeControl.m_levelFinished = false;

        LoadCurrentLevelAmount();

        SpawnLevelFromArray();
        //do this after spawn whole level
        m_winnerZone = GameObject.FindGameObjectWithTag("WinnerZone").GetComponent<WinnerZone>();
        m_winnerZonePosition = m_winnerZone.transform.position;

        SetupScrollBar();
    }

    private void Update()
    {
        CalculateProgressBarValue();
    }

    private void StartRun()
    {
        m_playerInstance.AllowToRun(true);

        m_playerInstance.PlayRunAnimation();

        m_tapToPlayButton.gameObject.SetActive(false);

        AnalyticsManager.FireRoundStartEvent(m_currentLevel, 0);
    }

    private void RestartLevel()
    {
        TimeControl.NormalizeTime(0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenDefeatPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            AnalyticsManager.FireRoundFailEvent(m_currentLevel, 0, 0, 100);//Need consider 2, 3, 4

            AnalyticsManager.FireRoundEndEvent(m_currentLevel, 0, 0);

            m_defeatAnimator.Play("ShowEndgamePanel");
        }
    }

    public void OpenWinnerPanel()
    {
        if (!TimeControl.m_levelFinished)
        {
            AnalyticsManager.FireRoundCompleteEvent(m_currentLevel, 0, 0); //Need consider 2nd & 3rd 
            AnalyticsManager.FireRoundEndEvent(m_currentLevel, 0, 0);
            m_winnerAnimator.Play("ShowEndgamePanel");

            PrepareNextLevel();
            PlayerPrefs.SetInt("CurrentLevelAmount", m_currentLevel + 1);
        }
    }

    private void SetupScrollBar()
    {
        m_fill.color = m_progressBarColors[Random.Range(0, m_progressBarColors.Length)];

        m_currentLevelText.text = m_currentLevel.ToString();

        m_progressBar.maxValue = Vector3.Distance(m_playerInstance.transform.position, m_winnerZonePosition);
        m_progressBarMaxValue = m_progressBar.maxValue;
    }

    private void CalculateProgressBarValue()
    {
        m_progressBar.value = m_progressBarMaxValue - Vector3.Distance(m_playerInstance.transform.position, m_winnerZonePosition);
    }

    private void LoadCurrentLevelAmount()
    {
        if (PlayerPrefs.HasKey("CurrentLevelAmount"))
        {
            m_currentLevel = PlayerPrefs.GetInt("CurrentLevelAmount");
        }
        else
        {
            NullifyLevelsAmount();
        }
    }

    private void PrepareNextLevel()
    {
        m_currentSpawnedLevelID++;
        PlayerPrefs.SetInt("CurrentSpawnedLevel", m_currentSpawnedLevelID);
    }

    private void PreparePrevLevel()
    {
        if (m_currentSpawnedLevelID >= 0)
        {
            m_currentSpawnedLevelID--;
            PlayerPrefs.SetInt("CurrentSpawnedLevel", m_currentSpawnedLevelID);
        }
    }

    public void OpenNextLevel()
    {
        PrepareNextLevel();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenPrevLevel()
    {
        PreparePrevLevel();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SetupLevelsArray()
    {
        m_listOfLevels = Resources.LoadAll<GameObject>("Prefabs/Levels");
    }

    private void SpawnLevelFromArray()
    {
        if (PlayerPrefs.HasKey("CurrentSpawnedLevel"))
        {
            m_currentSpawnedLevelID = PlayerPrefs.GetInt("CurrentSpawnedLevel");
        }
        else
        {
            m_currentSpawnedLevelID = 0;
            PlayerPrefs.SetInt("CurrentSpawnedLevel", 0);
        }

        if ((m_currentSpawnedLevelID > m_listOfLevels.Length - 1) || (m_currentSpawnedLevelID < 0))
        {
            m_currentSpawnedLevelID = 0;
            PlayerPrefs.SetInt("CurrentSpawnedLevel", 0);
        }

        GameObject currentLevelObject = Instantiate(m_listOfLevels[m_currentSpawnedLevelID], Vector3.zero, Quaternion.identity);
    }

    private void NullifyLevelsAmount()
    {
        m_currentLevel = 1;
        PlayerPrefs.SetInt("CurrentLevelAmount", 1);
    }

    public void FullResetLevelsInfo()
    {
        NullifyLevelsAmount();

        m_currentSpawnedLevelID = 0;
        PlayerPrefs.SetInt("CurrentSpawnedLevel", 0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
