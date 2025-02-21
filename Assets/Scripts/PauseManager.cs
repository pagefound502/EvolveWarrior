using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseScreen; // Pause ekranını buraya atayacağız
    public GameObject pauseButton; // Ana ekrandaki Pause butonu

    public void PauseGame()
    {
        Time.timeScale = 0f; // OYUNU DURDUR
        pauseScreen.SetActive(true); // PauseScreen'i Aç
        pauseButton.SetActive(false); // Ana Pause Butonunu Gizle
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // OYUNU DEVAM ETTİR
        pauseScreen.SetActive(false); // PauseScreen'i Kapat
        pauseButton.SetActive(true); // Ana Pause Butonunu Tekrar Göster
    }
}
