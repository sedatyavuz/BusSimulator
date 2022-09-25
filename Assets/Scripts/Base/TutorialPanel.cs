using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    GameObject bus;
    [SerializeField] GameObject upgradePanel;
    private void Start()
    {
        bus = GameObject.FindGameObjectWithTag("Player");
    }
    public void TutorialDeActive()
    {
        GameManager.Instance.GameStart.Invoke();
        gameObject.SetActive(false);
        bus.GetComponent<BusSc>().busForStartMethod();
        bus.GetComponent<SwerveHorizontal>()._changeLine = true;
        upgradePanel.SetActive(false);
    }
}
