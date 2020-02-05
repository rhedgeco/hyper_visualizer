using UnityEngine;
using HyperCoreScripts;
using Random = UnityEngine.Random;

public class TestRot : MonoBehaviour
{
    private Vector3 spin;

    private void Awake()
    {
        spin = new Vector3(
            Random.Range(-50f, 50f),
            Random.Range(-50f, 50f),
            Random.Range(-50f, 50f)
        );
        
        HyperCore.ConnectFrameUpdate(RotateObject);
    }

    private void RotateObject(HyperValues values)
    {
        transform.rotation = Quaternion.identity;
        transform.Rotate(spin * HyperCore.Time);
    }
}