using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class CoolLettering : MonoBehaviour
{
    public Color[] m_colors;

    private Text m_coolText;
    private Animator m_animator;

    private string[] m_coolWordsList = new string[]
    {
        "WOW!",
        "AWESOME!",
        "GREAT!",
        "COOL!",
        "YEAH!",
        "PERFECT!",
        "BOOM!",
        "POW!",
        "OOPS!",
        "EXCELSIOR!"
    };

    private void Awake()
    {
        m_coolText = GetComponent<Text>();
        m_animator = GetComponent<Animator>();
    }

    public IEnumerator ShowCoolWord(float delay)
    {
        yield return new WaitForSeconds(delay);

        m_coolText.text = m_coolWordsList[Random.Range(0, m_coolWordsList.Length)];

        m_coolText.color = m_colors[Random.Range(0, m_colors.Length)];

        m_coolText.rectTransform.anchoredPosition =
            new Vector2(Random.Range(Screen.width / 4f, Screen.width / 1.5f) / 2f * Random.Range(-1, 2),
                        Random.Range(Screen.height / 8f, Screen.height / 3f));

        m_animator.Play("TestAnim");

    }
}
