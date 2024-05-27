using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float _speed = 3f;
    [SerializeField] float _mapSize = 20f;
    Vector3 targetPosition;

    private void Start()
    {
        StartCoroutine(PickNewTargetPosition());
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * _speed);
    }

    IEnumerator PickNewTargetPosition()
    {
        while (true)
        {
            targetPosition = new Vector3(Random.Range(-_mapSize, _mapSize), 0, Random.Range(-_mapSize, _mapSize));
            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }
}
