using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/* Author:
 * Sebastián Jiménez Fernández
 * Class GameController.
 */

public class GameController : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject player1;
    public GameObject player2;
    public GameObject menuPausa;

    [Header("Tiles")]
    [SerializeField] private Tile tileFlower;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap tilemapDecoBack;

    [Header("Booleans")]
    public bool isPaused;
    public bool victoryAchieved;
    public bool isDead;

    private Vector2 screenBounds;
    private Camera mainCam;
    private Player1 player1Script;
    private float objectWidth;
    private float objectHeight;
    private bool panelControlesVisible;
    private bool panelCreditosVisible;
    private bool gameReady;
    private AudioManager audioPlayer1;

    [Header("Menú y controles")]
    [SerializeField] private GameObject panelControles;
    [SerializeField] private GameObject panelCreditos;
    [SerializeField] private GameObject panelVictory;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private Image imagenFadeOut;
    [SerializeField] private Image win;
    [SerializeField] private Animator winAnim;

    public AnimationCurve m_smoothCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 1f) });
    private float m_timerCurrent;
    private readonly WaitForSecondsRealtime m_skipFrame = new WaitForSecondsRealtime(0.01f);

    /*singelton pattern: con esto podemos acceder al GameController desde cualquier otra clase*/
    private static GameController _gameControllerInstance;
    public static GameController gameControllerInstance { get { return _gameControllerInstance; } }


    bool stop = false;

    private void Awake()
    {
        if (_gameControllerInstance != null && _gameControllerInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameControllerInstance = this;
        }
        mainCam = Camera.main;
        player1Script = player1.GetComponent<Player1>();
        audioPlayer1 = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        //Bounds de la cámara.
        screenBounds = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Recoge los bounds de los jugadores para que no salgan de cámara.
        objectWidth = player1.transform.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        objectHeight = player1.transform.GetComponentInChildren<SpriteRenderer>().bounds.size.y / 2;

        objectWidth = player2.transform.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        objectHeight = player2.transform.GetComponentInChildren<SpriteRenderer>().bounds.size.y / 2;

        StartCoroutine(AnimarFadeOut(1f, 0f, 1f));
    }

    private void Update()
    {
        RoadToVictory();

        if (Input.GetKeyDown(KeyCode.Escape) && !victoryAchieved)
        {
            PausarJuego();
        }
        if (isDead)
        {
            //animacion panel de muerte
            Debug.Log("muerto");
            deathMenu.SetActive(true);
            LeanTween.scale(deathMenu, Vector3.one, 1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
            isDead = false;
        }
        if (!stop)
            Victory();
    }

    private void LateUpdate()
    {
        BoundsPlayer1();
        BoundsPlayer2();

    }

    #region Bounds de jugadores.

    ///Bounds del jugador 1.
    //Para que el jugador 1 no salga de cámara.
    private void BoundsPlayer1()
    {
        Vector3 viewPos = player1.transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);
        player1.transform.position = viewPos;
    }

    ///Bounds del jugador 2.
    //Para que el jugador 2 no salga de cámara.
    private void BoundsPlayer2()
    {
        Vector3 viewPos = player2.transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + objectWidth, screenBounds.x - objectWidth);
        viewPos.y = Mathf.Clamp(viewPos.y, screenBounds.y * -1 + objectHeight, screenBounds.y - objectHeight);
        player2.transform.position = viewPos;
    }
    #endregion

    #region GameManager

    //Comprobará si todos los Tiles son de flores para decidir la victoria.
    private void RoadToVictory()
    {
        //Recoge variable cada vez que el jugador 1 planta
        //Para comprobar y no realizar un For cada frame del Update.
        if (player1Script.canWin == true)
        {
            int victoryCount = 0;
            TileBase[] tileArray = tilemap.GetTilesBlock(tilemap.cellBounds);
            foreach (TileBase tile in tileArray)
            {
                if (tile != null && tile.Equals(tileFlower))
                {
                    victoryCount++;
                }
            }
            if (tileArray.Length == victoryCount)
            {
                victoryAchieved = true;
                Victory();
                player1Script.canWin = false;
            }
            else { player1Script.canWin = false; }
        }
    }

    //Pusará el juego y saldrá la pantalla de victoria.
    private void Victory()
    {
        if (victoryAchieved)
        {
            stop = true;
            tilemapDecoBack.gameObject.SetActive(false);
            Debug.Log("GANASTE LOCO");
            Time.timeScale = 0F;

            //Busca a todos los enemigos, los desactiva primero, y después los destruye.
            //Esto se hace para que no tenga tanta carga el programa.
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(false);
                Destroy(enemy, 0.5F);
            }
            StartCoroutine(VictoryIMG());
        }
    }

    #region Menú y controles.

    //Pausa el juego y activa el menú.
    public void PausarJuego()
    {
        if (ControladorInicioJuego.Instance.start)
        {
            isPaused = !isPaused;
            menuPausa.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
            if (!isPaused)
            {
                //TODO: Controlar animacion ui
            }
        }
    }

    IEnumerator VictoryIMG()
    {
        win.gameObject.SetActive(true);
        winAnim.Play("Appear");
        yield return new WaitForSecondsRealtime(3F);
        panelVictory.SetActive(true);
    }

    public void CargarJuego()
    {
        SceneManager.LoadScene("Juego");
    }
    public void CargarControles()
    {
        LeanTween.moveLocalX(panelControles, panelControlesVisible ? -2000 : -250, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(pauseMenu, panelControlesVisible ? 0 : 500, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        panelControlesVisible = !panelControlesVisible;
    }
    public void CargarControlesInverso()
    {
        LeanTween.moveLocalX(victoryMenu, 0, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(panelControles, -2000, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(pauseMenu, panelControlesVisible ? 0 : -500, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        panelControlesVisible = false;
    }

    public void CargarCreditos()
    {
        LeanTween.moveLocalX(panelCreditos, panelCreditosVisible ? -2000 : 0, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(victoryMenu, panelCreditosVisible ? 0 : 2000, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        panelCreditosVisible = !panelCreditosVisible;
    }
    public void CargarCreditosInverso()
    {
        LeanTween.moveLocalX(pauseMenu, 0, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(panelCreditos, -2000, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.moveLocalX(victoryMenu, panelCreditosVisible ? 0 : -2000, 0.1f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        panelCreditosVisible = false;
    }

    IEnumerator AnimarFadeOut(float start, float end, float m_fadeDuration)
    {
        imagenFadeOut.gameObject.SetActive(true);
        m_timerCurrent = 0f;

        while (m_timerCurrent <= m_fadeDuration)
        {
            m_timerCurrent += Time.deltaTime;
            Color c = imagenFadeOut.color;
            imagenFadeOut.color = new Color(c.r, c.g, c.b, Mathf.Lerp(start, end, m_smoothCurve.Evaluate(m_timerCurrent / m_fadeDuration)));
            yield return m_skipFrame;
        }
        if (m_timerCurrent >= m_fadeDuration)
        {
            imagenFadeOut.gameObject.SetActive(false);
        }

    }
    IEnumerator CargarMenuFadeIn()
    {
        StartCoroutine(AnimarFadeOut(0f, 1f, 0.8f));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }
    public void SalirJuego()
    {
        Time.timeScale = 1;
        audioPlayer1.Stop("GamePlay");
        audioPlayer1.Stop("GamePlayIntro");
        SceneManager.LoadScene("Menu");
        //StartCoroutine(CargarMenuFadeIn());
        //Application.Quit();
    }
    #endregion
#endregion
}
