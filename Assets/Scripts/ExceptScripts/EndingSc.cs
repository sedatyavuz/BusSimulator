using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndingSc : MonoBehaviour
{
    [SerializeField] GameObject endingCanvas;
    [SerializeField] private GameObject[] passengers;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private Vector3 forceVector;
    [SerializeField] private Vector3 forceVectorOfsset;
    [SerializeField] private float force;
    private GameObject bus;
    [Header("CameraElents")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float cameraAnimtionEndTime;
    private GameObject mainCamera;
    [Header("Passenger")]
    [SerializeField] GameObject stationParent;
    public static int totalPassengerAmount;
    void Start()
    {
        DOTween.Init();
        bus = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        foreach (Transform t in stationParent.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Passenger"))
            {
                totalPassengerAmount++;
            }
        }
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
            Vector3 angle = new Vector3(Random.Range(0,361), Random.Range(0, 361), Random.Range(0, 361));
            GameObject g = Instantiate(passengers[Random.Range(0, passengers.Length)], spawn + spawnOffset, Quaternion.Euler(angle));
            Rigidbody[] gRigis = g.GetComponentsInChildren<Rigidbody>();
            Collider[] gCols = g.GetComponentsInChildren<Collider>();
            StartCoroutine(setColsTrigger(gCols));
            foreach (Rigidbody r in gRigis)
            {
                r.mass = Random.Range(r.mass, r.mass);
                r.AddForce(new Vector3(Random.Range(forceVector.x-forceVectorOfsset.x, forceVector.x + forceVectorOfsset.x)
                    , Random.Range(forceVector.y - forceVectorOfsset.y, forceVector.y + forceVectorOfsset.y)
                    , Random.Range(forceVector.z - forceVectorOfsset.z, forceVector.z + forceVectorOfsset.z)) * force);
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
