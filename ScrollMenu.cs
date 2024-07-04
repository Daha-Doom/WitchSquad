using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMenu : MonoBehaviour
{
    [SerializeField]
    float scrollSpeed = 1f;

    [SerializeField]
    float startPos = -500f;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, startPos);
    }

    private void FixedUpdate()
    {

        if (startPos < 0)
        {
            startPos += scrollSpeed;
            rectTransform.anchoredPosition = new Vector2(0, startPos);
        }
    }
}
