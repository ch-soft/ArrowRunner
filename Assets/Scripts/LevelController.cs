using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [BoxGroup("Buttons"), SerializeField] private Button m_levelMenuButton;
    [BoxGroup("Buttons"), SerializeField] private Button m_continueButton;

    [BoxGroup("References"), SerializeField] private Animator m_levelAnimator;

    private void Awake()
    {
        m_levelMenuButton.onClick.AddListener(OpenEndgamePanel);
        m_continueButton.onClick.AddListener(RestartLevel);

    }

    private void Update()
    {
        TimeControl.AutoNormalizeTime();
    }

    private void Start()
    {
        StartCoroutine(TimeControl.NormalizeTime(0f));
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenEndgamePanel()
    {
        m_levelAnimator.Play("ShowEndgamePanel");
    }
}
