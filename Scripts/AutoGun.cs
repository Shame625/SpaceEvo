using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoGun : MonoBehaviour {

    Player player;
    public Transform movablePart;
    public Transform[] shootStart = {null, null};
    Animation anim;
    CircleCollider2D radius;
    float cooldown = 0;
    float speed = 10;
    public GameObject currentTarget;

    List<GameObject> possibleTargets = new List<GameObject>();

    bool shooteSide = false;


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        movablePart = transform.GetChild(0);
        shootStart[0] = movablePart.GetChild(0).GetChild(0);
        shootStart[1] = movablePart.GetChild(0).GetChild(1);
        anim = movablePart.GetChild(0).GetComponent<Animation>();
        radius = transform.GetChild(1).GetComponent<CircleCollider2D>();
    }

    bool startShooting = false;

    public bool didReset = false;

    float timer = 0;

    void FixedUpdate()
    {
        if (!Builder.isBuilding)
        {
            if (currentTarget != null)
            {

                    //Uh oh, quaternions, very complex stuff, why does it need Vector3.forward? no fucking clue.
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, currentTarget.transform.position - transform.position);
                    movablePart.rotation = Quaternion.Slerp(movablePart.rotation, targetRotation, Time.deltaTime * (speed + 10));


                if (timer <= 0) 
                { 
                    timer = 0.1f; 
                }
                timer = timer - Time.deltaTime;

                if(player.autoGunCooldown <= 0)
                {
                    if (startShooting)
                        Shoot();
                }
            }
            else if(possibleTargets.Count != 0)
                CalculateNewTarget();

            if (currentTarget == null && movablePart.localRotation != Quaternion.identity)
            {
                movablePart.localRotation = Quaternion.Slerp(movablePart.localRotation, Quaternion.identity, Time.deltaTime * (speed + 5));
                if (movablePart.localRotation.eulerAngles.z <= 2 || movablePart.localRotation.eulerAngles.z <= -2)
                {
                    movablePart.localRotation = Quaternion.identity;
                }
            }
        }


    }

    public void AddPossibleTarget(GameObject go)
    {
        if (!possibleTargets.Contains(go))
            possibleTargets.Add(go);
    }

    public void RemovePossibleTarget(GameObject go)
    {
        if (possibleTargets.Contains(go))
            possibleTargets.Remove(go);
    }


    public void CalculateNewTarget()
    {
        ClearList();
        WaitTime(false);
        if (possibleTargets.Count != 0)
        {
            currentTarget = possibleTargets[0];

            foreach (GameObject go in possibleTargets)
            {
                if(go != null)
                    if (Vector2.Distance(transform.position, go.transform.position) <= Vector2.Distance(transform.position, currentTarget.transform.position))
                    {
                        currentTarget = go;
                    }
            }
        }
        if (currentTarget != null)
        {
            WaitTime(true);
        }
    }

    void ClearList()
    {
        possibleTargets.RemoveAll(item => item == null);

    }

    public void WaitTime(bool condition)
    {
        if (condition)
            Invoke("StartShoting", 0.2f);
        else
            startShooting = false;
    }

    void StartShoting()
    {
        startShooting = true;
    }

    public void Shoot()
    {
        if (!Builder.isBuilding && currentTarget!=null && player.autoGunCooldown <= 0)
        {
            anim.Play();
                player.Shoot(1, shootStart[0].position, movablePart.rotation);
                player.Shoot(1, shootStart[1].position, movablePart.rotation);
            shooteSide = !shooteSide;
        }
    }

    public void SetRange(float range, float reload)
    {
        radius.radius = range;
        speed = range;
    }
}
