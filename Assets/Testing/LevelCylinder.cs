using System;
using HyperScripts;
using HyperScripts.Core;
using UnityEngine;

public class LevelCylinder : MonoBehaviour
{
    [SerializeField] private float targetScale = 1;
    private float startingYscale;

    private void Awake()
    {
        HyperCore.ConnectFrameUpdate(ScaleToLevel);
        startingYscale = transform.localScale.y;
    }

    private void ScaleToLevel(HyperValues values)
    {
        float scale = ((targetScale - startingYscale) * values.Amplitude);
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }
}