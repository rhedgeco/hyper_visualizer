using System.Collections.Generic;
using HyperEngine;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]
public class PostController : MonoBehaviour
{
    private List<PostProcessEffectSettings> settings;

    private void Awake()
    {
        settings = GetComponent<PostProcessVolume>().profile.settings;
        HyperCore.ConnectFrameUpdate(AnimateVolume);
    }

    private void AnimateVolume(HyperValues values)
    {
        ChromaticAberration ca = settings.Find(e => e is ChromaticAberration) as ChromaticAberration;
        if (ca != null) ca.intensity.value = values.Amplitude;
    }
}