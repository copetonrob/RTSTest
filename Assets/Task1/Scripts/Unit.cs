using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float AttackRadius = 5;

    [HideInInspector]
    public Vector2Int GridPosition;
    [HideInInspector]
    public int Side = 0;
    [HideInInspector]
    public bool HasTarget = false;

    [SerializeField] float _speed = 3;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] LineRenderer _targetLine;

    Transform _target;
    Vector3 _movePosition;
    

    public void Init(int side, Material material, Vector2Int gridPosition)
    {
        Side = side;
        _meshRenderer.material = material;
        _targetLine.startColor = material.color;
        _targetLine.endColor = material.color;
        GridPosition = gridPosition;
        PickRandomPoint();
    }

    private void Update()
    {
        Move();
        CheckTarget();
        UpdateTargetLine();
    }

    private void Move()
    {
        if (transform.position == _movePosition)
        {
            PickRandomPoint();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _movePosition, Time.deltaTime * _speed);
        }
    }

    private void CheckTarget()
    {
        if (_target == null || Vector3.Distance(transform.position, _target.position) > AttackRadius)
        {
            HasTarget = false;
        }
    }

    private void UpdateTargetLine()
    {
        if (_target == null)
        {
            _targetLine.enabled = false;
        }
        else
        {
            _targetLine.enabled = true;
            _targetLine.SetPosition(0, _targetLine.transform.position);
            _targetLine.SetPosition(1, _target.position);
        }
    }

    private void PickRandomPoint()
    {
        _movePosition = GameManager.Instance.GetRandomPointOnMap();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if (_target != null)
            HasTarget = true;
    }
}
