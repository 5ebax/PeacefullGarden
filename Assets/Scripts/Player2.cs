using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author:
 * Sebastián Jiménez Fernández
 * Class Player2.
 */

public class Player2 : MonoBehaviour
{

    private Vector2 movPlayer2;
    private Rigidbody2D rbPlayer2;
    [SerializeField] private Animator animPlayer;
    [SerializeField] private Animator animSword;
    [SerializeField] private GameObject sword;
    [SerializeField] private float movementSpeed;

    private void Awake()
    {
        rbPlayer2 = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        EntryMovementPlayer2();
        Actions();
    }

    private void FixedUpdate()
    {
        MovementPlayer2();
    }

    #region Movimientos.

    /// Movimiento del jugador 2.
    private void EntryMovementPlayer2()
    {
        //Recoge el movimiento del jugador 2.
        movPlayer2.x = Input.GetAxisRaw("P2_Horizontal");
        movPlayer2.y = Input.GetAxisRaw("P2_Vertical");
    }

    //Posiciona la animación del jugador 2 según su movimiento.
    private void SetMovAnimationsPlayer2()
    {
        //Ajusta los parámetros para la animación del jugador 2.
        animPlayer.SetFloat("Horizontal", movPlayer2.x);
        animPlayer.SetFloat("Vertical", movPlayer2.y);
        animPlayer.SetFloat("Speed", movPlayer2.sqrMagnitude);

        //Recoge el último Axis del jugador 2 para colocar el Idle correcto.
        if (Input.GetAxisRaw("P2_Vertical") == 1 || Input.GetAxisRaw("P2_Vertical") == -1 || Input.GetAxisRaw("P2_Horizontal") == 1 || Input.GetAxisRaw("P2_Horizontal") == -1)
        {
            animPlayer.SetFloat("LastMoveX", Input.GetAxisRaw("P2_Horizontal"));
            animPlayer.SetFloat("LastMoveY", Input.GetAxisRaw("P2_Vertical"));
        }
    }

    private void MovementPlayer2()
    {
        SetMovAnimationsPlayer2();
        rbPlayer2.MovePosition(rbPlayer2.position + movPlayer2 * movementSpeed * Time.fixedDeltaTime);
    }
    #endregion

    #region Acciones del jugador 2.
    /// Acciones de botones.
    private void Actions()
    {
        //Golpea con la espada al pulsar Enter en la dirección a la que esté mirando.
        if (Input.GetKey(KeyCode.Return) && Time.timeScale != 0F)
        {
            sword.SetActive(true);
            if (animPlayer.GetFloat("LastMoveX") > 0) { animSword.Play("Swing_Right"); } else if (animPlayer.GetFloat("LastMoveX") < 0) { animSword.Play("Swing_Left"); }
            if (animPlayer.GetFloat("LastMoveY") > 0){ animSword.Play("Swing_Up"); } else if (animPlayer.GetFloat("LastMoveY") < 0) { animSword.Play("Swing"); }
            if (animPlayer.GetFloat("LastMoveX") == 0 && animPlayer.GetFloat("LastMoveY") == 0) { animSword.Play("Swing"); }
        }
    }
    #endregion
}
