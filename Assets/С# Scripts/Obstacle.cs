using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float maxX = 0f;
    [SerializeField] private float minX = 0f;
    [SerializeField] private float speed = 1f;
    private int dir = 1;
    private Vector3 V3 = new Vector3(1, 0, 0);
    void Start()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), transform.position.y, 0);
    }
    private void Update()
    {
        if (transform.position.x > maxX && dir > 0)
        {
            dir = -1;
        }
        else if (transform.position.x < minX && dir < 0)
        {
            dir = 1;
        }

        

        transform.position += V3 * dir * Time.deltaTime * speed;
    }
}
