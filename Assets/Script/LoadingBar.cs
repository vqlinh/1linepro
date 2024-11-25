using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public Slider slider;
     float time = 0f;
     float maxTime = 2f;
    private void Awake()
    {
        slider.maxValue = 1;
        slider.value = 0;
        Invoke("LoadScene",2f);
    }

    void Update()
    {
        if (time < maxTime)
        {
            time += Time.deltaTime; 
            float full = time / maxTime; 
            slider.value = full; 
        }
        else
        {
            slider.value = 1f; 
        }
    }
    public void LoadScene()
    {
        UiManager.Instance.LoadSceneHomeScene();
    }
}
