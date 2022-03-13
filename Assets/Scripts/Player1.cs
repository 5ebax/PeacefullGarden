using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* Author:
 * Sebastián Jiménez Fernández
 * Class Player1.
 */

public class Player1 : MonoBehaviour
{

    private Rigidbody2D rbPlayer1;
    private Vector2 movPlayer1;
    private BoundsInt area;
    private AudioManager audioPlayer1;
    private int numWater;
    private int numFlowers;
    private bool inside;
    private bool plant;

    [Header("Numbers")]
    public int numMaxFlowers;
    public int numMaxWater;
    public float timePlant;

    //Serialized Fields.
    [Header("Serializable Fields")]
    [SerializeField] private Tile tileFlower;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Animator animPlayer;
    [SerializeField] private float movementSpeed;
    [HideInInspector] public bool canWin;

    public GameObject[] listaFlores;
    public GameObject[] listaRegaderas;

    private void Awake()
    {
        animPlayer.updateMode = AnimatorUpdateMode.Normal;

        //Inicializa si no se ha escogido la cantidad.
        if (numMaxFlowers.Equals(null) || numMaxFlowers < 1) { numMaxFlowers = 5; }
        if (numMaxWater.Equals(null) || numMaxWater < 1) { numMaxWater = 5; }
        if (timePlant.Equals(null) || timePlant < 1) { timePlant = 3; }

        rbPlayer1 = GetComponent<Rigidbody2D>();
        audioPlayer1 = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        inside = false;
        canWin = false;
        plant = false;
        numWater = numMaxWater;
        numFlowers = numMaxFlowers;
    }

    void Update()
    {
        EntryMovementPlayer1();
        Actions();
        AnimarUI();
    }

    private void FixedUpdate()
    {
        MovementPlayer1();
    }

    #region Movimientos.

    /// Movimiento del jugador 1.
    private void EntryMovementPlayer1()
    {
        //Recoge el movimiento del jugador 1.
        movPlayer1.x = Input.GetAxisRaw("P1_Horizontal");
        movPlayer1.y = Input.GetAxisRaw("P1_Vertical");
    }

    //Posiciona la animación del jugador 1 según su movimiento.
    private void SetMovAnimationsPlayer1()
    {
        //Ajusta los parámetros para la animación del jugador 1.
        animPlayer.SetFloat("Horizontal", movPlayer1.x);
        animPlayer.SetFloat("Vertical", movPlayer1.y);
        animPlayer.SetFloat("Speed", movPlayer1.sqrMagnitude);

        //Recoge el último Axis del jugador 1 para colocar el Idle correcto.
        if (Input.GetAxisRaw("P1_Vertical") == 1 || Input.GetAxisRaw("P1_Vertical") == -1 || Input.GetAxisRaw("P1_Horizontal") == 1 || Input.GetAxisRaw("P1_Horizontal") == -1)
        {
            animPlayer.SetFloat("LastMoveX", Input.GetAxisRaw("P1_Horizontal"));
            animPlayer.SetFloat("LastMoveY", Input.GetAxisRaw("P1_Vertical"));
        }
    }

    private void MovementPlayer1()
    {
        if (animPlayer.GetBool("Plant") == false)
        {
            SetMovAnimationsPlayer1();
            rbPlayer1.MovePosition(rbPlayer1.position + movPlayer1 * movementSpeed * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Acciones del jugador 1.
    //Acciones de botones.
    private void Actions()
    {
        //Al pulsar espacio recoge la posición del Tile y la cambia.
        if (Input.GetKeyDown(KeyCode.Space) && Time.timeScale != 0F & !plant)
        {
            Vector3Int currentCell = tilemap.WorldToCell(transform.position);
            if (tilemap.GetTile(currentCell) != tileFlower && numFlowers > 0 && !inside)
            {
                //Sonido de plantar.
                audioPlayer1.Play("Plant");
                StartCoroutine(PlayerPlant(currentCell));
            }
        }
    }

    //Jugador muere y se reproduce su animación.
    private void PlayerDead()
    {
        StopInDead();
        animPlayer.updateMode = AnimatorUpdateMode.UnscaledTime;
        animPlayer.Play("Player_Dead");
    }

    //Recoge el agua a su cantidad máxima.
    private void CollectWater()
    {
        numWater = numMaxWater;
    }

    //Recoge flores a su cantidad máxima.
    private void CollectFlowers()
    {
        numFlowers = numMaxFlowers;
    }

    #endregion

    #region Corrutinas
    //Para al jugador y cambia la animación y el Tile.
    IEnumerator PlayerPlant(Vector3Int currentCell)
    {
        plant = true;
        animPlayer.SetBool("Plant", plant);
        rbPlayer1.MovePosition(new Vector2(currentCell.x + 0.5F, currentCell.y + 0.5F));
        yield return new WaitForSeconds(timePlant);
        numFlowers -= 1;
        tilemap.SetTile(currentCell, tileFlower);
        canWin = true;
        plant = false;
        animPlayer.SetBool("Plant", plant);
    }
    //Para al jugador y "replanta" al enemigo cambiando las Tiles.
    IEnumerator PlayerRePlant(Vector3Int enemyCell)
    {
        plant = true;
        animPlayer.SetBool("Plant", plant);
        rbPlayer1.MovePosition(new Vector2(enemyCell.x + 0.5F, enemyCell.y + 0.5F));
        enemyCell.x -= 1;
        enemyCell.y -= 1;
        area.position = enemyCell;
        area.size = new Vector3Int(3, 3, 1);
        TileBase[] tileArray = tilemap.GetTilesBlock(area);
        for (var i = 0; i < tileArray.Length; i++)
        {
            if(tileArray[i] != null)
                tileArray[i] = tileFlower;
        }

        yield return new WaitForSeconds(timePlant);
        numWater -= 1;
        tilemap.SetTilesBlock(area, tileArray);
        canWin = true;
        plant=false;
        animPlayer.SetBool("Plant", plant);
    }
    #endregion

    #region Colisiones.

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Si está "Dentro" del enemigo y pulsa espacio, lo "replanta".
        if (collision.gameObject.CompareTag("Enemy"))
        {
            inside = true;
            Vector3Int enemyCell = tilemap.WorldToCell(collision.gameObject.transform.position);
            if (Input.GetKey(KeyCode.Space) && tilemap.GetTile(enemyCell) != tileFlower && numWater > 0)
            {
                animPlayer.Play("Player_Plant");
                StartCoroutine(PlayerRePlant(enemyCell));

                collision.gameObject.SetActive(false);
                Destroy(collision.transform.parent.gameObject, 0.5f);
            }
        }
        else { inside = false; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "ContainerWater":
                CollectWater();
                break;
            case "ContainerFlowers":
                CollectFlowers();
                break;
            case "Enemy":
                PlayerDead();
                break;
        }
    }
    #endregion

    private void StopInDead()
    {
        Time.timeScale = 0F;
        audioPlayer1.Stop("GamePlay");
        audioPlayer1.Stop("GamePlayIntro");

        GameController.gameControllerInstance.isDead = true;
    }

    //UI de las flores y el agua.
    public void AnimarUI()
    {
        foreach (var item in listaFlores)
        {
            item.SetActive(false);

        }
        switch (numFlowers)
        {
            case 0:
                for (int i = 0; i < 4; i++)
                {
                    listaFlores[i].SetActive(false);
                }
                break;
            case 1:
                for (int i = 0; i < 1; i++)
                {
                    listaFlores[i].SetActive(true);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    listaFlores[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    listaFlores[i].SetActive(true);
                }
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                {
                    listaFlores[i].SetActive(true);
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    listaFlores[i].SetActive(true);
                }
                break;
        }
        foreach (var item in listaRegaderas)
        {
            item.SetActive(false);

        }
        switch (numWater)
        {
            case 0:
                for (int i = 0; i < 4; i++)
                {
                    listaRegaderas[i].SetActive(false);
                }
                break;
            case 1:
                for (int i = 0; i < 1; i++)
                {
                    listaRegaderas[i].SetActive(true);
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    listaRegaderas[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    listaRegaderas[i].SetActive(true);
                }
                break;
            case 4:
                for (int i = 0; i < 4; i++)
                {
                    listaRegaderas[i].SetActive(true);
                }
                break;
            case 5:
                for (int i = 0; i < 5; i++)
                {
                    listaRegaderas[i].SetActive(true);
                }
                break;
        }
    }
}
