using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManagerCustom : MonoBehaviour
{

    public GameObject playerPrefab;
    public static PlayerController playerController;
    public Transform playerSpawnPosition;

    public  Image healthImage;
    public Image expImage;

    public EnemySpawnManager enemySpawnManager;

    public static bool playerIsDeath=false;
    public static bool isPaused = false;
    static GameObject playerObject;

    public int startDelay = 3;
    int currentDelay;


    void Awake()
    {
        Debug.Log("GameManagerStarted");
        currentDelay = startDelay;
        Debug.Log("PlayerSpawned");
        playerObject = Instantiate(playerPrefab, playerSpawnPosition);
        playerObject.GetComponent<PlayerController>().healthImage = healthImage;
        playerObject.GetComponent<PlayerController>().expImage = healthImage;
        playerObject.GetComponent<PlayerController>().SpawnPlayer();
        //
        playerController = playerObject.GetComponent<PlayerController>();




        StartCoroutine("GameStart");
    }
    public static GameObject GetPlayer()
    {
        return playerObject;
    }
    public static PlayerController GetPlayerController()
    {
        return playerObject.GetComponent<PlayerController>();
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1);
        currentDelay -= 1;
        Debug.Log(currentDelay);
        if (currentDelay <= 0)
        {
            currentDelay = 0;
            enemySpawnManager.SpawnStart();
            StopCoroutine("GameStart");
        }
        else
        {
            StartCoroutine("GameStart");
        }
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            playerObject.GetComponent<PlayerController>().SpawnPlayer();
            return;
        }
    }

}
