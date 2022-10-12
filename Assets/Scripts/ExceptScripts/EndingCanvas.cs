using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EndingCanvas : MonoBehaviour
{
    [SerializeField] private float nextLevelForPercent;
    [SerializeField] private float nextLevelForPercentCheckTime;
    [SerializeField] private Image filledImage;
    [SerializeField] private TextMeshProUGUI percentText;
    int currentRagdoll = 0;
    float percentRogdall = 0;
    private void Start()
    {
        DOTween.Init();
        Invoke("nextLevelCheck", nextLevelForPercentCheckTime);
    }

    public void EndingCanvasPercent()
    {
        currentRagdoll++;
        percentRogdall = (float)currentRagdoll / EndingSc.totalPassengerAmount;
        filledImage.fillAmount = percentRogdall;
        filledImage.color = Color.Lerp(Color.red, Color.green, percentRogdall);
        percentText.text = (percentRogdall*100).ToString("0.00")+"%";
    }

    void nextLevelCheck()
    {
        gameObject.SetActive(false);
        if ((percentRogdall * 100)>= nextLevelForPercent)
        {
            GameManager.Instance.LevelState(true);
        }
        else
        {
            GameManager.Instance.LevelState(false);
        }
    }
}
