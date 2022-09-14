using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class StationSc : MonoBehaviour
{
    #region private
    private List<Transform> _passengers;
    private SwerveHorizontal _busSwerve;
    private SplineFollower _busFollower;
    private BusSc _busSc;
    [Header("PassengerElement")]
    [SerializeField] private TextMeshProUGUI _passengerCountText;
    [SerializeField] private TextMeshProUGUI _passengerOutCountText;
    [SerializeField] private int _passengerOutCount;
    [SerializeField] private float _passengersEndTime;
    [SerializeField] private GameObject _passengerObject;
    [SerializeField] private Transform _outPassengerTarget;

    [Header("PassengersTargetAndSpawnDetermine")]
    private Transform rightFrontDoor;
    private Transform leftFrontDoor;
    private Transform rightBackDoor;
    private Transform leftBackDoor;
    private Vector3 toBoardPassengersTarget;
    private Vector3 outPassengersSpawnPosition;
    #endregion
    void Start()
    {
        DOTween.Init();
        _busSwerve = GameObject.FindGameObjectWithTag("Player").GetComponent<SwerveHorizontal>();
        _busSc = GameObject.FindGameObjectWithTag("Player").GetComponent<BusSc>();
        _busFollower = GameObject.FindGameObjectWithTag("Player").GetComponent<SplineFollower>();
        _passengers = new List<Transform>();
        Transform[] array = GetComponentsInChildren<Transform>();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].transform.CompareTag("Passenger"))
            {
                _passengers.Add(array[i]);
            }
        }
        _passengerCountText.text = _passengers.Count.ToString();
        _passengerOutCountText.text = _passengerOutCount.ToString();
    }


    IEnumerator PassengersMove()
    {
        float startBusSpeed = _busFollower.followSpeed;
        _busFollower.followSpeed = 0;
        _busSwerve._changeLine = false;
        float currentTime = Time.time;
        VectorsDetermine();
        float outPassengerCount;
        if (_busSc.GetCurrentPassengerAmount() > _passengerOutCount)
        {
            outPassengerCount = _passengerOutCount;
        }
        else
        {
            outPassengerCount = _busSc.GetCurrentPassengerAmount();
        }
        foreach (Transform t in _passengers)
        {
            t.DOMove(toBoardPassengersTarget, _passengersEndTime);
        }
        for (int i = 0; i < outPassengerCount; i++)
        {
            GameObject g = Instantiate(_passengerObject, outPassengersSpawnPosition, Quaternion.identity);
            g.transform.DOMove(_outPassengerTarget.position, _passengersEndTime);
            yield return new WaitForSeconds(.15f);
        }
        yield return new WaitForSeconds(_passengersEndTime - (Time.time - currentTime));
        _busFollower.followSpeed = startBusSpeed;
        _busSwerve._changeLine = true;
    }
    void VectorsDetermine()
    {
        float rightDistance = Vector3.Distance(transform.position, rightFrontDoor.position);
        float leftDistance = Vector3.Distance(transform.position, leftFrontDoor.position);

        if (rightDistance <= leftDistance)
        {
            toBoardPassengersTarget = rightFrontDoor.position;
            outPassengersSpawnPosition = rightBackDoor.position;
        }
        else
        {
            toBoardPassengersTarget = leftFrontDoor.position;
            outPassengersSpawnPosition = leftBackDoor.position;
        }
    }

    public void targetAndSpawn(Transform rightFrontDoor, Transform leftFrontDoor, Transform rightBackDoor, Transform leftBackDoor)
    {
        this.rightFrontDoor = rightFrontDoor;
        this.leftFrontDoor = leftFrontDoor;
        this.rightBackDoor = rightBackDoor;
        this.leftBackDoor = leftBackDoor;

        StartCoroutine(PassengersMove());
    }
}
