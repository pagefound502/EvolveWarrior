using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject mainMenuPanel;  // Ana Menü Paneli (Inspector'dan bağla)
    public GameObject loadingScreen;  // Yükleme ekranı (Inspector'dan bağla)
    public Slider progressBar;        // Yükleme ilerleme çubuğu (Slider, Inspector'dan bağla)

    public void LoadGameScene()
    {
        // MainMenuPanel'i kapat
        mainMenuPanel.SetActive(false);

        // Yükleme ekranını aç
        loadingScreen.SetActive(true);

        // Asenkron sahne yüklemeye başla
        StartCoroutine(LoadSceneAsync("GameScene")); // Oyun sahnesinin ismi
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // Asenkron sahne yükleme başlat
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // %100 olmadan sahneye geçişi engelle

        while (!operation.isDone)
        {
            // Yükleme ilerlemesini %0 - %1 arasında döndür
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress; // Slider ilerleme çubuğunu güncelle

            // Eğer yükleme tamamlandıysa, sahneye geçiş yap
            if (operation.progress >= 0.9f)
            {
                progressBar.value = 1f; // Slider'ı %100'e getir

                // İsteğe bağlı olarak biraz bekleme süresi ekle
                yield return new WaitForSeconds(1f); // Bu süreyi ihtiyaca göre ayarla

                operation.allowSceneActivation = true; // Sahneye geçişi sağla
            }

            yield return null;
        }
    }
}
