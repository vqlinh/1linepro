using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiGame : MonoBehaviour
{
     public GameObject panelWin;
     public  GameObject panelShop;
     public TextMeshProUGUI txtNumberHint;
     public TextMeshProUGUI lv;

    private void Start()
    {
        panelWin.SetActive(false);
        panelShop.SetActive(false);
    }
    public void ShowPanelWin()
    {
        panelWin.SetActive(true);
    }

    public void ShowPanelShop()
    {
        panelShop.SetActive(true);
    }

    public void UpdateHintText(int numberHint)
    {
        txtNumberHint.text = numberHint.ToString();
    }
    public void UpdateLevel( int a)
    {
        lv.text = (a + 1).ToString();
    }
}
