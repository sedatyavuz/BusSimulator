using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dreamteck.Splines;

public class EndingMultiplier : MonoBehaviour
{
    [SerializeField] GameObject rightPanel;
    [SerializeField] private int rightMultiplier;
    [SerializeField] private TextMeshProUGUI rightMultiplierText;
    [SerializeField] GameObject leftPanel;
    [SerializeField] private int leftMultiplier;
    [SerializeField] private TextMeshProUGUI leftMultiplierText;
    [SerializeField] private BusSc busSc;
    [SerializeField] SplineFollower busFollower;
    [SerializeField] float lineCenter;

    private void OnEnable()
    {
        rightMultiplierText.text = "X" + rightMultiplier;
        leftMultiplierText.text = "X" + leftMultiplier;
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (busFollower.motion.offset.x >= lineCenter)
            {
                rightPanel.SetActive(false);
                busSc.SetCurrentPassengerAmount(busSc.GetCurrentPassengerAmount() * rightMultiplier);
            }
            else
            {
                leftPanel.SetActive(false);
                busSc.SetCurrentPassengerAmount(busSc.GetCurrentPassengerAmount() * leftMultiplier);
            }
        }
    }
    public int GetRigthMultiplier()
    {
        return rightMultiplier;
    }
    public int GetLeftMultiplier()
    {
        return leftMultiplier;
    }
}
