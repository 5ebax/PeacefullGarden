using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author:
 * Sebastián Jiménez Fernández
 * Class Sword.
 */

public class Sword : MonoBehaviour
{
    [SerializeField] private Animator animPlayer2;

    //La "Espada"(Collider) empiece desactivado.
    private void Start()
    {
        gameObject.SetActive(false);
    }
    //Vuelve a desactivar la "Espada".
    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    //Reproduce el sonido de la espada.
    private void PlaySound()
    {
        FindObjectOfType<AudioManager>().Play("Sword");
    }

    //Activa la animación de la espada con un evento que reactiva y desactiva el collider.
    public void Swing()
    {
        PlaySound();
        animPlayer2.Play("Player2_SwordDown");
    }
    public void Swing_Left()
    {
        PlaySound();
        animPlayer2.Play("Player2_SwordLeft");
    }
    public void Swing_Right()
    {
        PlaySound();
        animPlayer2.Play("Player2_SwordRight");
    }
    public void Swing_Up()
    {
        PlaySound();
        animPlayer2.Play("Player2_SwordUp");
    }
}
