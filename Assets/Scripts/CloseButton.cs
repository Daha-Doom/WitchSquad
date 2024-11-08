using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    Collider2D col;

    // Start is called before the first frame update
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnMouseUp()
    {
        Application.Quit();
    }

    void Update()
    {
        if (Input.GetKey("escape"))  // если нажата клавиша Esc (Escape)
        {
            Application.Quit();    // закрыть приложение
        }
    }
}
