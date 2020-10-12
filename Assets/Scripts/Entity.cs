using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int health = 100;
    public void Damage(int damageAmount)
    {
        //subtract damage amount when Damage function is called
        health -= damageAmount;

        //Check if health has fallen below zero
        if (health <= 0) 
        {
            //if health has fallen below zero, destroy it 
            Destroy(gameObject);
        }
    }
}
