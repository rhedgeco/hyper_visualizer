using System;
using UnityEngine;

namespace HyperScripts.UI
{
    public class EditorSingleton : MonoBehaviour
    {
        private static EditorSingleton _instance;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            if (_instance != this) Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}
