using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EndingDetectionSc : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] Canvas endingCanvasObject;
    [SerializeField] private EndingCanvas endingCanvas;
    [Header("MoneyElemets")]
    [SerializeField] private float addedMoney;
    [SerializeField] private Image moneyImage;
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private float startScale =.5f;
    [SerializeField] private float targetScale = 1;
    [SerializeField] private float animationEndTime=1.7f;
    GameManager gameManager;
    private void Start()
    {
        DOTween.Init();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("RagdollPassenger"))
        {
            MoneyImageElements(other);
            other.GetComponent<Collider>().enabled = false;
            SetPassengerPercentCount();
        }
    }
   
    void MoneyImageElements(Collider other)
    {
        Image i = Instantiate(moneyImage, mainCam.WorldToScreenPoint(other.transform.position), Quaternion.identity);
        i.transform.SetParent(endingCanvasObject.transform);
        i.rectTransform.localScale = new Vector3(startScale, startScale, startScale);
        i.rectTransform.DOScale(new Vector3(targetScale, targetScale, targetScale), animationEndTime).SetEase(Ease.InQuad);
        i.rectTransform.DOAnchorPos(targetTransform.anchoredPosition, animationEndTime).SetEase(Ease.InQuad)
            .OnComplete(()=> 
            {
                setMoney();
                Destroy(i.gameObject);
            });
    }
    private object _lock = new object();

    public void SetPassengerPercentCount()
    {
        lock (_lock)
        {
            endingCanvas.EndingCanvasPercent();
        }
    }

    private object moneyLock = new object();

    public void setMoney()
    {
        lock (moneyLock)
        {
            gameManager.PlayerMoney += addedMoney;
        }
    }
}
