using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndingSc : MonoBehaviour
{
    private GameObject player;
    private BusSc busSc;
    private SwerveHorizontal busSwerve;
    private SplineFollower busFollower;
    private GameObject mainCamera;
    [SerializeField] private Transform endingOrigin;
    [SerializeField] private float endingOriginMoveTime;
    [SerializeField] private Transform passengerRightOutOrigin;
    [SerializeField] private Transform passengerLeftOutOrigin;
    [SerializeField] private float passengerWalkEndTime;
    [SerializeField] private GameObject[] passengers;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        busSc = player.GetComponent<BusSc>();
        busFollower = player.GetComponent<SplineFollower>();
        busSwerve = player.GetComponent<SwerveHorizontal>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        DOTween.Init();
    }

    public void PassengersEndingMove(Transform rigthBackDoor,Transform leftBackDoor)
    {
        busFollower.follow = false;
        busSwerve._changeLine = false;
        player.transform.DOMove(new Vector3(endingOrigin.position.x, 0, endingOrigin.position.z), endingOriginMoveTime)
            .OnComplete(()=> 
            {
                mainCamera.GetComponent<CameraFollower>().isCameraFollow = false;
                mainCamera.transform.SetParent(endingOrigin);
                endingOrigin.GetComponent<Animator>().SetBool("Rotate", true);
                int currentPassengerAmount = busSc.GetCurrentPassengerAmount();
                StartCoroutine(method(currentPassengerAmount, rigthBackDoor, leftBackDoor));
            });
    }

    IEnumerator method(int passengerAmount,Transform rigthBackDoor, Transform leftBackDoor)
    {
        for (int i = 0; i < passengerAmount; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                int x = Random.Range(0, passengers.Length);
                GameObject g = Instantiate(passengers[x], new Vector3(rigthBackDoor.position.x, 0, rigthBackDoor.position.z), passengerRightOutOrigin.rotation);
                g.transform.DOMove(new Vector3(passengerRightOutOrigin.position.x, 0, passengerRightOutOrigin.position.z), passengerWalkEndTime);
                g.GetComponent<Animator>().SetBool("Walk", true);
                busSc.currentPassengerUpdate(false);
            }
            else
            {
                int x = Random.Range(0, passengers.Length);
                GameObject g = Instantiate(passengers[x], new Vector3(leftBackDoor.position.x, 0, leftBackDoor.position.z), passengerLeftOutOrigin.rotation);
                g.transform.DOMove(new Vector3(passengerLeftOutOrigin.position.x, 0, passengerLeftOutOrigin.position.z), passengerWalkEndTime);
                g.GetComponent<Animator>().SetBool("Walk", true);
                busSc.currentPassengerUpdate(false);
            }
            yield return new WaitForSeconds(.15f);
        }
    }
}
