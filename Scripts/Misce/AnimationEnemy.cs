using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [Header("Referências")]
    private Animator animator;
    private BoxCollider2D boxCollider;

    [Header("Configurações do Collider")]
    [Tooltip("Tamanho do collider para movimentos horizontais (X, Y)")]
    public Vector2 horizontalColliderSize = new Vector2(1.5f, 1f);

    [Tooltip("Tamanho do collider para movimentos verticais (X, Y)")]
    public Vector2 verticalColliderSize = new Vector2(1f, 1.5f);

    // Direção atual e anterior do movimento
    private Vector2 moveDirection;
    private Vector2 previousPosition;

    // Nome das animações (você pode alterá-los conforme necessário)
    [Header("Nomes das Animações")]
    public string animIdle = "Idle";
    public string animWalkBaixo = "WalkBaixo";
    public string animWalkCima = "WalkCima";
    public string animWalkEsquerda = "WalkEsquerda";
    public string animWalkDireita = "WalkDireita";

    // Animação atual
    private string currentAnimation;

    void Start()
    {
        // Obtendo componentes
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Verificações de segurança
        if (animator == null)
            Debug.LogError("Animator não encontrado em " + gameObject.name);

        if (boxCollider == null)
            Debug.LogError("BoxCollider2D não encontrado em " + gameObject.name);

        // Inicializa posição anterior
        previousPosition = transform.position;

        // Inicia com animação padrão
        ChangeAnimation(animWalkBaixo);
    }

    void Update()
    {
        // Calcula a direção do movimento baseado na mudança de posição
        Vector2 currentPosition = transform.position;
        moveDirection = currentPosition - previousPosition;

        // Só atualiza se houver movimento significativo
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            // Determina qual animação deve ser reproduzida
            UpdateAnimation();

            // Ajusta o tamanho do collider
            UpdateColliderSize();
        }

        // Atualiza a posição anterior para o próximo frame
        previousPosition = currentPosition;
    }

    void UpdateAnimation()
    {
        // Determina a direção predominante
        float absX = Mathf.Abs(moveDirection.x);
        float absY = Mathf.Abs(moveDirection.y);

        // Escolhe a animação baseada na direção predominante
        string newAnimation;

        if (absX > absY)
        {
            // Movimento horizontal predominante
            newAnimation = moveDirection.x > 0 ? animWalkDireita : animWalkEsquerda;
        }
        else
        {
            // Movimento vertical predominante
            newAnimation = moveDirection.y > 0 ? animWalkCima : animWalkBaixo;
        }

        // Troca de animação apenas se for diferente da atual
        if (newAnimation != currentAnimation)
        {
            ChangeAnimation(newAnimation);
        }
    }

    void ChangeAnimation(string animationName)
    {
        // Guarda o nome da animação atual
        currentAnimation = animationName;

        // Inicia a animação
        animator.Play(animationName);
    }

    void UpdateColliderSize()
    {
        if (boxCollider == null) return;

        // Verificar direção predominante
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            // Movimento predominantemente horizontal
            boxCollider.size = horizontalColliderSize;
        }
        else
        {
            // Movimento predominantemente vertical
            boxCollider.size = verticalColliderSize;
        }
    }
}