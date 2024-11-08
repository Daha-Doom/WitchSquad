using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    public List<bool> exit = new List<bool>() { false, false, false, false, false, false, false, false };
}
