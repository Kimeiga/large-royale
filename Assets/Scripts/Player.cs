using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float maxHealth = 100;
    public float health;

    public Transform head;

    private void Start()
    {
        health = maxHealth;
    }

    public void DecreaseHealth(float amount)
    {
        health -= amount;

        // dumb way to die at first
        if (health <= 0)
        {
            // remove the game object from the game manager
            GameManager.instance.RemovePlayer(gameObject);
            Destroy(gameObject);
        }
    }

    public void IncreaseHealth(float amount)
    {
        // for when you eventually have health packs
        health += amount;
    }
}