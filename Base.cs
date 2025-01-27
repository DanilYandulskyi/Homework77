using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class Base : MonoBehaviour
{
    [SerializeField] private List<Unit> _units = new List<Unit>();
    [SerializeField] private List<Gold> _collectedGold = new List<Gold>();

    [SerializeField] private Scanner _scanner;
    [SerializeField] private UnitSpawner _unitSpawner;
    [SerializeField] private Raycaster _raycaster;
    [SerializeField] private ClickProcessor _clickProcessor;
    [SerializeField] private FlagHandler _flagHandler;

    [SerializeField] private int _price;

    private bool _isSelected;
    private bool _isFlagTaken = false;

    public event UnityAction GoldAmountChanged;

    public int CollectedResounrseAmount => _collectedGold.Count;

    private void Start()
    {
        _clickProcessor.OnClicked += SetFlag;

        foreach (Unit unit in _units)
        {
            unit.CollectedResource += CollectResource;
        }
    }

    private void LateUpdate()
    {
        if (_scanner.GoldList.Count > 0)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                if (_units[i].IsStanding)
                {
                    Gold gold = _scanner.GoldList[0];

                    _units[i].SetTarget(gold);
                    _scanner.RemoveGold(gold);
                    break;
                }
            }
        }

        if (CanBuildNewBase())
        {
            TrySendUnitToFlag();
        }
    }

    public void Assign(Unit unit)
    {
        _units.Add(unit);

        unit.SetInitialPosition(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z));
    }

    private void OnMouseDown()
    {
        if (_isSelected == false)
        {
            _isSelected = true;
        }
    }

    public void SetFlag()
    {
        if (_isSelected)
        {
            Vector3 flagSetPosition = _raycaster.Cast(Input.mousePosition);

            if (flagSetPosition != Vector3.zero)
            {
                _flagHandler.SetFlag(new Vector3(flagSetPosition.x, flagSetPosition.y + 0.5f, flagSetPosition.z));
                _isSelected = false;
            }
        }
    }

    private void CollectResource(Gold gold)
    {
        _collectedGold.Add(gold);

        TryAddNewUnit();

        GoldAmountChanged?.Invoke();
    }

    private void TryAddNewUnit()
    {
        if (_collectedGold.Count >= _unitSpawner.GoldToSpawn)
        {   
            if (_units.Count == 1 || _flagHandler.IsFlagSet == false)
            {
                Unit unit = _unitSpawner.SpawnUnit(_units[_units.Count - 1].InitialPosition + new Vector3(2, 0, 0));

                unit.Stop(1);

                _units.Add(unit);

                unit.CollectedResource += CollectResource;

                for (int i = 0; i < _unitSpawner.GoldToSpawn; i++)
                {
                    _collectedGold.Remove(_collectedGold[_collectedGold.Count - 1]);
                }
            }
        }
    }

    private bool CanBuildNewBase()
    {   
        return _collectedGold.Count >= _price & _flagHandler.IsFlagSet & _isFlagTaken == false & _units.Count > 1;
    }

    private void TrySendUnitToFlag()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            Unit unit = _units[i];

            if (unit.IsStanding)
            {
                unit.CollectedResource -= CollectResource;

                unit.SetTarget(_flagHandler.TryGetFlag());
                _isFlagTaken = true;

                _units.Remove(unit);

                for (int j = 0; j < _price; j++)
                {
                    _collectedGold.Remove(_collectedGold[_collectedGold.Count - 1]);
                }

                break;
            }
        }
    }

    public Base Initialize(Scanner scanner, UnitSpawner unitSpawner, FlagHandler flagHandler, Raycaster raycaster, ClickProcessor clickProcessor)
    {
        _scanner = scanner;
        _unitSpawner = unitSpawner;
        _flagHandler = flagHandler;
        _raycaster = raycaster;
        _clickProcessor = clickProcessor;

        return this;
    }

    private void OnDestroy()
    {
        _clickProcessor.OnClicked -= SetFlag;

        foreach (Unit unit in _units)
        {
            unit.CollectedResource -= CollectResource;
        }
    }
}
