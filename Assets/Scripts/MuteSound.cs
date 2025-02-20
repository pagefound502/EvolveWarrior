using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteSound : MonoBehaviour
{
    public Toggle muteToggle;

    void Start()
    {
        // Daha önce kaydedilen sesi yükle
        bool isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        muteToggle.isOn = isMuted;
        AudioListener.volume = isMuted ? 0 : 1;

        // Toggle değiştiğinde fonksiyonu çağır
        muteToggle.onValueChanged.AddListener(SetMute);
    }

    public void SetMute(bool isMuted)
    {
        AudioListener.volume = isMuted ? 0 : 1;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }
}
