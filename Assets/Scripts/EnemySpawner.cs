using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Author:
 * Sebastián Jiménez Fernández
 * Class EnemySpawner.
 */

public class EnemySpawner : MonoBehaviour
{
    private Camera mainCam;
    public GameObject enemy;

    private float heightCam;
    private float widthCam;

    private int randomSpawnZone;
    private float randomXposition;
    private float randomYposition;

    [SerializeField] private float spawnTime;

    private void Awake()
    {
        //Si no tiene tiempo se inicializa a un valor base.
        if(spawnTime.Equals(null) || spawnTime <= 1F) { spawnTime = 5F; }
    }

    void Start()
    {
        mainCam = Camera.main;
        heightCam = mainCam.orthographicSize + 1;
        widthCam = mainCam.orthographicSize * mainCam.aspect + 1;

        InvokeRepeating("SpawnEnemies", 0F, spawnTime);
    }

    //De manera aleatoria se recogen posiciones para 
    //spawnear enemigos fuera de la cámara.
    private void SpawnEnemies()
    {
        randomSpawnZone = Random.Range(0, 4);

        switch (randomSpawnZone)
        {
            case 0:
                randomXposition = Random.Range(-widthCam,widthCam);
                randomYposition = heightCam + 0.5F;
                break;
            case 1:
                randomXposition = Random.Range(-widthCam, widthCam);
                randomYposition = -heightCam - 0.5F;
                break;
            case 2:
                randomXposition = widthCam + 0.5F;
                randomYposition = Random.Range(-heightCam, heightCam);
                break;
            case 3:
                randomXposition = -widthCam - 0.5F;
                randomYposition = Random.Range(-heightCam, heightCam);
                break;
        }
        Instantiate(enemy, new Vector3(randomXposition, randomYposition, 0), Quaternion.identity);
    }
}
