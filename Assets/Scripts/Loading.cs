using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public string oyunSahnesiAdi = "GameScene";
    public Slider yuklemeCubugu;
    public float beklemeSuresi = 10f; // Bekleme süresi (saniye)

    void Start()
    {
        StartCoroutine(OyunuYukle()); // Coroutine'i başlat
    }

    IEnumerator OyunuYukle()
    {
        yield return new WaitForSeconds(beklemeSuresi); // Bekleme süresi kadar bekle

        // Bekleme süresi bittikten sonra yükleme işlemine başla
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(oyunSahnesiAdi);

        if (yuklemeCubugu != null)
        {
            float hedefDeger = 0f; // İlereme çubuğunun ulaşması gereken hedef değer
            while (!asyncOperation.isDone)
            {
                float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                hedefDeger = Mathf.Lerp(hedefDeger, progress, Time.deltaTime * 5f); // Yumuşak geçiş için Lerp kullanıyoruz
                yuklemeCubugu.value = hedefDeger;
                yield return null;
            }
        }

        SceneManager.LoadScene(oyunSahnesiAdi); // Sahne geçişini yap
    }
}