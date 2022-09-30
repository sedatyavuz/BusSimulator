using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EndingCanvas : MonoBehaviour
{

    
    [SerializeField] private float succesPercent;
    [SerializeField] private Image filledImage;
    [SerializeField] private TextMeshProUGUI percentText;
    int currentRagdoll = 0;
    private void Start()
    {
        DOTween.Init();
    }

    public void EndingCanvasPercent()
    {
        currentRagdoll++;
        float percentRogdall = (float)currentRagdoll / EndingSc.totalPassengerAmount;
        filledImage.fillAmount = percentRogdall;
        filledImage.color = Color.Lerp(Color.red, Color.green, percentRogdall);
        percentText.text = (percentRogdall*100).ToString("0.00")+"%";
    }
}
