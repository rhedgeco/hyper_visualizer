using System;
using HyperCoreScripts;
using HyperCoreScripts.Managers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AudioTitleController : MonoBehaviour
{
    private Text text;
    
    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        text.text = AudioManager.AudioTitle;
    }
}
