using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DestroyExitTime : MonoBehaviour
{
    public float time = 0.5f;
    private float timer = 0f;

    void Update()
    {
        if (timer < time)
            timer += Time.deltaTime;
        else
            Destroy(gameObject);
    }
}
