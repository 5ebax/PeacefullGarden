using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using UnityEngine.UI;

public class ControladorMenu : MonoBehaviour
{
    [SerializeField] private GameObject botonesMenu;
    [SerializeField] private GameObject botonSalir;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private float botonesMenuPosicionFinalX;
    [SerializeField] private GameObject botonSaltarCinematica;
    [SerializeField] private GameObject panelControles;
    [SerializeField] private GameObject panelCreditos;
    bool panelControlesVisible;
    bool panelCreditosVisible;

    private AudioManager audioMenu;
    
    [SerializeField] private VideoPlayer controladorVideoIntro;
    [SerializeField] private VideoPlayer controladorVideoMenu;
    [SerializeField] private string videoFileName;
    [SerializeField] private string videoFileMenuName;

    [SerializeField] private Image imagenFadeOut;
    public AnimationCurve m_smoothCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
    private float m_timerCurrent;
    private readonly WaitForSeconds m_skipFrame = new WaitForSeconds(0.01f);

    public LeanTweenType tipoAnimacion;

    private void Awake()
    {
#if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#elif (UNITY_WEBGL)
    botonSalir.SetActive(false);
#endif
        audioMenu = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))
            SaltarVideoIntro();
    }

    private void Start()
    {
        controladorVideoIntro.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        controladorVideoMenu.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileMenuName);

        botonSaltarCinematica.transform.localScale = Vector3.zero;
        StartCoroutine(AnimarFadeOut(1f,0f,1f));
        Invoke("SaltarVideoIntro",39f);
        LeanTween.scale(botonSaltarCinematica, Vector3.one * 1.1f, 2f).setEase(LeanTweenType.easeInExpo).setDelay(2f);
    }
    void ActivarBotones()
    {
        botonesMenu.SetActive(true);
        titleScreen.SetActive(true);
        audioMenu.Play("MenuIntro");
        audioMenu.PlayDelayed("MenuLoop",06.478F);
        botonSaltarCinematica.SetActive(false);
    }

    public void SaltarVideoIntro()
    {
        controladorVideoIntro.gameObject.SetActive(false);
        StartCoroutine(AnimarFadeOut(1f,0f,0.25f));
        ActivarBotones();
    }
    IEnumerator AnimarFadeOut(float start, float end, float m_fadeDuration)
    {
        imagenFadeOut.gameObject.SetActive(true);
        m_timerCurrent = 0f;

        while (m_timerCurrent <= m_fadeDuration)
        {
            m_timerCurrent += Time.fixedDeltaTime;
            Color c = imagenFadeOut.color;
            imagenFadeOut.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, end, m_smoothCurve.Evaluate(m_timerCurrent / m_fadeDuration)));
            yield return m_skipFrame;
        }
        if (m_timerCurrent >= m_fadeDuration)
        {
            imagenFadeOut.gameObject.SetActive(false);
        }

    }

    public void CargarControles()
    {
        LeanTween.moveLocalX(panelControles, panelControlesVisible ? -2000 : -50, 0.1f).setEase(tipoAnimacion);
        LeanTween.moveLocalX(botonesMenu, panelControlesVisible ? 0 : 500, 0.1f).setEase(tipoAnimacion);
        panelControlesVisible = !panelControlesVisible;
    }
    public void CargarControlesInverso()
    {
        LeanTween.moveLocalX(botonesMenu, 0, 0.1f).setEase(tipoAnimacion);
        LeanTween.moveLocalX(panelControles, -2000, 0.1f).setEase(tipoAnimacion);
        panelControlesVisible = false;
    }

    public void CargarCreditos()
    {
        LeanTween.moveLocalX(panelCreditos, panelCreditosVisible ? -2000 : -50, 0.1f).setEase(tipoAnimacion);
        LeanTween.moveLocalX(botonesMenu, panelCreditosVisible ? 0 : 2000, 0.1f).setEase(tipoAnimacion);
        panelCreditosVisible = !panelCreditosVisible;
    }
    public void CargarCreditosInverso()
    {
        LeanTween.moveLocalX(botonesMenu, 0, 0.1f).setEase(tipoAnimacion);
        LeanTween.moveLocalX(panelCreditos, -2000, 0.1f).setEase(tipoAnimacion);
        panelCreditosVisible = false;
    }

    public void CargarJuego()
    {
        StartCoroutine(CargarJuegoFadeIn());
    }
    IEnumerator CargarJuegoFadeIn()
    {
        panelControles.SetActive(false);
        StartCoroutine(AnimarFadeOut(0f,1f,1.2f));
        yield return new WaitForSeconds(0.6f);
        audioMenu.Stop("MenuIntro");
        audioMenu.Stop("MenuLoop");
        SceneManager.LoadScene("Juego");
    }
    IEnumerator CargarMenuFadeIn()
    {
        StartCoroutine(AnimarFadeOut(0f,1f,1f));
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene("Menu");
    }
    public void SalirJuego()
    {
        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
#endif
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
    Application.Quit();
#elif (UNITY_WEBGL)
    botonSalir.SetActive(false);
#endif
    }
}
