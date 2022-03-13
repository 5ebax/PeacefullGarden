using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorInicioJuego : MonoBehaviour
{
    private AudioManager audioStart;

    public bool player1Ready;
    public bool player2Ready;
    public bool gameReady;

    public GameObject canvas;
    public GameObject panelPlayer1;
    public GameObject imagenPlayer1;
    public GameObject panelPlayer2;
    public GameObject imagenPlayer2;

    public GameObject textoPlayer1;
    public GameObject textoPlayer2;

    public GameObject textoNum1;
    public GameObject textoNum2;
    public GameObject textoNum3;

    private void Awake()
    {
        audioStart = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        gameReady = false;
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player1Ready = true;
            panelPlayer1.GetComponent<Image>().color = Color.white;
            imagenPlayer1.GetComponent<Image>().color = Color.white;
            textoPlayer1.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            player2Ready = true;
            panelPlayer2.GetComponent<Image>().color = Color.white;
            imagenPlayer2.GetComponent<Image>().color = Color.white;
            textoPlayer2.SetActive(false);
        }
        if (player1Ready && player2Ready && !gameReady)
        {
            StartCoroutine(StartGame());
        }
    }
    IEnumerator StartGame()
    {
        gameReady = true;
        AudioStartGame();

        textoNum3.SetActive(true);
        LeanTween.scale(textoNum3, Vector3.one * 1.5f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true); 
        yield return new WaitForSecondsRealtime(1f);

        textoNum3.SetActive(false);
        textoNum2.SetActive(true);
        LeanTween.scale(textoNum2, Vector3.one * 1.5f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true); 
        yield return new WaitForSecondsRealtime(1f);

        textoNum2.SetActive(false);
        textoNum1.SetActive(true);
        LeanTween.scale(textoNum1, Vector3.one * 1.5f, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true); 
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }

    private void AudioStartGame()
    {
        audioStart.Play("GamePlayIntro");
        audioStart.PlayDelayed("GamePlay", 19.304F);
    }

}
