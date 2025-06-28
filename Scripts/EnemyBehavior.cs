using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    
     EntityStats entityStats;
     GameObject player;
    InfosStats infoStats;
    void Start()
    {
        entityStats = gameObject.GetComponent<EntityStats>();
        player = GameObject.FindGameObjectWithTag("Player");
        infoStats = GameObject.FindGameObjectWithTag("Manager").GetComponent<InfosStats>();
    }

    
    void FixedUpdate()
    {
        moveToPlayer();
    }


    void moveToPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position , entityStats.moveSpeed_ * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(entityStats.damage_) ;
            Debug.Log(collision.gameObject.GetComponent<EntityStats>().hp_);
            entityStats.RemoveHp(entityStats.damage_);
        }
    }
}
