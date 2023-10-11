using UnityEngine;
using System.Collections.Generic;

public class VisibilityManager : MonoBehaviour
{
    public float maxAngle = 60;
    public LayerMask seeEnemyMask;
    private Dictionary<int, GameManager.PlayerInfo> players => GameManager.instance.playersDictionary;

    // List to store all AIBrain instances in the scene.
    // public List<AIBrain> allAIBrains = new List<AIBrain>();

    private void Start()
    {
        // Populate the list with all AIBrains on startup.
        // allAIBrains.AddRange(FindObjectsOfType<AIBrain>());
    }

    private void FixedUpdate()
    {
        CheckVisibilityForAll();
    }

    void CheckVisibilityForAll()
    {
        var allPlayerInfos = new List<GameManager.PlayerInfo>(players.Values);


        for (int i = 0; i < allPlayerInfos.Count; i++)
        {
            for (int j = i + 1; j < allPlayerInfos.Count; j++)
            {
                var p1 = allPlayerInfos[i];
                var p2 = allPlayerInfos[j];

                // Debug.DrawLine(ai1.head.position, ai2.head.position, Color.white);
                // Debug.DrawRay(position, p2.player.head.forward, Color.cyan);
                Debug.DrawRay(p1.player.head.position, p1.player.head.forward, Color.cyan);

                Player ai1 = allPlayerInfos[i].player;
                Player ai2 = allPlayerInfos[j].player;


                if (p1.aiBrain)
                {
                    // Check if ai1 can see ai2.
                    if (IsVisible(p1.player.head, p2.player.head))
                    {
                        if (!p1.aiBrain.enemy)
                        {
                            Debug.DrawLine(ai1.head.position, ai2.head.position, Color.red);
                            p1.aiBrain.OnEnemySpotted(ai2);

                        }
                    }
                    else if (p1.aiBrain.enemy == ai2) // ai1 lost sight of ai2
                    {
                        if (p1.aiBrain.enemy)
                        {
                            Debug.DrawLine(ai1.head.position, ai2.head.position, Color.blue);

                            p1.aiBrain.OnEnemyLost();
                        }
                    }
                }

                if (p2.aiBrain)
                {
                    // Check if ai2 can see ai1.
                    if (IsVisible(p2.player.head, p1.player.head))
                    {
                        if (!p2.aiBrain.enemy)
                        {
                            Debug.DrawLine(ai1.head.position, ai2.head.position, Color.yellow);
                            p2.aiBrain.OnEnemySpotted(ai1);

                        }
                    }
                    else if (p2.aiBrain.enemy == ai1) // ai2 lost sight of ai1
                    {
                        if (p2.aiBrain.enemy)
                        {
                            Debug.DrawLine(ai1.head.position, ai2.head.position, Color.green);

                            p2.aiBrain.OnEnemyLost();
                        }
                    }
                }
            }
        }
    }

    bool IsVisible(Transform observer, Transform target)
    {
        Vector3 directionToTarget = (target.position - observer.position).normalized;
        float dot = Vector3.Dot(directionToTarget, observer.forward);

        // Check if target is within observer's field of view.
        if (Mathf.Acos(dot) * Mathf.Rad2Deg < maxAngle)
        {
            // Check line of sight using raycast.
            if (!Physics.Linecast(observer.position, target.position, out RaycastHit hit, seeEnemyMask))
            {
                return true;
            }

            if (hit.transform == target)
            {
                return true;
            }
        }

        return false;
    }
}