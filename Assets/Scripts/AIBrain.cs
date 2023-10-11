using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIBrain : MonoBehaviour
{
    private NavMeshAgent agent;

    public enum AIAction
    {
        Idle,
        Attack,
        Wander
    }

    private Player playerScript;

    public LayerMask seeEnemyMask;
    // public Transform enemy;
    public Player enemy;

    public bool debugFlag = false;

    public float damage = 10;
    public float damageVariability = 5;
    public float fireRate = 0.3f;
    private float lastFire = 0;

    private AIAction bestAction;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
        playerScript = GetComponent<Player>();
    }



    AIAction GetBestAction()
    {
        if (enemy)
            return AIAction.Attack;

        if (DidReachDestination())
            return AIAction.Wander;

        return AIAction.Idle;
    }

    public void OnEnemySpotted(Player enemyPlayer)
    {
        enemy = enemyPlayer;
    }

    public void OnEnemyLost()
    {
        print(enemy);
        enemy = null;
    }

    void Update()
    {

        switch (bestAction)
        {
            case AIAction.Attack:
                HandleAttack();
                break;

            case AIAction.Wander:
                HandleWander();
                break;

            case AIAction.Idle:
                HandleIdle();
                break;
        }
    }

    private void FixedUpdate()
    {
        bestAction = GetBestAction();

    }

    void HandleIdle()
    {
        // Handle idle behaviors here
    }

    void HandleWander()
    {
        SetNewDestination();
    }

    void HandleAttack()
    {
        AimAndShootAtEnemy();
        // else part removed as it's now handled by OnEnemyLost method.
    }


    void AimAndShootAtEnemy()
    {
        Vector3 headRotation = Quaternion.LookRotation(enemy.head.position - playerScript.head.position).eulerAngles;
        Vector3 bodyRotation = headRotation;

        headRotation.y = 0f;
        headRotation.z = 0f;

        bodyRotation.x = 0f;
        bodyRotation.z = 0f;

        transform.rotation = Quaternion.Euler(bodyRotation);
        playerScript.head.localRotation = Quaternion.Euler(headRotation);

        if (Time.time > lastFire + fireRate)
        {
            if (!enemy)
            {
                print(GetBestAction());
                print(enemy);
            }

            enemy.DecreaseHealth(Random.Range(damage - damageVariability, damage + damageVariability));
            lastFire = Time.time;
            // Draw debug line between characters that are fighting
            Debug.DrawLine(playerScript.head.position, enemy.head.position, Color.red);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // this ray just represents where they are looking.
    //     if (debugFlag)
    //         Debug.DrawRay(head.position, head.forward, Color.cyan);
    //
    //     if (!enemy)
    //     {
    //         mode = "idle";
    //     }
    //
    //     if (mode == "idle")
    //     {
    //         enemy = FindEnemyInVision();
    //         if (enemy)
    //         {
    //             enemyBrain = enemy.GetComponent<AIBrain>();
    //             mode = "attack";
    //         }
    //     }
    //     else if (mode == "attack")
    //     {
    //         if (debugFlag)
    //         {
    //             Debug.DrawLine(head.position, enemy.position, Color.white);
    //         }
    //
    //         var visibleHead = MaybeVisibleEnemy(enemy);
    //         if (!visibleHead)
    //         {
    //             mode = "idle";
    //         }
    //         else
    //         {
    //             print(enemy);
    //             // aim at enemy:
    //
    //             Vector3 headRotation = Quaternion.LookRotation(enemy.position - head.position).eulerAngles;
    //             Vector3 bodyRotation = headRotation;
    //
    //             headRotation.y = 0f;
    //             headRotation.z = 0f;
    //
    //             bodyRotation.x = 0f;
    //             bodyRotation.z = 0f;
    //
    //             transform.rotation = Quaternion.Euler(bodyRotation);
    //             head.localRotation = Quaternion.Euler(headRotation);
    //
    //             if (Time.time > lastFire + fireRate)
    //             {
    //                 // "shoot" enemy (aka decrease their health)
    //                 if (!enemyBrain)
    //                 {
    //                     print('w');
    //                 }
    //
    //                 enemyBrain.DecreaseHealth(Random.Range(damage - damageVariability, damage + damageVariability));
    //
    //                 lastFire = Time.time;
    //             }
    //
    //         }
    //     }
    //
    //     if (DidReachDestination())
    //     {
    //         SetNewDestination();
    //     }
    // }

    // private Transform MaybeVisibleEnemy(Transform otherPlayer)
    // {
    //     if (Physics.Linecast(head.position, otherPlayer.position, out var hit))
    //     {
    //         // you hit the other player's body
    //         if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
    //             hit.collider.transform.root.transform == otherPlayer.parent.transform)
    //         {
    //             return otherPlayer;
    //         }
    //     }
    //
    //     if (hit.collider)
    //     {
    //         print(hit.collider.gameObject);
    //         print(hit.collider.transform.root.transform);
    //     }
    //
    //     // something obscuring other player
    //     return null;
    // }
    //
    // // ok i feel like we only need to call this once when the enemy comes into vision and when it exits the vision
    // // since the ideal way to fight even if another enemy comes into view is to just finish off the one that you were
    // // already attacking before moving on to the next one.
    // // The only time that this wouldn't be the ideal strategy would be if you could somehow see the health bars of all the
    // // enemies and a really low health enemy comes into view while you were fighting a high health enemy; like yeah you would
    // // switch targets.
    //
    // // or if your team said that that person was one shot or low and you targeted them first even in a group of two.
    // // but i think for now we will assume that that information is hidden...
    //
    // // so now what we want to do is just call find enemy in vision when there currently is no enemy,
    // // then we have an enemy and we fight them until they die or they pass out of vision
    // // which means that we need to check line of sight on the enemy every frame.
    //
    // private Transform FindEnemyInVision()
    // {
    //     // you have one player
    //     // for all other players, find which are visible to this player
    //     foreach (var otherPlayer in GameManager.instance.players)
    //     {
    //         // don't return your self
    //         if (transform == otherPlayer)
    //         {
    //             continue;
    //         }
    //
    //
    //         var transform1 = head.transform;
    //
    //         // otherPlayer.position should be the position of their head now...
    //         Vector3 relativeNormalizedPos = (otherPlayer.position - transform1.position).normalized;
    //         float dot = Vector3.Dot(relativeNormalizedPos, transform1.forward);
    //
    //         // angle difference between looking direction and direction to item (radians)
    //         float angle = Mathf.Acos(dot);
    //         // print(angle *  Mathf.Rad2Deg);
    //
    //         float maxAngleRadians = Mathf.Deg2Rad * maxAngle;
    //
    //         if (angle > maxAngleRadians)
    //         {
    //             // outside of player's vision cone
    //             continue;
    //         }
    //
    //         if (debugFlag)
    //             Debug.DrawLine(head.position, otherPlayer.position, Color.red);
    //
    //         // now we know that they are inside the vision cone so
    //         // we want to see if there's line of sight on them
    //
    //         if (Physics.Linecast(transform1.position, otherPlayer.position, out var hit))
    //         {
    //             // you hit the other player's body
    //             if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
    //                 hit.collider.transform.root.transform == otherPlayer.parent.transform)
    //             {
    //                 return otherPlayer;
    //             }
    //
    //             // something obscuring other player
    //             continue;
    //         }
    //
    //         // return otherPlayer;
    //     }
    //
    //     return null;
    // }

    bool DidReachDestination()
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance) return false;
        return !agent.hasPath || agent.velocity.sqrMagnitude == 0f;
    }

    void SetNewDestination()
    {
        NavMesh.SamplePosition(new Vector3(Random.Range(-50, 50), Random.Range(-5, 5), Random.Range(-50, 50)),
            out var hit, Mathf.Infinity, NavMesh.AllAreas);
        var myRandomPositionInsideNavMesh = hit.position;
        agent.destination = myRandomPositionInsideNavMesh;
    }
}