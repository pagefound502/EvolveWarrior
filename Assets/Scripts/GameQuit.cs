using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQuit : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Oyun kapatıldı!"); // Editor'de test için
        Application.Quit(); // Oyunu kapat
    }
}
