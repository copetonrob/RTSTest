using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 _targetPosition;
    float _speed;    

    public void Init(Vector3 targetPosition, float speed)
    {
        _targetPosition = targetPosition;
        _speed = speed;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        if (transform.position == _targetPosition)
        {
            Destroy(gameObject);
        }
    }
}
