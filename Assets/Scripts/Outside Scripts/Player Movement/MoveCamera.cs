using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform CameraPosition;

    void Update() {
        transform.position = CameraPosition.position;
    }
}
