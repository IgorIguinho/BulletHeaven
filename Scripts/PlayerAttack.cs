using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet;

    [Header("Configurações de Ataque")]
    [Tooltip("Ângulo de dispersão para o tiro em escopeta (em graus)")]
    public float shotgunSpreadAngle = 45f;

    private bool isAttacking = false;
    private EntityStats entityStats;
    private GameObject player;
    private InfosStats infoStats;

    [Header("Sistema de Buff")]
    public bool shotgunBuffActive = false;
    public int shotgunBulletCount = 5;

    private void Start()
    {
        infoStats = GameObject.FindGameObjectWithTag("Manager").GetComponent<InfosStats>();
        entityStats = gameObject.GetComponent<EntityStats>();
        player = GameObject.FindGameObjectWithTag("Player");

        // Inicia a coroutine de ataque
        StartCoroutine(AttackRoutine());
    }

    private void Update()
    {
        // Você pode adicionar aqui lógica para ativar/desativar o buff, se necessário
        // Por exemplo: if (Input.GetKeyDown(KeyCode.B)) shotgunBuffActive = !shotgunBuffActive;
        shotgunBuffActive = entityStats.shotGunBuffActive;
    }

    // Coroutine principal de ataque
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Verifica se há inimigos para atacar
            if (infoStats.enemiesSpawned.Count > 0)
            {
                // Executa o tiro
                if (shotgunBuffActive)
                {
                    ShotgunAttack();
                }
                else
                {
                    RegularShot();
                }

                // Calcula o tempo de espera baseado na velocidade de ataque
                float attackDelay = CalculateAttackDelay();

                // Espera pelo tempo de cooldown
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                // Se não há inimigos, verifica novamente após um curto período
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    // Calcula o tempo entre os ataques com base nos stats do jogador
    private float CalculateAttackDelay()
    {
        // Converte a velocidade de ataque em segundos
        // Mesma lógica do seu cooldown anterior, mas convertida para segundos
        return entityStats.atkSpeed_ * ((100 - entityStats.bonusAtackSpeed) / 100) / 60f;
    }

    // Tiro normal em direção ao inimigo mais próximo
    private void RegularShot()
    {
        Transform targetEnemy = FindNearEnemy();

        if (targetEnemy != null)
        {
            // Cria o projétil
            GameObject bullet_ = Instantiate(bullet, transform.position, Quaternion.identity);
            bullet_.GetComponent<BulletDamage>().explosionBuffActive = entityStats.explosionBuffActive;
            // Configura o dano e tempo de vida
            BulletDamage bulletDamage = bullet_.GetComponent<BulletDamage>();
            bulletDamage.bulletDamage = entityStats.damage_;
            bulletDamage.bulletLife = 3;

            // Calcula a direção para o inimigo
            Vector2 bulletDirection = (targetEnemy.position - transform.position).normalized;

            // Aplica força ao projétil
            bullet_.GetComponent<Rigidbody2D>().AddForce(bulletDirection * entityStats.atkRange_, ForceMode2D.Impulse);
        }
    }

    // Tiro em escopeta (múltiplos projéteis em arco)
    private void ShotgunAttack()
    {
        Transform targetEnemy = FindNearEnemy();

        if (targetEnemy != null)
        {
            // Calcula a direção base para o inimigo
            Vector2 baseDirection = (targetEnemy.position - transform.position).normalized;

            // Calcula o ângulo inicial para distribuir os tiros
            float startAngle = -shotgunSpreadAngle / 2;
            float angleStep = shotgunSpreadAngle / (shotgunBulletCount - 1);

            // Cria cada projétil do tiro em escopeta
            for (int i = 0; i < shotgunBulletCount; i++)
            {
                // Calcula o ângulo para este projétil
                float currentAngle = startAngle + (angleStep * i);

                // Rotaciona a direção base pelo ângulo atual
                Vector2 bulletDirection = RotateVector(baseDirection, currentAngle);

                // Cria o projétil
                GameObject bullet_ = Instantiate(bullet, transform.position, Quaternion.identity);

                // Configura o dano e tempo de vida
                BulletDamage bulletDamage = bullet_.GetComponent<BulletDamage>();
                // No método Start() ou onde criar os projéteis
                bullet_.GetComponent<BulletDamage>().explosionBuffActive = entityStats.explosionBuffActive;
                bulletDamage.bulletDamage = entityStats.damage_;
                bulletDamage.bulletLife = 3;

                // Aplica força ao projétil
                bullet_.GetComponent<Rigidbody2D>().AddForce(bulletDirection * entityStats.atkRange_, ForceMode2D.Impulse);
            }
        }
    }

    // Função para rotacionar um vetor por um ângulo em graus
    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    // Encontra o inimigo mais próximo ao jogador
    public Transform FindNearEnemy()
    {
        float closeDistanceToPlayer = Mathf.Infinity;
        Transform closeEnemy = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemie = Vector2.Distance(player.transform.position, enemy.transform.position);
            if (distanceToEnemie < closeDistanceToPlayer)
            {
                closeDistanceToPlayer = distanceToEnemie;
                closeEnemy = enemy.transform;
            }
        }

        return closeEnemy;
    }
}