using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public struct PlayerInfo
    {
        public Transform headTransform;
        public Player player;
        [CanBeNull] public AIBrain aiBrain; // This can be null if the player doesn't have an AIBrain component.
    }

    public Dictionary<int, PlayerInfo> playersDictionary = new Dictionary<int, PlayerInfo>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Register each player's AIBrain (if it exists) and head transform in the dictionary
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Player playerScript = player.GetComponent<Player>();
            Transform headTransform = playerScript.head;
            AIBrain aiBrain = player.GetComponent<AIBrain>(); // This will be null if there's no AIBrain component.
            RegisterPlayer(player, playerScript, aiBrain, headTransform);
        }
    }

    public void RegisterPlayer(GameObject obj, Player player, AIBrain aiBrain, Transform headTransform)
    {
        int id = obj.GetInstanceID();
        PlayerInfo info = new PlayerInfo
        {
            headTransform = headTransform,
            player = player,
            aiBrain = aiBrain // This can be null.
        };
        if (!playersDictionary.ContainsKey(id))
        {
            playersDictionary.Add(id, info);
        }
    }

    public void RemovePlayer(GameObject player)
    {
        int id = player.GetInstanceID();
        if (playersDictionary.ContainsKey(id))
        {
            playersDictionary.Remove(id);
        }
    }

    public PlayerInfo? GetPlayerInfo(GameObject obj)
    {
        int id = obj.GetInstanceID();
        if (playersDictionary.TryGetValue(id, out var playerInfo))
        {
            return playerInfo;
        }
        return null;
    }

    public void DamagePlayer(GameObject player, float damage)
    {
        PlayerInfo? info = GetPlayerInfo(player);
        if (info.HasValue)
        {
            info.Value.player.DecreaseHealth(damage);
        }
    }
}