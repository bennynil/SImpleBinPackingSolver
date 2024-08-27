using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class userInterface : MonoBehaviour
{
    [SerializeField] GameObject demandConfigurer;
    [SerializeField] GameObject solvingConfigurer;

    [SerializeField] Image panel;
    [SerializeField] GameObject viewConfigurer;

    [SerializeField] GameObject setting;

    public void openDemandConfigurer()
    {
        demandConfigurer.SetActive(true);
        solvingConfigurer.SetActive(false);
        viewConfigurer.SetActive(false);
        setting.SetActive(false);
        panel.color = Color.black;
    }

    public void openSolvingConfigurer()
    {
        solvingConfigurer.SetActive(true);
        demandConfigurer.SetActive(false);
        viewConfigurer.SetActive(false);
        setting.SetActive(false);
        panel.color = Color.black;
    }

    public void openViewConfigurer()
    {
        viewConfigurer.SetActive(true);
        solvingConfigurer.SetActive(false);
        demandConfigurer.SetActive(false);
        setting.SetActive(false);
        panel.color = Color.clear;
    }

    public void openSetting()
    {
        setting.SetActive(true);
        solvingConfigurer.SetActive(false);
        demandConfigurer.SetActive(false);
        viewConfigurer.SetActive(false);
        panel.color = Color.black;
    }

}


