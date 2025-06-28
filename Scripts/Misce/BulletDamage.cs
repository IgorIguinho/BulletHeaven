using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [Header("Configurações Básicas")]
    public float bulletDamage;
    public float bulletLife;
    public bool isPlayer;

    [Header("Configurações de Explosão")]
    public bool explosionBuffActive = false;
    public float explosionRadius = 3f;
    public float explosionDamageMultiplier = 0.5f;
    public float explosionDuration = 0.3f;

    [Header("Referências")]
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

        // Adiciona um collider circular para a explosão (desativado inicialmente)
        explosionCollider = gameObject.AddComponent<CircleCollider2D>();
        explosionCollider.radius = explosionRadius;
        explosionCollider.isTrigger = true;
        explosionCollider.enabled = false;

        // Configura a destruição automática após o tempo de vida
        Destroy(gameObject, bulletLife);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evita processar colisões múltiplas ou colisões após a explosão já ter iniciado
        if (hasExploded || collision.GetInstanceID() == explosionCollider.GetInstanceID())
            return;

        // Lógica para projétil do jogador atingindo inimigo
        if (collision.gameObject.CompareTag("Enemy") && isPlayer)
        {
            // Marca o inimigo como já tendo recebido dano direto
            int enemyID = collision.gameObject.GetInstanceID();
            damagedEnemies.Add(enemyID);

            // Causa dano ao inimigo atingido diretamente
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(bulletDamage);

            // Se o buff de explosão estiver ativo, executa a explosão
            if (explosionBuffActive)
            {
                hasExploded = true;
                StartCoroutine(ExplodeCoroutine());
            }
            else
            {
                // Se não for explodir, destrói o projétil normalmente
                Destroy(gameObject);
            }
        }
        // Lógica para projétil de inimigo atingindo o jogador
        else if (collision.gameObject.CompareTag("Player") && !isPlayer)
        {
            collision.gameObject.GetComponent<EntityStats>().RemoveHp(bulletDamage);
            Destroy(gameObject);
        }
        // Colisão com paredes
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Se o buff de explosão estiver ativo, executa a explosão mesmo em paredes
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
        // Desativa o collider original do projétil para evitar múltiplas colisões
        Collider2D originalCollider = GetComponents<Collider2D>()[0];
        if (originalCollider != null && originalCollider != explosionCollider)
        {
            originalCollider.enabled = false;
        }

        // Para o movimento do projétil
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Aumenta o tamanho do sprite para representar a explosão
        //Transform visualTransform = transform;
        //Vector3 originalScale = visualTransform.localScale;
        //visualTransform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, originalScale.z);

        // Muda a cor para um efeito visual de explosão (opcional)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow; // Ou qualquer cor para representar explosão
        }

        // Instancia um efeito visual se fornecido
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Aplica o dano em área uma única vez
        ApplyExplosionDamage();

        // Espera a duração da explosão
        yield return new WaitForSeconds(explosionDuration);

        // Destroi o projétil após a explosão
        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        // Encontra todos os inimigos na área de explosão
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            // Verifica se é um inimigo
            if (hitCollider.CompareTag("Enemy"))
            {
                int enemyID = hitCollider.gameObject.GetInstanceID();

                // Verifica se este inimigo já recebeu dano direto (impacto inicial)
                if (damagedEnemies.Contains(enemyID))
                    continue;

                // Adiciona à lista de inimigos que já receberam dano
                damagedEnemies.Add(enemyID);

                // Calcula o dano da explosão (metade do dano normal)
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

    // Método para visualizar o raio da explosão no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}