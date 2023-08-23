using UnityEngine;

public class CameraManager : MonoBehaviour {
    private Transform camera;
    private void Start() {
         camera = Camera.main.transform;
        camera.parent = transform;
        camera.localPosition = Vector3.zero;
    }

    private void OnDestroy() {
        if(Camera.main == null) return;
        if(camera) camera.parent = null;
    }
}