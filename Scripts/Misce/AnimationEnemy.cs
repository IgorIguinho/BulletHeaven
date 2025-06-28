using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [Header("Refer�ncias")]
    private Animator animator;
    private BoxCollider2D boxCollider;

    [Header("Configura��es do Collider")]
    [Tooltip("Tamanho do collider para movimentos horizontais (X, Y)")]
    public Vector2 horizontalColliderSize = new Vector2(1.5f, 1f);

    [Tooltip("Tamanho do collider para movimentos verticais (X, Y)")]
    public Vector2 verticalColliderSize = new Vector2(1f, 1.5f);

    // Dire��o atual e anterior do movimento
    private Vector2 moveDirection;
    private Vector2 previousPosition;

    // Nome das anima��es (voc� pode alter�-los conforme necess�rio)
    [Header("Nomes das Anima��es")]
    public string animIdle = "Idle";
    public string animWalkBaixo = "WalkBaixo";
    public string animWalkCima = "WalkCima";
    public string animWalkEsquerda = "WalkEsquerda";
    public string animWalkDireita = "WalkDireita";

    // Anima��o atual
    private string currentAnimation;

    void Start()
    {
        // Obtendo componentes
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Verifica��es de seguran�a
        if (animator == null)
            Debug.LogError("Animator n�o encontrado em " + gameObject.name);

        if (boxCollider == null)
            Debug.LogError("BoxCollider2D n�o encontrado em " + gameObject.name);

        // Inicializa posi��o anterior
        previousPosition = transform.position;

        // Inicia com anima��o padr�o
        ChangeAnimation(animWalkBaixo);
    }

    void Update()
    {
        // Calcula a dire��o do movimento baseado na mudan�a de posi��o
        Vector2 currentPosition = transform.position;
        moveDirection = currentPosition - previousPosition;

        // S� atualiza se houver movimento significativo
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            // Determina qual anima��o deve ser reproduzida
            UpdateAnimation();

            // Ajusta o tamanho do collider
            UpdateColliderSize();
        }

        // Atualiza a posi��o anterior para o pr�ximo frame
        previousPosition = currentPosition;
    }

    void UpdateAnimation()
    {
        // Determina a dire��o predominante
        float absX = Mathf.Abs(moveDirection.x);
        float absY = Mathf.Abs(moveDirection.y);

        // Escolhe a anima��o baseada na dire��o predominante
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

        // Troca de anima��o apenas se for diferente da atual
        if (newAnimation != currentAnimation)
        {
            ChangeAnimation(newAnimation);
        }
    }

    void ChangeAnimation(string animationName)
    {
        // Guarda o nome da anima��o atual
        currentAnimation = animationName;

        // Inicia a anima��o
        animator.Play(animationName);
    }

    void UpdateColliderSize()
    {
        if (boxCollider == null) return;

        // Verificar dire��o predominante
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