using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MyAgent : Agent
{
    private enum ACTION
    {
        UP,
        FALL,
        LEFT,
        FORWARD,
        RIGHT,
        BACKWARD
    }

    public Transform LandZone;

    public Transform StartStation;

    public float speed;

    public bool onGround = true;

    public float originalDistance;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(StartStation.localPosition.x, StartStation.localPosition.y + 0.4f, StartStation.localPosition.z);
        onGround = true;

        LandZone.localPosition = new Vector3(-32.28171f, 1.4f, -71.1604f);

        originalDistance = Vector3.Distance(transform.localPosition, LandZone.localPosition);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ágens pozítciója
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localPosition.z);

        // target pozíciója
        sensor.AddObservation(LandZone.localPosition.x);
        sensor.AddObservation(LandZone.localPosition.y);
        sensor.AddObservation(LandZone.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.DiscreteActions[0];

        Vector3 movingDirect = Vector3.zero;

        switch (actionTaken)
        {
            case (int)ACTION.FORWARD:
                movingDirect = Vector3.forward;
                break;
            case (int)ACTION.BACKWARD:
                movingDirect = Vector3.back;
                break;
            case (int)ACTION.LEFT:
                movingDirect = Vector3.left;
                break;
            case (int)ACTION.RIGHT:
                movingDirect = Vector3.right;
                break;
            case (int)ACTION.UP:
                movingDirect = Vector3.up;
                onGround = false;
                break;
            case (int)ACTION.FALL:
                movingDirect = Vector3.zero;
                break;

        }
        
        if (actionTaken == (int)ACTION.UP)
        {
            transform.Translate(movingDirect * speed * Time.fixedDeltaTime);
        }
        else if (!onGround)
        {
            transform.Translate(movingDirect * speed * Time.fixedDeltaTime);
            transform.Translate(Vector3.down * (speed * 1/3) * Time.fixedDeltaTime);
        }
        float distance = Vector3.Distance(transform.localPosition, LandZone.localPosition);

        if (distance != 0f)
        {
            // AddReward(0.005f * (originalDistance / distance) - (StepCount / 5000));
            AddReward((distance / originalDistance) * -0.01f);
        }
        

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        action[0] = (int)ACTION.FALL;

        if (horizontal == -1)
        {
            action[0] = (int)ACTION.LEFT;
        }
        else if (horizontal == +1)
        {
            action[0] = (int)ACTION.RIGHT;
        }

        if (vertical == -1)
        {
            action[0] = (int)ACTION.BACKWARD;
        }
        else if (vertical == +1)
        {
            action[0] = (int)ACTION.FORWARD;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            action[0] = (int)ACTION.UP;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        onGround = collision.transform.CompareTag("Start");

        if (collision.transform.CompareTag("Finish"))
        {
            Debug.Log($"finish: {StepCount}");
            AddReward(1);
            AddReward((StepCount / 5000) * -1);
            EndEpisode();
        }

        if (collision.transform.CompareTag("Untouchable") || collision.transform.CompareTag("Wall"))
        {
            AddReward(-1);
            EndEpisode();
        }
        
    }

}
