using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines : MonoBehaviour
{
    private static Defines instance;

    public static Defines myInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Defines>();
            }
            return instance;
        }
    }

    public bool EnableDebug = false;
}
