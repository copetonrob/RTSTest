using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Bullet _bulletPrefab;
    [SerializeField] float _bulletSpeed = 10f;
    [SerializeField] float _fireRate = 1f;

    Vector3 _previousTargetPosition;
    Vector3 _currentTargetPosition;

    private void Start()
    {
        _currentTargetPosition = _target.position;
        StartCoroutine(Shooting());
    }

    private void Update()
    {
        _previousTargetPosition = _currentTargetPosition;
        _currentTargetPosition = _target.position;
    }

    IEnumerator Shooting()
    {
        yield return null;
        while (true)
        {
            Vector3 targetVelocity = (_currentTargetPosition - _previousTargetPosition) / Time.deltaTime;

            Vector3 interceptPoint;
            if (CalculateInterceptPoint(_currentTargetPosition, targetVelocity, transform.position, _bulletSpeed, out interceptPoint))
            {
                Bullet bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
                bullet.Init(interceptPoint, _bulletSpeed);
            }

            yield return new WaitForSeconds(_fireRate);
        }

        bool CalculateInterceptPoint(Vector3 targetPos, Vector3 targetVel, Vector3 turretPos, float bulletSpeed, out Vector3 interceptPoint)
        {
            interceptPoint = Vector3.zero;
            Vector3 toTarget = targetPos - turretPos;
            float a = Vector3.Dot(targetVel, targetVel) - bulletSpeed * bulletSpeed;
            float b = 2 * Vector3.Dot(targetVel, toTarget);
            float c = Vector3.Dot(toTarget, toTarget);

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                Debug.Log("No solution");
                return false; // No solution, no intercept
            }

            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            float t = Mathf.Max(t1, t2);

            if (t < 0)
            {
                Debug.Log("Solution in the past");
                return false; // Solution in the past, no intercept
            }

            interceptPoint = targetPos + targetVel * t;
            return true;
        }
    }
}
