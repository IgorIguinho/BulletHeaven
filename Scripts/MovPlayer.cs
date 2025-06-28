using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    EntityStats entityStats;
    Animator animator;
    private void Start()
    {
        entityStats = gameObject.GetComponent<EntityStats>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Moviment(entityStats.moveSpeed_ + entityStats.bonusAtackSpeed);

    }

    public void Moviment(float movespeed_)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Debug.Log($"Horizontal: {horizontal}, Vertical: {vertical}");

        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontal * movespeed_ * Time.deltaTime, vertical * movespeed_ * Time.deltaTime));

        //////Movimento diagonal
        if ((horizontal != 0) && (vertical != 0))
        {
            movespeed_ = entityStats.moveSpeed_ + entityStats.bonusAtackSpeed * 0.66f;
        }
        else
        {
            movespeed_ = entityStats.moveSpeed_ + entityStats.bonusAtackSpeed;
        }

        if (horizontal != 0 || vertical != 0)
        {
            // Movimento diagonal
            if (vertical > 0 && horizontal > 0)
            {
                animator.Play("WalkCimaDireita");
            }
            else if (vertical > 0 && horizontal < 0)
            {
                animator.Play("WalkCimaEsquerda");
            }
            else if (vertical < 0 && horizontal > 0)
            {
                animator.Play("WalkBaixoDireita");
            }
            else if (vertical < 0 && horizontal < 0)
            {
                animator.Play("WalkBaixoEsquerda");
            }
            // Movimento ortogonal (mantendo seu código original)
            else if (vertical > 0)
            {
                animator.Play("IdleCima");
               
            }
            else if (vertical < 0)
            {
                animator.Play("WalkBaixo");
            }
            else if (horizontal > 0)
            {
                animator.Play("WalkDireita");
                
            }
            else if (horizontal < 0)
            {
                animator.Play("IdleEsquerda");
          
            }
        }
        else
        {
            animator.Play("Idle");
        }
    }
}