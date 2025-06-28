using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfosStats : MonoBehaviour
{
    PlayerAttack playerAttack;
    SpawnEnemies spawnEnemies;

    public List<GameObject> enemiesSpawned;
    public int enemiesRangedSpawned;
    public bool pauseGeral;

    public float elapsedTime;
    public Text timer;
 


    void Start()
    {
        pauseGeral = false;
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
        spawnEnemies = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SpawnEnemies>(); 
    }


    void Update()
    {
        
       PauseGame();
        timerGame();

        if (elapsedTime > 180) 
        {
            SceneManager.LoadScene("WonScene");
        }
    }

    void PauseGame()
    {
        if (pauseGeral == true)
        {
            playerAttack.enabled = false;
            spawnEnemies.enabled = false;
        }
        else
        {
            playerAttack.enabled = true;
            spawnEnemies.enabled = true;
        }
    }

    void timerGame()
    {
        elapsedTime += Time.deltaTime;
        timer.text = FormatTime(elapsedTime);
    }
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60); // Calcula os minutos
        int seconds = Mathf.FloorToInt(time % 60); // Calcula os segundos
        return string.Format("{0:00}:{1:00}", minutes, seconds); // Formata para exibição
    }
}
