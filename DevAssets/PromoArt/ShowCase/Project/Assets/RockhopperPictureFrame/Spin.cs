using UnityEngine;

public class SlowShowcaseSpin : MonoBehaviour
{
    public float rotationSpeed = 80f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
