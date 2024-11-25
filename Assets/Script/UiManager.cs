using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class UiManager : Singleton<UiManager>
{
    int number;

    public void LightVib()
    {
        HapticFeedback.LightFeedback();
    }

    public void MediumVib()
    {
        HapticFeedback.MediumFeedback();
    }

    public void HeavyVib()
    {
        HapticFeedback.HeavyFeedback();
    }

    public void LoadSceneGamePlay()
    {
        SceneManager.LoadScene("GamePlay");
    } 

    public void LoadSceneHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void OnButtonClick()
    {
        number = PlayerPrefs.GetInt("CompletedLevel");
        int nextlv = number;
        PlayerPrefs.SetInt("SelectedLevel", nextlv);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GamePlay");
        AudioManager.Instance.AudioButtonClick();
    }
}
