using UnityEngine;

public class CameraWobble : MonoBehaviour
{
    public float wobbleAmount, wobbleSpeed;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float y = Mathf.Cos(Time.time * wobbleSpeed) * wobbleAmount;

        transform.localPosition = startPos + new Vector3(x, y, 0);
    }
}
