using UnityEngine;

public class GoldUIViewSpawner : MonoBehaviour
{
    [SerializeField] private GoldUIView _goldUIView;
    
    private GoldUIView _lastSpawnedGoldUIView;

    private void Start()
    {
        _lastSpawnedGoldUIView = _goldUIView;
    }

    public void Spawn(Base @base)
    {
        GoldUIView goldUIView =  Instantiate(_goldUIView, transform).Initialize(@base);

        goldUIView.transform.localPosition = new Vector3(_lastSpawnedGoldUIView.transform.localPosition.x, _lastSpawnedGoldUIView.transform.localPosition.y - 300,
        _lastSpawnedGoldUIView.transform.localPosition.z);

        _lastSpawnedGoldUIView = goldUIView;
    }
}
