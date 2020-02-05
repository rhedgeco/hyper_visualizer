using UnityEngine;
using HyperCoreScripts;
using Random = UnityEngine.Random;

public class TestRot : MonoBehaviour
{
    private Vector3 spin;

    private void Awake()
    {
        spin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        );
        
        HyperCore.ConnectFrameUpdate(RotateObject);
    }

    private void RotateObject(HyperValues values)
    {
        transform.Rotate(spin);
        Debug.Log(HyperCore.DeltaTime);
    }
}