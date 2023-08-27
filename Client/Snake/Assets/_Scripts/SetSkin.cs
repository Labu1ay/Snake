using UnityEngine;

public class SetSkin : MonoBehaviour {
    [SerializeField] private MeshRenderer [] _meshRenderers;

    public void Set(Material material){
        for (int i = 0; i < _meshRenderers.Length; i++){
            _meshRenderers[i].material = material;
        }
    }
    public void SetMaterial(byte index) => Set(Skins.Instance.GetMaterial(index));
}
