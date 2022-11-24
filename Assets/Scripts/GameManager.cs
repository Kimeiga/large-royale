using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{

    // must be singleton
    public static GameManager instance = null;

    // public GameObject enemy;



    // I feel like it makes the most sense to just store the Transforms of people's heads.
    // because that's what the visibility calculations are going to be using anyways.
    public List<Transform> players = new List<Transform>();
    public float maxAngle = 120;

    public LayerMask seeEnemyMask;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // for (int i = 0; i < 1000; i++)
        // {
        //     NavMesh.SamplePosition(new Vector3(Random.Range(-50, 50), Random.Range(0, 20), Random.Range(-50, 50)), out var hit, Mathf.Infinity, NavMesh.AllAreas);
        //     var myRandomPositionInsideNavMesh = hit.position;
        //
        //     Instantiate(enemy, myRandomPositionInsideNavMesh, Quaternion.identity);
        // }

        // add all existing AIs in the scene to the players List
        players.AddRange(GameObject.FindGameObjectsWithTag("Player").ToList().ConvertAll<Transform>(go => go.GetComponent<AIBrain>().head));
    }

    // Update is called once per frame
    void Update()
    {
        // // better to do this per player if we can't optimize this down from O(n^2)
        // // For all player positions, find which are visible to which
        // foreach (var player in players)
        // {
        //     Transform enemy;
        //
        //     // you have one player
        //     // for all other players, find which are visible to this player
        //     foreach (var otherPlayer in players)
        //     {
        //         // don't return your self
        //         if (player == otherPlayer)
        //         {
        //             continue;
        //         }
        //
        //         Vector3 relativeNormalizedPos = (otherPlayer.position - player.position).normalized;
        //         float dot = Vector3.Dot(relativeNormalizedPos, player.forward);
        //
        //         // angle difference between looking direction and direction to item (radians)
        //         float angle = Mathf.Acos(dot);
        //
        //         float maxAngleRadians = Mathf.Deg2Rad * maxAngle;
        //
        //         if(angle > maxAngleRadians)
        //         {
        //             // outside of player's vision
        //             continue;
        //         }
        //
        //         if (Physics.Linecast(player.position, player.position, out _, seeEnemyMask,
        //             QueryTriggerInteraction.Ignore))
        //         {
        //             // something obscuring other player
        //             continue;
        //         }
        //
        //         enemy = otherPlayer;
        //         break;
        //     }
        // }
    }
}