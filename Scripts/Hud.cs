using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{

    public static Hud Instance { get; private set; }
    public GameObject levelUpScreen;
    public GameObject upSkillScreen;
    public bool canOpenSkillScreen;


    public Slider hpBar;
    public Slider xpBar;

    public Text pointForUp;
    public GameObject DamagePopUp;
    public GameObject canvaRestart;

    EntityStats entityStats;
    InfosStats infoStats;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        infoStats = GameObject.FindGameObjectWithTag("Manager").GetComponent<InfosStats>();
        entityStats = GameObject.FindGameObjectWithTag("Player").GetComponent<EntityStats>();
        canOpenSkillScreen = true;
    }

    
    void Update()
    {
        if (entityStats.level < 4) { canOpenSkillScreen = true; }  
        else if (entityStats.level > 6 && entityStats.level < 9) { canOpenSkillScreen = true; }
        LevelUpScreen();
        BarSlider();
        SkillUpScreen();
    }

    void BarSlider()
    {
        hpBar.maxValue = entityStats.hpMax_;
        hpBar.value = entityStats.hp_;

        xpBar.maxValue = entityStats.level * 100;
        xpBar.value = entityStats.xp;
    }

    void LevelUpScreen()
    {
        if (entityStats.levelForUp > 0) 
        { 
            levelUpScreen.SetActive(true);
            pointForUp.text = "Pontos " + entityStats.levelForUp;
            Time.timeScale = 0;
            infoStats.pauseGeral = true;
        }
        else { levelUpScreen.SetActive(false);
            Time.timeScale = 1;
            infoStats.pauseGeral = false;
        }
    }

    void SkillUpScreen()
    {
        if (entityStats.level == 5 && canOpenSkillScreen || entityStats.level == 10  && canOpenSkillScreen)
        {
            upSkillScreen.SetActive(true);
            Time.timeScale = 0;
            infoStats.pauseGeral = true;
        }
        else if (!canOpenSkillScreen)
        {
            upSkillScreen.SetActive(false);
            Time.timeScale = 1;
            infoStats.pauseGeral = false;
        }

    }

    public  void SkillUp(string skill)
    {
        if (skill == "Doze")
        {
            entityStats.shotGunBuffActive = true;
        }else if (skill == "Explosion")
        {
            entityStats.explosionBuffActive = true;
        }
        canOpenSkillScreen = false;
    }

    public void SelectStats(string stats)
    {
        if (stats == "hp")
        {
            entityStats.hpMax_ += 5;
            entityStats.hp_ += 5;
            entityStats.levelForUp -= 1;
            entityStats.level += 1;
        }
        else if(stats == "atk" )
        { entityStats.damage_ += 2;
            entityStats.levelForUp -= 1;
            entityStats.level += 1;
        }
        else if (stats =="atkSpeed")
        { entityStats.bonusAtackSpeed += 10;
            entityStats.levelForUp -= 1;
            entityStats.level += 1;
        }
        else if (stats == "movSpeed")
        { entityStats.bonusMovSpeed += 50;
            entityStats.levelForUp -= 1;
            entityStats.level += 1;
        }


    }
}
