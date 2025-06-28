using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet;

    [Header("Configura��es de Ataque")]
    [Tooltip("�ngulo de dispers�o para o tiro em escopeta (em graus)")]
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
        // Voc� pode adicionar aqui l�gica para ativar/desativar o buff, se necess�rio
        // Por exemplo: if (Input.GetKeyDown(KeyCode.B)) shotgunBuffActive = !shotgunBuffActive;
        shotgunBuffActive = entityStats.shotGunBuffActive;
    }

    // Coroutine principal de ataque
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            // Verifica se h� inimigos para atacar
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
                // Se n�o h� inimigos, verifica novamente ap�s um curto per�odo
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    // Calcula o tempo entre os ataques com base nos stats do jogador
    private float CalculateAttackDelay()
    {
        // Converte a velocidade de ataque em segundos
        // Mesma l�gica do seu cooldown anterior, mas convertida para segundos
        return entityStats.atkSpeed_ * ((100 - entityStats.bonusAtackSpeed) / 100) / 60f;
    }

    // Tiro normal em dire��o ao inimigo mais pr�ximo
    private void RegularShot()
    {
        Transform targetEnemy = FindNearEnemy();

        if (targetEnemy != null)
        {
            // Cria o proj�til
            GameObject bullet_ = Instantiate(bullet, transform.position, Quaternion.identity);
            bullet_.GetComponent<BulletDamage>().explosionBuffActive = entityStats.explosionBuffActive;
            // Configura o dano e tempo de vida
            BulletDamage bulletDamage = bullet_.GetComponent<BulletDamage>();
            bulletDamage.bulletDamage = entityStats.damage_;
            bulletDamage.bulletLife = 3;

            // Calcula a dire��o para o inimigo
            Vector2 bulletDirection = (targetEnemy.position - transform.position).normalized;

            // Aplica for�a ao proj�til
            bullet_.GetComponent<Rigidbody2D>().AddForce(bulletDirection * entityStats.atkRange_, ForceMode2D.Impulse);
        }
    }

    // Tiro em escopeta (m�ltiplos proj�teis em arco)
    private void ShotgunAttack()
    {
        Transform targetEnemy = FindNearEnemy();

        if (targetEnemy != null)
        {
            // Calcula a dire��o base para o inimigo
            Vector2 baseDirection = (targetEnemy.position - transform.position).normalized;

            // Calcula o �ngulo inicial para distribuir os tiros
            float startAngle = -shotgunSpreadAngle / 2;
            float angleStep = shotgunSpreadAngle / (shotgunBulletCount - 1);

            // Cria cada proj�til do tiro em escopeta
            for (int i = 0; i < shotgunBulletCount; i++)
            {
                // Calcula o �ngulo para este proj�til
                float currentAngle = startAngle + (angleStep * i);

                // Rotaciona a dire��o base pelo �ngulo atual
                Vector2 bulletDirection = RotateVector(baseDirection, currentAngle);

                // Cria o proj�til
                GameObject bullet_ = Instantiate(bullet, transform.position, Quaternion.identity);

                // Configura o dano e tempo de vida
                BulletDamage bulletDamage = bullet_.GetComponent<BulletDamage>();
                // No m�todo Start() ou onde criar os proj�teis
                bullet_.GetComponent<BulletDamage>().explosionBuffActive = entityStats.explosionBuffActive;
                bulletDamage.bulletDamage = entityStats.damage_;
                bulletDamage.bulletLife = 3;

                // Aplica for�a ao proj�til
                bullet_.GetComponent<Rigidbody2D>().AddForce(bulletDirection * entityStats.atkRange_, ForceMode2D.Impulse);
            }
        }
    }

    // Fun��o para rotacionar um vetor por um �ngulo em graus
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

    // Encontra o inimigo mais pr�ximo ao jogador
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