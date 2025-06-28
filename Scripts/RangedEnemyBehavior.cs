using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RangedEnemyBehavior : MonoBehaviour
{
    Transform playerTranform;
    EntityStats entityStats;
    public GameObject bullet;
    public float distMaxBullet;
    public float distMaxMov;
    

    // Start is called before the first frame update
    void Start()
    {
        playerTranform = GameObject.FindGameObjectWithTag("Player").transform;
        entityStats = gameObject.GetComponent<EntityStats>();
        StartCoroutine(ShotBullet());
    }

    // Update is called once per frame
    void Update()
    {
        moveToPlayer();  
    }



    void moveToPlayer()
    {

        float dist = Vector2.Distance(transform.position, playerTranform.position);
        if (dist > distMaxMov)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTranform.position, entityStats.moveSpeed_ * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(entityStats.damage_);
            Debug.Log(collision.gameObject.GetComponent<EntityStats>().hp_);
            entityStats.RemoveHp(entityStats.hpMax_ + 1);
        }
    }
    IEnumerator ShotBullet()
    {
        float dist = Vector2.Distance(transform.position, playerTranform.position);

        if (dist < distMaxBullet)
        {
        GameObject bullet_ = Instantiate(bullet, transform.position, Quaternion.identity);
        bullet_.gameObject.GetComponent<BulletDamage>().bulletDamage = entityStats.damage_;
        bullet_.gameObject.GetComponent<BulletDamage>().bulletLife = 2;
        Vector2 bulletDirection = (playerTranform.position - transform.position).normalized;

        bullet_.GetComponent<Rigidbody2D>().AddForce(bulletDirection * entityStats.atkRange_, ForceMode2D.Impulse);
        bullet_.GetComponent<Rigidbody2D>().velocity = bulletDirection * entityStats.atkRange_;
        }
        yield return new WaitForSeconds(entityStats.atkSpeed_);
        StartCoroutine(ShotBullet());

    }
   
}
