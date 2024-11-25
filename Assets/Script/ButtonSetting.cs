using UnityEngine;
using UnityEngine.UI;

public class ButtonSetting : MonoBehaviour
{
    public GameObject imgOn; 
    public string key;
    private bool isActive = true; 

    private void Start()
    {
        LoadState();
        AudioManager.Instance.SetActive(isActive);
    }
    
    public void Toggle()
    {
        isActive = !isActive;
        imgOn.SetActive(isActive);

        SaveState();
        AudioManager.Instance.SetActive(isActive);
    }

    private void SaveState()
    {
        int numberSave = isActive ? 1 : 0;
        PlayerPrefs.SetInt(key, numberSave);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        int numberSavedState = PlayerPrefs.GetInt(key, 1); 
        isActive = numberSavedState == 1;
        imgOn.SetActive(isActive);
        AudioManager.Instance.SetActive(isActive);
    }
}
