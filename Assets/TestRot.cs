using HyperCore;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestRot : MonoBehaviour
{
    private Vector3 spin;

    private void Awake()
    {
        spin = new Vector3(
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f),
            Random.Range(-5f, 5f)
        );
        
        HyperCoreManager.ConnectFrameUpdate(RotateObject);
    }

    private void RotateObject(HyperValues values)
    {
        transform.Rotate(spin);
        Debug.Log(HyperCoreManager.DeltaTime);
    }
}