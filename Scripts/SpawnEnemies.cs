using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public float minDistanceFromPlayer = 10f; // Distância mínima de spawn do player
    public float maxDistanceFromPlayer = 20f; // Distância máxima de spawn do player
    public int enemiesPerSpawnCycle = 4; // Quantos inimigos tentar spawnar por ciclo
    private bool canSpawn;
    private float cooldown;

    [Header("Limites do Mapa")]
    public Transform mapBoundary; // Objeto que define os limites do mapa (deve ter um Collider 2D);
    private Bounds mapBounds;

    [Header("Tipos de Inimigos")]
    public List<GameObject> enemyTypes; // Lista de prefabs de inimigos

    [System.Serializable]
    public class EnemyTypeControl
    {
        public string enemyName;
        public int maxCount; // Quantidade máxima deste tipo
        public float spawnWeight; // Peso na probabilidade de spawn
        public float minSpawnTime; // Tempo mínimo (em segundos) para começar a spawnar este tipo
    }

    [Header("Controle de Tipos de Inimigos")]
    public List<EnemyTypeControl> enemyControls = new List<EnemyTypeControl>();

    [Header("Limites de Spawn")]
    public int enemiesCountMax = 60; // Máximo total de inimigos
    public int spawnRate = 500; // Taxa de spawn

    // Referências
    private GameObject player;
    private Dictionary<string, int> currentEnemyCounts = new Dictionary<string, int>();
    public GameObject[] enemiesArray;
    private InfosStats infoStats;
    private Transform playerTransform;

    void Start()
    {
        infoStats = GameObject.FindGameObjectWithTag("Manager").GetComponent<InfosStats>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        canSpawn = true;

        // Inicializar limites do mapa
        if (mapBoundary != null && mapBoundary.GetComponent<Collider2D>() != null)
        {
            mapBounds = mapBoundary.GetComponent<Collider2D>().bounds;
        }
        else if (mapBoundary != null && mapBoundary.GetComponent<Renderer>() != null)
        {
            mapBounds = mapBoundary.GetComponent<Renderer>().bounds;
        }
        else
        {
            Debug.LogWarning("Nenhum Collider2D ou Renderer encontrado no mapBoundary. Usando valores padrão para os limites.");
            mapBounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 10f));
        }

        // Inicializar contador de cada tipo de inimigo
        foreach (EnemyTypeControl control in enemyControls)
        {
            currentEnemyCounts[control.enemyName] = 0;
        }
    }

    void Update()
    {
        // Atualizar a lista de inimigos atuais
        enemiesArray = GameObject.FindGameObjectsWithTag("Enemy");

        // Atualizar contagens por tipo
        UpdateEnemyCounts();

        // Verificar se podemos spawnar
        if (canSpawn == true)
        {
            SpawnEnemiesAroundPlayer();
            canSpawn = false;
            cooldown = 0;
        }

        // Atualizar referência no InfosStats
        infoStats.enemiesSpawned = enemiesArray.ToList();

        // Controlar o cooldown de spawn
        Cooldown();
    }

    void UpdateEnemyCounts()
    {
        // Resetar contadores
        foreach (EnemyTypeControl control in enemyControls)
        {
            currentEnemyCounts[control.enemyName] = 0;
        }

        // Contar cada tipo de inimigo
        foreach (GameObject enemy in enemiesArray)
        {
            string enemyName = enemy.name.Replace("(Clone)", "");
            if (currentEnemyCounts.ContainsKey(enemyName))
            {
                currentEnemyCounts[enemyName]++;
            }
        }
    }

    void Cooldown()
    {
        if (cooldown > spawnRate && canSpawn == false)
        {
            canSpawn = true;
        }
        else
        {
            cooldown++;
        }
    }

    void SpawnEnemiesAroundPlayer()
    {
        // Verificar se não atingimos o limite total de inimigos
        if (enemiesArray.Length >= enemiesCountMax)
            return;

        // Tentar spawnar vários inimigos por ciclo
        for (int i = 0; i < enemiesPerSpawnCycle; i++)
        {
            // Verificar novamente o limite durante o ciclo
            if (enemiesArray.Length + i >= enemiesCountMax)
                break;

            // Gerar uma posição válida dentro dos limites do mapa
            Vector3 spawnPosition = GenerateValidSpawnPosition();

            // Se não foi possível encontrar uma posição válida, pular
            if (spawnPosition == Vector3.zero)
                continue;

            // Escolher qual tipo de inimigo spawnar baseado nas regras definidas
            GameObject enemyPrefab = ChooseEnemyType();

            // Se não tiver inimigo válido para spawnar, pular
            if (enemyPrefab == null)
                continue;

            // Criar o inimigo
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

            // Atualizar contadores especiais se necessário
            string enemyName = newEnemy.name.Replace("(Clone)", "");
            if (enemyName == "EnemyRanged")
            {
                infoStats.enemiesRangedSpawned++;
            }
        }
    }

    // Método para gerar uma posição de spawn válida dentro dos limites do mapa
    Vector3 GenerateValidSpawnPosition()
    {
        int maxAttempts = 10; // Número máximo de tentativas para encontrar uma posição válida

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Gerar um ângulo aleatório em radianos (0 a 2π)
            float randomAngle = Random.Range(0f, Mathf.PI * 2f);

            // Calcular direção a partir do ângulo
            Vector2 randomDirection = new Vector2(
                Mathf.Cos(randomAngle),
                Mathf.Sin(randomAngle)
            ).normalized;

            // Calcular distância aleatória entre min e max
            float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

            // Calcular posição potencial
            Vector3 potentialPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

            // Verificar se a posição está dentro dos limites do mapa
            if (IsPositionInsideMapBounds(potentialPosition))
            {
                return potentialPosition;
            }
        }

        // Se todas as tentativas falharem, retornar Vector3.zero como indicação de falha
        return Vector3.zero;
    }

    // Método para verificar se uma posição está dentro dos limites do mapa
    bool IsPositionInsideMapBounds(Vector3 position)
    {
        return mapBounds.Contains(position);
    }

    GameObject ChooseEnemyType()
    {
        // Lista de possíveis inimigos para spawnar neste momento
        List<EnemyTypeControl> availableEnemies = new List<EnemyTypeControl>();

        // Verificar quais inimigos podem ser spawnados agora
        foreach (EnemyTypeControl control in enemyControls)
        {
            // Verificar tempo mínimo para começar a spawnar este tipo
            if (infoStats.elapsedTime < control.minSpawnTime)
                continue;

            // Verificar se não atingimos o limite deste tipo
            if (currentEnemyCounts[control.enemyName] >= control.maxCount)
                continue;

            // Este inimigo está disponível para spawn
            availableEnemies.Add(control);
        }

        // Se não tiver nenhum inimigo disponível, retornar null
        if (availableEnemies.Count == 0)
            return null;

        // Calcular soma total dos pesos
        float totalWeight = 0;
        foreach (EnemyTypeControl enemy in availableEnemies)
        {
            totalWeight += enemy.spawnWeight;
        }

        // Escolher um inimigo baseado no peso
        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;

        foreach (EnemyTypeControl enemy in availableEnemies)
        {
            weightSum += enemy.spawnWeight;
            if (randomValue <= weightSum)
            {
                // Encontrar o prefab correspondente na lista
                foreach (GameObject prefab in enemyTypes)
                {
                    if (prefab.name == enemy.enemyName)
                    {
                        return prefab;
                    }
                }
            }
        }

        // Caso de fallback (não deveria chegar aqui normalmente)
        return enemyTypes.Count > 0 ? enemyTypes[0] : null;
    }
}