using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    GameObject bus;
    private void Start()
    {
        GameManager.Instance.GameStart.AddListener(busReference);
    }
    void busReference()
    {
        bus = GameObject.FindGameObjectWithTag("Player");
    }
    public void TutorialDeActive()
    {
        GameManager.Instance.GameStart.Invoke();
        gameObject.SetActive(false);
        bus.GetComponent<BusSc>().busForStartMethod();
        bus.GetComponent<SwerveHorizontal>()._changeLine = true;
    }
}
