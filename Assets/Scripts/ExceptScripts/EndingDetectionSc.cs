using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingDetectionSc : MonoBehaviour
{
    [SerializeField] private EndingCanvas endingCanvas;
    [SerializeField] private float addedMoney;
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("RagdollPassenger"))
        {
            other.GetComponent<Collider>().enabled = false;
            gameManager.PlayerMoney += addedMoney;
            SetPassengerPercentCount();
        }
    }
    private object _lock = new object();

    public void SetPassengerPercentCount()
    {
        lock (_lock)
        {
            endingCanvas.EndingCanvasPercent();
        }
    }
}
