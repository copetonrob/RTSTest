using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _mapSize = 200;
    [SerializeField] int _unitsPerSide = 100;
    [SerializeField] int _gridCellSize = 10;
    [SerializeField] Unit _unit;
    [SerializeField] Material _playerMaterial;
    [SerializeField] Material _enemyMaterial;

    Dictionary<Vector2Int, HashSet<Unit>> _grid = new Dictionary<Vector2Int, HashSet<Unit>>();
    List<Unit> _allUnits = new List<Unit>();

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        //singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //spawn units
        for (int i = 0; i < _unitsPerSide; i++)
        {
            SpawnUnit(0, _playerMaterial);
            SpawnUnit(1, _enemyMaterial);
        }
    }

    void SpawnUnit(int side, Material material)
    {
        //create unit
        Vector3 position = GetRandomPointOnMap();
        Vector2Int gridPosition = new Vector2Int((int)(position.x / _gridCellSize), (int)(position.z / _gridCellSize));
        Unit unit = Instantiate(_unit, position, Quaternion.identity);
        unit.Init(side, material, gridPosition);
        _allUnits.Add(unit);

        //add unit to grid
        if (_grid.ContainsKey(gridPosition))
        {
            _grid[gridPosition].Add(unit);
        }
        else
        {
            _grid[gridPosition] = new HashSet<Unit> { unit };
        }
    }

    private void Update()
    {
        //update grid and find targets for each unit each frame
        UpdateGridUnits();
        FindTargetsForEachUnit();
    }

    void UpdateGridUnits()
    {
        foreach (Unit unit in _allUnits)
        {
            Vector2Int oldGridPosition = unit.GridPosition;
            Vector2Int newGridPosition = new Vector2Int((int)(unit.transform.position.x / _gridCellSize), (int)(unit.transform.position.z / _gridCellSize));
            if (oldGridPosition != newGridPosition)
            {
                _grid[oldGridPosition].Remove(unit);
                if (_grid.ContainsKey(newGridPosition))
                {
                    _grid[newGridPosition].Add(unit);
                }
                else
                {
                    _grid[newGridPosition] = new HashSet<Unit> { unit };
                }
                unit.GridPosition = newGridPosition;
            }
        }
    }

    void FindTargetsForEachUnit()
    {
        foreach (Unit unit in _allUnits)
        {
            if (unit.HasTarget)
            {
                continue;
            }

            Transform closestEnemy = null;
            unit.SetTarget(closestEnemy);
            float minSqrDistance = float.MaxValue;
            List<Vector2Int> gridCells = GetGridCellsAroundUnit(unit);
            foreach (Vector2Int gridCell in gridCells)
            {
                if (_grid.ContainsKey(gridCell))
                {
                    foreach (Unit enemy in _grid[gridCell])
                    {
                        if (enemy.Side != unit.Side)
                        {
                            float sqrDistance = Vector3.SqrMagnitude(enemy.transform.position - unit.transform.position);
                            if (sqrDistance < minSqrDistance)
                            {
                                minSqrDistance = sqrDistance;
                                closestEnemy = enemy.transform;
                            }
                        }
                    }
                }
            }
            if (minSqrDistance < unit.AttackRadius * unit.AttackRadius)
            {
                unit.SetTarget(closestEnemy);
            }
        }
    }

    List<Vector2Int> GetGridCellsAroundUnit(Unit unit)
    {
        //get all grid cells in unit attack radius
        List<Vector2Int> gridCells = new List<Vector2Int>();
        int leftGridCell = (int)((unit.transform.position.x - unit.AttackRadius) / _gridCellSize);
        int rightGridCell = (int)((unit.transform.position.x + unit.AttackRadius) / _gridCellSize);
        int bottomGridCell = (int)((unit.transform.position.z - unit.AttackRadius) / _gridCellSize);
        int topGridCell = (int)((unit.transform.position.z + unit.AttackRadius) / _gridCellSize);
        for (int x = leftGridCell; x <= rightGridCell; x++)
        {
            for (int y = bottomGridCell; y <= topGridCell; y++)
            {
                gridCells.Add(new Vector2Int(x, y));
            }
        }
        return gridCells;
    }

    public Vector3 GetRandomPointOnMap()
    {
        return new Vector3(Random.Range(0f, _mapSize), 0, Random.Range(0f, _mapSize));
    }
}
