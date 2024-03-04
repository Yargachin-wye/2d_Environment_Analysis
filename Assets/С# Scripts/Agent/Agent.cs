using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _spawPoint;
    [SerializeField] private float _inaccuracyMoving = 0.1f;

    [HideInInspector] public UnityEvent movementIsOver;

    private void Start()
    {
        Spawn(0);
        MoveTo(11);
    }
    private IEnumerator Moving(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > _inaccuracyMoving)
        {
            transform.position += (target - transform.position).normalized * Time.fixedDeltaTime * _speed;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        movementIsOver?.Invoke();
    }

    private void Spawn(int spawwnPoint)
    {
        transform.position = Map.instance.GetPointPos(spawwnPoint);
    }
    public void MoveTo(int id)
    {
        Vector2 target = Map.instance.GetPointPos(id);
        StartCoroutine(Moving(target));
    }
}
