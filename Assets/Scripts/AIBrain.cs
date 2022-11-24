using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIBrain : MonoBehaviour
{
    private NavMeshAgent _agent;

    public string mode = "patrol";
    private float maxAngle = 60;
    public LayerMask seeEnemyMask;
    public Transform head;
    public Transform enemy;

    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(head.position, head.forward, Color.cyan);

        // if (!enemy)
        // {
            enemy = FindEnemyInVision();
        // }

        // if you see an enemy
        // enemy = GameManager.instance.playersTree.FindClosestWithCondition(transform.position, IsValidEnemy);
        if (enemy)
        {
            mode = "attack";
            Debug.DrawLine(head.position, enemy.position, Color.white);

            // every frame we have to check if that enemy is still visible, if not we look again or go back to patrolling.
        }

        if (mode == "patrol")
        {
            // if (DidReachDestination())
            // {
            //     SetNewDestination();
            // }
        }
        else if (mode == "attack")
        {
            Vector3 headRotation = Quaternion.LookRotation(enemy.position - head.position).eulerAngles;
            Vector3 bodyRotation = headRotation;

            headRotation.y = 0f;
            headRotation.z = 0f;

            bodyRotation.x = 0f;
            bodyRotation.z = 0f;

            transform.rotation = Quaternion.Euler(bodyRotation);
            head.localRotation = Quaternion.Euler(headRotation);

            // head.LookAt(enemy, Vector3.up);
        }

        if (DidReachDestination())
        {
            SetNewDestination();
        }
    }

    private Transform MaybeVisibleEnemy(Transform otherPlayer)
    {
        if (Physics.Linecast(head.position, otherPlayer.position, out var hit))
        {
            // you hit the other player's body
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
                hit.collider.transform.root.transform == otherPlayer.parent.transform)
            {
                return otherPlayer;
            }

        }
            // something obscuring other player
            return null;
    }

    void OnDrawGizmos()
    {
        Handles.Label(head.position, angle.ToString());
    }

    private Transform FindEnemyInVision()
    {
        // Transform enemy;

        // you have one player
        // for all other players, find which are visible to this player
        foreach (var otherPlayer in GameManager.instance.players)
        {
            // don't return your self
            if (transform == otherPlayer)
            {
                continue;
            }


            var transform1 = head.transform;

            // otherPlayer.position should be the position of their head now...
            Vector3 relativeNormalizedPos = (otherPlayer.position - transform1.position).normalized;
            float dot = Vector3.Dot(relativeNormalizedPos, transform1.forward);

            // angle difference between looking direction and direction to item (radians)
            angle = Mathf.Acos(dot);
            // print(angle *  Mathf.Rad2Deg);

            float maxAngleRadians = Mathf.Deg2Rad * maxAngle;

            if (angle > maxAngleRadians)
            {
                // outside of player's vision
                continue;
            }

            Debug.DrawLine(head.position, otherPlayer.position, Color.red);

            if (Physics.Linecast(transform1.position, otherPlayer.position, out var hit))
            {
                // you hit the other player's body
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player") &&
                    hit.collider.transform.root.transform == otherPlayer.parent.transform)
                {
                    return otherPlayer;
                }

                // something obscuring other player
                continue;
            }

            return otherPlayer;
        }

        return null;
    }

    private bool IsValidEnemy(Transform obj)
    {
        // is in FOV
        Transform enemyHead = obj;
        var transform1 = transform;
        Vector3 relativeNormalizedPos = (enemyHead.position - transform1.position).normalized;

        float dot = Vector3.Dot(relativeNormalizedPos, transform1.forward);

        // angle difference between looking direction and direction to item (radians)
        float angle = Mathf.Acos(dot);

        float maxAngleRadians = Mathf.Deg2Rad * maxAngle;

        if (angle > maxAngleRadians)
        {
            // this enemy is within player's FOV
//                print(angle);
            return false;
//                break;
        }


        // is able to be seen
        if (Physics.Linecast(transform.position, enemyHead.transform.position, out _, seeEnemyMask,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        // cannot be myself
        if (obj == transform1)
        {
            return false;
        }

        return true;
    }

    bool DidReachDestination()
    {
        if (_agent.pathPending) return false;
        if (!(_agent.remainingDistance <= _agent.stoppingDistance)) return false;
        return !_agent.hasPath || _agent.velocity.sqrMagnitude == 0f;
    }

    void SetNewDestination()
    {
        NavMesh.SamplePosition(new Vector3(Random.Range(-5, 5), Random.Range(0, 0), Random.Range(-5, 5)),
            out var hit, Mathf.Infinity, NavMesh.AllAreas);
        var myRandomPositionInsideNavMesh = hit.position;
        _agent.destination = myRandomPositionInsideNavMesh;
    }
}