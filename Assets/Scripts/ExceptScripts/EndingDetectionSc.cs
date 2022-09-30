using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingDetectionSc : MonoBehaviour
{
    [SerializeField] private EndingCanvas endingCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("RagdollPassenger"))
        {
            other.GetComponent<Collider>().enabled = false;
            SetPassengerPercentCount();
            Debug.Log("Asdasd");
        }
    }

    private object _lock = new object();

    public void SetPassengerPercentCount()
    {
        lock (_lock)
        {
            Debug.Log("Asdsad");
            endingCanvas.EndingCanvasPercent();
        }
    }
}
