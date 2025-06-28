using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityStats : MonoBehaviour
{
    public float hpMax_;
    public float hp_;
    public float damage_;
    public float atkSpeed_;
    public float moveSpeed_;
    public float atkRange_;

    public AudioClip damageClip;
    public AudioSource audioSource;


    public float xpTotal_;
    public float xp;

    //oonly mobs
    public float xpDrop_;

    [Header ("OnlyPlayers")]
    public bool explosionBuffActive;
    public bool shotGunBuffActive;
    public int levelForUp;
    public int level;
    public float bonusAtackSpeed;
    public float bonusMovSpeed;


    Hud hud;
    GameObject player;
    InfosStats infoStats;

    private void Start()
    {
        infoStats = GameObject.FindGameObjectWithTag("Manager").GetComponent<InfosStats>();
        player = GameObject.FindGameObjectWithTag("Player");
        hud = GameObject.FindGameObjectWithTag("Hud").GetComponent<Hud>();
        if ( gameObject.name != "Player")
        {
            audioSource = GameObject.FindGameObjectWithTag("AudioSourceEnemy").GetComponent<AudioSource>();
        }
        level = 1;
       hp_ = hpMax_;
        
    }

    private void Update()
    {
    
    }

    public void AddXp(float xpAdd)
    {
        xp += xpAdd;
        if (xp >= level * 100)
        {
            xp = 0;
            levelForUp++;  

        }
    }

    void Dead()
    {
        if (hp_ <= 0)
        {
            if (this.gameObject.tag != "Player")
            {
               player.GetComponent<EntityStats>().AddXp(xpDrop_);
                if (this.gameObject.name == "EnemyRanged(Clone)")
                {
                    infoStats.enemiesRangedSpawned -= 1;

                }
            }
            if (this.gameObject.tag == "Player")
            {
               hud.canvaRestart.SetActive(true);
            }
            Destroy(this.gameObject);
        }

    }

    public void RemoveHp(float hpRemoved)
    {
        GameObject newPopup_ = Instantiate(Hud.Instance.DamagePopUp, this.gameObject.transform.position, Quaternion.identity);
        newPopup_.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f, 2f), 5), ForceMode2D.Impulse);
        newPopup_.GetComponentInChildren<Text>().text = hpRemoved.ToString();
        Destroy(newPopup_, 1);

        hp_ -= hpRemoved;
        Dead();
           
    }
}

