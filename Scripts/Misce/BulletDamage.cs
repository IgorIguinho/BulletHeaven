using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [Header("Configura��es B�sicas")]
    public float bulletDamage;
    public float bulletLife;
    public bool isPlayer;

    [Header("Configura��es de Explos�o")]
    public bool explosionBuffActive = false;
    public float explosionRadius = 3f;
    public float explosionDamageMultiplier = 0.5f;
    public float explosionDuration = 0.3f;

    [Header("Refer�ncias")]
    public GameObject explosionEffect; // Opcional: prefab de efeito visual

    private GameObject playerStats;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D explosionCollider;
    private Color originalColor;
    private bool hasExploded = false;
    private HashSet<int> damagedEnemies = new HashSet<int>();

    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Salva a cor original do sprite
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Adiciona um collider circular para a explos�o (desativado inicialmente)
        explosionCollider = gameObject.AddComponent<CircleCollider2D>();
        explosionCollider.radius = explosionRadius;
        explosionCollider.isTrigger = true;
        explosionCollider.enabled = false;

        // Configura a destrui��o autom�tica ap�s o tempo de vida
        Destroy(gameObject, bulletLife);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evita processar colis�es m�ltiplas ou colis�es ap�s a explos�o j� ter iniciado
        if (hasExploded || collision.GetInstanceID() == explosionCollider.GetInstanceID())
            return;

        // L�gica para proj�til do jogador atingindo inimigo
        if (collision.gameObject.CompareTag("Enemy") && isPlayer)
        {
            // Marca o inimigo como j� tendo recebido dano direto
            int enemyID = collision.gameObject.GetInstanceID();
            damagedEnemies.Add(enemyID);

            // Causa dano ao inimigo atingido diretamente
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(bulletDamage);

            // Se o buff de explos�o estiver ativo, executa a explos�o
            if (explosionBuffActive)
            {
                hasExploded = true;
                StartCoroutine(ExplodeCoroutine());
            }
            else
            {
                // Se n�o for explodir, destr�i o proj�til normalmente
                Destroy(gameObject);
            }
        }
        // L�gica para proj�til de inimigo atingindo o jogador
        else if (collision.gameObject.CompareTag("Player") && !isPlayer)
        {
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(bulletDamage);
            Destroy(gameObject);
        }
        // Colis�o com paredes
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Se o buff de explos�o estiver ativo, executa a explos�o mesmo em paredes
            if (explosionBuffActive && isPlayer && !hasExploded)
            {
                hasExploded = true;
                StartCoroutine(ExplodeCoroutine());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator ExplodeCoroutine()
    {
        // Desativa o collider original do proj�til para evitar m�ltiplas colis�es
        Collider2D originalCollider = GetComponents<Collider2D>()[0];
        if (originalCollider != null && originalCollider != explosionCollider)
        {
            originalCollider.enabled = false;
        }

        // Para o movimento do proj�til
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Aumenta o tamanho do sprite para representar a explos�o
        //Transform visualTransform = transform;
        //Vector3 originalScale = visualTransform.localScale;
        //visualTransform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, originalScale.z);

        // Muda a cor para um efeito visual de explos�o (opcional)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow; // Ou qualquer cor para representar explos�o
        }

        // Instancia um efeito visual se fornecido
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Aplica o dano em �rea uma �nica vez
        ApplyExplosionDamage();

        // Espera a dura��o da explos�o
        yield return new WaitForSeconds(explosionDuration);

        // Destroi o proj�til ap�s a explos�o
        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        // Encontra todos os inimigos na �rea de explos�o
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verifica se � um inimigo
            if (hitCollider.CompareTag("Enemy"))
            {
                int enemyID = hitCollider.gameObject.GetInstanceID();

                // Verifica se este inimigo j� recebeu dano direto (impacto inicial)
                if (damagedEnemies.Contains(enemyID))
                    continue;

                // Adiciona � lista de inimigos que j� receberam dano
                damagedEnemies.Add(enemyID);

                // Calcula o dano da explos�o (metade do dano normal)
                float explosionDamage = bulletDamage * explosionDamageMultiplier;

                // Aplica o dano
                EntityStats enemyStats = hitCollider.GetComponent<EntityStats>();
                if (enemyStats != null)
                {
                    enemyStats.RemoveHp(explosionDamage);
                }
            }
        }
    }

    // M�todo para visualizar o raio da explos�o no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}