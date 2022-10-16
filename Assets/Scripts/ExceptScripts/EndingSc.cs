using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndingSc : MonoBehaviour
{
    [SerializeField] EndingMultiplier endingMultiplier;
    [SerializeField] GameObject endingCanvas;
    [SerializeField] private GameObject[] passengers;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private Transform forceVector;
    [SerializeField] private Vector3 forceVectorOfsset;
    [SerializeField] private float force;
    [SerializeField] private GameObject bus;
    [Header("CameraElents")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float cameraAnimtionEndTime;
    [SerializeField] private GameObject mainCamera;
    [Header("Passenger")]
    [SerializeField] GameObject stationParent;
    public static int totalPassengerAmount;
    [SerializeField] Transform container;

    private void OnEnable()
    {
        DOTween.Init();
        int totalPassengerAmountCopy = 0;
        foreach (Transform t in stationParent.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Passenger"))
            {
                totalPassengerAmountCopy++;
            }
        }
        totalPassengerAmount = totalPassengerAmountCopy;
        if (endingMultiplier.GetRigthMultiplier() > endingMultiplier.GetLeftMultiplier())
        {
            totalPassengerAmount = totalPassengerAmount * endingMultiplier.GetRigthMultiplier();
        }
        else
        {
            totalPassengerAmount = totalPassengerAmount * endingMultiplier.GetLeftMultiplier();
        }
    }
    void Start()
    {
        
    }
    public void endingPassengersThrow(int passengersAmount, Vector3 spawn)
    {
        endingCanvas.SetActive(true);
        bus.GetComponent<SplineFollower>().follow = false;
        mainCamera.GetComponent<CameraFollower>().isCameraFollow = false;
        mainCamera.transform.DOMove(cameraTarget.position, cameraAnimtionEndTime);
        mainCamera.transform.DORotate(cameraTarget.eulerAngles, cameraAnimtionEndTime);
        for (int i = 0; i < passengersAmount; i++)
        {
            Vector3 angle = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            GameObject g = Instantiate(passengers[Random.Range(0, passengers.Length)], spawn + spawnOffset, Quaternion.Euler(angle));
            g.transform.SetParent(container);
            Rigidbody[] gRigis = g.GetComponentsInChildren<Rigidbody>();
            Collider[] gCols = g.GetComponentsInChildren<Collider>();
            StartCoroutine(setColsTrigger(gCols));
            Vector3 forceVectorCopy = forceVector.position-g.transform.position;

            forceVectorCopy = new Vector3(Random.Range(forceVectorCopy.x - forceVectorOfsset.x, forceVectorCopy.x + forceVectorOfsset.x)
                , Random.Range(forceVectorOfsset.y - (forceVectorOfsset.y/3), forceVectorOfsset.y)
                , Random.Range(forceVectorCopy.z - forceVectorOfsset.z, forceVectorCopy.z + forceVectorOfsset.z));
            foreach (Rigidbody r in gRigis)
            {
                //r.mass = Random.Range(r.mass/2, r.mass*2);
                r.AddForce(forceVectorCopy*force);
            }
        }
    }
    IEnumerator setColsTrigger(Collider[] gCols)
    {
        foreach(Collider c in gCols)
        {
            c.isTrigger = true;
        }
        yield return new WaitForSeconds(1);
        foreach (Collider c in gCols)
        {
            if (!c.CompareTag("RagdollPassenger"))
            {
                c.isTrigger = false;
            }
        }
    }
}
