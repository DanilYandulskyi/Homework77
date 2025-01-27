using UnityEngine;

public class FlagHandler : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Flag _spawnedFlag;
    [SerializeField] private BaseSpawner _baseSpawner;

    private bool _isFlagSet = false;

    public bool IsFlagSet => _isFlagSet;

    private void Update()
    {
        if (_spawnedFlag != null)
        {
            if (_spawnedFlag.isActiveAndEnabled == false)
            {
                _isFlagSet = false;
            }
        }
    }

    public void SetFlag(Vector3 position)
    {
        if (_isFlagSet)
        {
            _isFlagSet = true;
            SetFlagPosition(position);
        }
        else
        {
            _isFlagSet = true;
            _spawnedFlag = SpawnFlag(position);
        }
    }

    public Flag TryGetFlag()
    {
        return _spawnedFlag;
    }

    public void SetFlagPosition(Vector3 position)
    {
        _spawnedFlag.gameObject.SetActive(true);
        _spawnedFlag.transform.position = position;
    }

    public Flag SpawnFlag(Vector3 position)
    {
        return Instantiate(_flagPrefab, position, Quaternion.identity);
    }
}
