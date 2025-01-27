using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    private const float DistanceToStop = 0.2f;

    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private Gold _gold;
    [SerializeField] private Flag _flag;
    [SerializeField] private bool _isStanding = true;
    [SerializeField] private BaseSpawner _baseSpawner;

    private IUnitTarget _target;
    private UnitMover _mover;

    public event Action<Gold> CollectedResource;

    public bool IsResourceCollected { get; private set; } = false;
    public bool IsStanding => _isStanding;

    public Vector3 InitialPosition => _initialPosition;

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
        _initialPosition = transform.position;
    }

    private void Update()
    {
        if (_gold != null)
        {
            if (_gold.isActiveAndEnabled == false)
            {
                _target = null;
                _gold = null;
                _isStanding = true;
            }
        }

        if (IsResourceCollected & Vector2.SqrMagnitude(transform.position - _initialPosition) <= DistanceToStop)
            OnCollectedGold();

        if (_target != null)
        {
            _mover.Move(_target.Transform.position - transform.position);
            _isStanding = false;
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.TryGetComponent(out IUnitTarget unitTarget))
        {
            if (unitTarget == _target)
            {
                if (unitTarget is Gold)
                {
                    IsResourceCollected = true;
                    _mover.SetDirection(_initialPosition - transform.position);
                    _gold.StartFollow(transform);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IUnitTarget unitTarget))
        {
            if (unitTarget == _target)
            {
                if (unitTarget is Flag)
                {
                    _baseSpawner.SpawnBase(_flag.transform.position).Assign(this);
                    _flag.Disable();
                    _target = null;
                    _flag = null;
                    _isStanding = true;
                }
            }
        }
    }

    public void SetTarget(IUnitTarget unitTarget)
    {
        _target = unitTarget;

        if (_target is Gold)
            _gold = (Gold)_target;
        else if (_target is Flag)
            _flag = (Flag)_target;
    }

    private void OnCollectedGold()
    {
        CollectedResource?.Invoke(_gold);
        _mover.Stop();
        IsResourceCollected = false;
        _isStanding = true;
        _gold.Disable();
        _gold.StopFollow();
        _gold = null;
        _target = null;
    }

    public void Stop(float time)
    {
        StartCoroutine(StopForSeconds(time));
    }

    private IEnumerator StopForSeconds(float time)
    {
        _isStanding = false;

        yield return new WaitForSeconds(time);

        _target = null;
        _gold = null;
        _isStanding = true;
    }

    public void SetInitialPosition(Vector3 position)
    {
        _initialPosition = position;
    }

    public Unit Initialize(BaseSpawner baseSpawner)
    {
        _baseSpawner = baseSpawner;

        return this;
    }
}
