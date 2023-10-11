using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGun : MonoBehaviour
{
    /// <summary>
    /// The purpose of this script is to give the AIs the ability to attack one another while they are in the state of
    /// looking at each other via AIBrain.cs
    /// </summary>
    public float damage = 10;
    public float damageVariability = 5;
    public float fireRate = 0.3f;
    private float lastFire = 0;
    public LayerMask layerMask = ~0; // everything
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Fire(Vector3 origin, Vector3 direction)
    {
        // fire a raycast out and do damage

        if (!(Time.time > lastFire + fireRate)) return;
        // enough time has passed since the last shot

        if (!Physics.Raycast(origin, direction, out hit, layerMask)) return;
        // we hit something

        if (!hit.transform.CompareTag("Player")) return;
        // we hit a player

        if (!hit.transform != transform) return;
        // we didn't hit ourself

        // tell the Game Manager to damage that player
        GameManager.instance.DamagePlayer(hit.transform.gameObject,
            Random.Range(damage - damageVariability, damage + damageVariability));

        //
        // AIBrain enemyBrain = hit.transform.GetComponent<AIBrain>();
        //
        // if (!enemyBrain) return;
        //
        // enemyBrain.DecreaseHealth(Random.Range(damage - damageVariability, damage + damageVariability));

        lastFire = Time.time;
    }
}