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

    public float speed = 5.0f;

    public bool onGround = false;

    public override void OnEpisodeBegin()
    {
        // transform.position = new Vector3(-16.91f, 41.85f, -19.26f);
        transform.position = new Vector3(35.9621f, 1.757185f, 58.90649f);
        onGround = false;

        //LandZone.localPosition = new Vector3(11.5f, 0f, 13.5f);
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

        float movingSpeed = 0f;

        switch (actionTaken)
        {
            case (int)ACTION.FORWARD:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                movingSpeed = 1f;
                break;
            case (int)ACTION.BACKWARD:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                movingSpeed = 1f;
                break;
            case (int)ACTION.LEFT:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                movingSpeed = 1f;
                break;
            case (int)ACTION.RIGHT:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                movingSpeed = 1f;
                break;
            case (int)ACTION.UP:
                transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
                movingSpeed = 1f;
                onGround = false;
                break;
            case (int)ACTION.FALL:
                movingSpeed = 0f;
                break;

        }

        
        if (transform.localPosition.y <= 0.6f)
        {
            onGround = false;
        }
        else
        {
            onGround = false;
        }
        

        if (actionTaken != (int)ACTION.UP)// && !onGround)
        {
            transform.Translate(Vector3.forward * speed * movingSpeed * Time.fixedDeltaTime);
        }
        AddReward(-0.01f);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        // alapértelmezett érték, hogyha nem nyomunk semmit, akkor zuhanjon
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
        //TODO
    }

}
