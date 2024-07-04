using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    Collider2D col;

    // Start is called before the first frame update
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnMouseUp()
    {
        SceneManager.LoadScene("Lvl_1");
    }
}
