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
    [SerializeField] private Transform spawnsAgent;
    [SerializeField] private Transform spawnsTarget;
    [SerializeField] private Transform target;
    [SerializeField] private float wallReward = -1;
    [SerializeField] private float winReward = +1;
    [SerializeField] private float spead;
    private void Awake()
    {
        Time.timeScale = TimeScale;
    }
    public override void OnEpisodeBegin()
    {
        transform.position = spawnsAgent.GetChild(Random.Range(0, spawnsAgent.childCount)).position;
        target.position = spawnsTarget.GetChild(Random.Range(0, spawnsTarget.childCount)).position;
    }

    public override void CollectObservations(VectorSensor sensors)
    {
        sensors.AddObservation(new Vector3(transform.position.x, transform.position.y, 0));
        sensors.AddObservation(new Vector3(target.position.x, target.position.y, 0));
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1], 0);
        transform.position += move.normalized * spead * Time.deltaTime;
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == target.gameObject.name)
        {
            SetReward(winReward);
            Debug.Log("Win");
            EndEpisode();
        }
        else
        {
            SetReward(wallReward);
            Debug.Log("Lose");
            EndEpisode();
        }
    }
}
