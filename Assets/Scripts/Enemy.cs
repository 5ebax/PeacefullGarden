using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* Author:
 * Sebastián Jiménez Fernández
 * Class Enemy.
 */

public class Enemy : MonoBehaviour
{
    private NavMeshAgent enemy;
    private Rigidbody2D rbEnemy;
    private Animator animEnemy;
    private BoxCollider2D enemyCollision;
    private Transform player1Transform;
    private Transform player2Transform;

    private bool isDead;

    [SerializeField] private SpriteRenderer spriteEnemyBody;
    [SerializeField] private SpriteRenderer spriteEnemyHead;

    private void Awake()
    {
        enemy = GetComponent<NavMeshAgent>();
        rbEnemy = GetComponent<Rigidbody2D>();
        animEnemy = GetComponentInChildren<Animator>();
        enemyCollision = GetComponentInChildren<BoxCollider2D>();

        player1Transform = GameObject.FindGameObjectWithTag("Player").transform;
        player2Transform = GameObject.FindGameObjectWithTag("Player2").transform;
    }

    private void Start()
    {
        //El Sprite no haga locuras al moverse.
        enemy.updateRotation = false;
        enemy.updateUpAxis = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    #region Movimiento.
    //Movimiento del enemigo.
    private void Movement()
    {
        float diferencia = player1Transform.position.x - this.transform.position.x;
        if (enemy.isStopped == false)
        { 
            if (diferencia < 0) { spriteEnemyBody.flipX = true; } else { spriteEnemyBody.flipX = false; }
            if (diferencia < 0) { spriteEnemyHead.flipX = true; } else { spriteEnemyHead.flipX = false; }
            enemy.SetDestination(player1Transform.position);
        }
    }

    //Pausa y desactivación de la colisión.
    IEnumerator StopAndContinue()
    {
        isDead = true;
        enemy.isStopped = true;
        enemyCollision.isTrigger = true;
        animEnemy.SetBool("Hitted", true);
        Debug.Log("Hitted " + this.gameObject.name);
        yield return new WaitForSecondsRealtime(7F);
        
        animEnemy.SetBool("Hitted", false);
        enemy.isStopped = false;
        enemyCollision.isTrigger = false;
        isDead = false;
        yield break;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SwordCollider"))
        {
            if (!isDead)
            {
                StartCoroutine(StopAndContinue());
                Vector3 dir = transform.position*1.1F - player2Transform.position;
                rbEnemy.AddForce(dir, ForceMode2D.Impulse);
            }
            
        }
    }
}
