using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    //To seek toward the other object
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }


    //To flee from the other object
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }


    //Predicting future location
    void Pursue()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;

        

        float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(target.transform.forward));
        float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));


        if((toTarget > 90 && relativeHeading < 20) || target.GetComponent<Drive>().currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }
        float lookAhead = targetDir.magnitude/(agent.speed + target.GetComponent<Drive>().currentSpeed);
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }


    //Evading from the other object with predicting the location
    void Evade()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float lookAhead = targetDir.magnitude/(agent.speed + target.GetComponent<Drive>().currentSpeed);
        Flee(target.transform.position + target.transform.forward * lookAhead);

    }

    Vector3 wanderTarget  = Vector3.zero;


    void Wander()
    {
        float wanderRadious = 10;
        float wanderDistance = 10;
        float wanderJitter = 1;
        wanderTarget += new Vector3(Random.Range(-1.0f,1.0f) * wanderJitter,0,Random.Range(-1.0f,1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadious;
        Vector3 targetLocal = wanderTarget + new Vector3(0,0,wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);
        Seek(targetWorld);
    }



    void Hide()
    {
        //Finding the closest hiding spots
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        for(int i=0;i<World.Instance.GetHidingSpots().Length;i++)
        {
            Vector3 hideDir = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePos = World.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 5;
            if(Vector3.Distance(this.transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                dist = Vector3.Distance(this.transform.position, hidePos);
            }
        }
        Seek(chosenSpot);
    }


    // Update is called once per frame
    void Update()
    {
        //Seeking toward the other object
        // Seek(target.transform.position);


        //Fleeing from the other object
        // Flee(target.transform.position);



        //Pursue
        // Pursue();


        //Evade
        // Evade();


        //Wander
        // Wander();


        //Hide
        Hide();
    }
}
