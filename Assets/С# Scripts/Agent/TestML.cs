using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Barracuda;

public class TestML : Agent
{
    [SerializeField] private Transform spawnsAgent;
    [SerializeField] private Transform spawnsTarget;
    [SerializeField] private Transform target;
    [SerializeField] private float wallReward = -1;
    [SerializeField] private float winReward = +1;
    /*
    [SerializeField] private PhysicsMaterial2D winMaterial;
    [SerializeField] private PhysicsMaterial2D loMaterial;
    */

    [SerializeField] private float spead;
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        transform.position = spawnsAgent.GetChild(Random.RandomRange(0, spawnsAgent.childCount)).position;
        target.position = spawnsTarget.GetChild(Random.RandomRange(0, spawnsTarget.childCount)).position;
    }

    public override void CollectObservations(VectorSensor sensors)
    {
        sensors.AddObservation(new Vector2(transform.position.x, transform.position.y));
        sensors.AddObservation(new Vector2(target.position.x, target.position.y));
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1], 0);
        transform.position += move.normalized * spead;
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[0] = Input.GetAxisRaw("Vertical");
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
