using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Barracuda;
using Unity.VisualScripting;

public class TestML : Agent
{
    [SerializeField] private float TimeScale;
    [SerializeField] private int maxStepsMove = 100;
    [SerializeField] private Transform spawnsAgent;
    [SerializeField] private Transform spawnsTarget;
    [SerializeField] private Transform target;
    [SerializeField] private float wallReward = -1;
    [SerializeField] private float winReward = +1;
    [SerializeField] private float maxStepsReward = -0.5f;
    [SerializeField] private float moveReward = 0.5f;
    [SerializeField] private float spead;

    [SerializeField] private Transform[] sensorDir;
    [SerializeField] private float sensorDist;
    [SerializeField] private LayerMask sensorLayer;

    private int stepsMove = 0;
    private void Awake()
    {
        Time.timeScale = TimeScale;
    }
    public override void OnEpisodeBegin()
    {
        stepsMove = 0;
        transform.localPosition = spawnsAgent.GetChild(Random.Range(0, spawnsAgent.childCount)).localPosition;
        target.localPosition = spawnsTarget.GetChild(Random.Range(0, spawnsTarget.childCount)).localPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Transform tr in sensorDir)
        {
            Vector2 dir = (tr.localPosition).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, sensorDist, sensorLayer);

            if (hit.collider != null)
            {
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.DrawRay(transform.position, dir * sensorDist);
            }
        }
    }

    public override void CollectObservations(VectorSensor sensors)
    {
        sensors.AddObservation(new Vector2(transform.localPosition.x, transform.localPosition.y));
        sensors.AddObservation(new Vector2(target.localPosition.x, target.localPosition.y));

        foreach (Transform tr in sensorDir)
        {
            Vector2 dir = (tr.localPosition).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, sensorDist, sensorLayer);
            float dist = 0;
            
            if (hit.collider != null)
            {
                dist = Vector2.Distance(transform.position, hit.point);
            }
            else
            {
                dist = Vector2.Distance(transform.position, hit.point);
            }
            
            sensors.AddObservation(dist);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1], 0);
        Vector3 oldPos = transform.localPosition;
        transform.localPosition += move.normalized * spead * Time.deltaTime;
        Vector3 newPos = transform.localPosition;

        if (stepsMove >= maxStepsMove)
        {
            SetReward(maxStepsReward);
            Debug.Log("l_0.5");
            EndEpisode();
        }
        if (Vector3.Distance(newPos, target.localPosition) < Vector3.Distance(oldPos, target.localPosition))
        {
            SetReward(moveReward);
        }
        else
        {
            SetReward(-moveReward);
        }
        stepsMove++;
    }
    /*
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == target.gameObject.name)
        {

            Debug.Log("w_1");
            SetReward(winReward);
            SetReward(maxStepsMove/stepsMove);
            EndEpisode();
        }
        else
        {
            SetReward(wallReward);
            Debug.Log("l_1");
            EndEpisode();
        }
    }
}
