using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public Action m_onRectTransformDimensionsChange = null;
    public List<GameObject> m_subCanvases = new List<GameObject>();
    public List<GameObject> m_objects = new List<GameObject>();
    public Vector2 m_subCanvasSize = new Vector2( 600, 800 );

    private RectTransform m_rectTransform = null;

    // Start is called before the first frame update
    void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateUI();

        m_onRectTransformDimensionsChange?.Invoke();
    }

    private void UpdateUI()
    {
        if( !m_rectTransform )
            return;

        Vector2 rectSize = m_rectTransform.sizeDelta;

        float s = rectSize.x / m_subCanvasSize.x;

        foreach( GameObject canvas in m_subCanvases )
        {
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvasRectTransform.localScale = new Vector2( s, s );

        }

        foreach( GameObject obj in m_objects )
        {
            RectTransform objRectTransform = obj.GetComponent<RectTransform>();
            objRectTransform.localScale = new Vector2( s, s );
        }

    }
}
