using UnityEngine;

public class CameraManager : MonoBehaviour {
    private Transform camera;
    public void Init(float offsetY) {
         camera = Camera.main.transform;
        camera.parent = transform;
        camera.localPosition = Vector3.up * offsetY;
    }

    private void OnDestroy() {
        if(Camera.main == null) return;
        if(camera) camera.parent = null;
    }
}