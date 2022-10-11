using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class StationSc : MonoBehaviour
{
    #region private
    private Collider frontCollider;
    private List<GameObject> _passengers;
    private SwerveHorizontal _busSwerve;
    private SplineFollower _busFollower;
    private BusSc _busSc;
    [SerializeField] private GameObject[] outPassengers;
    [Header("PassengerElement")]
    [SerializeField] private TextMeshProUGUI _passengerCountText;
    [SerializeField] private float _passengersEndTime;
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
        frontCollider = GetComponent<Collider>();
        DOTween.Init();
        _passengers = new List<GameObject>();
        Transform[] array = GetComponentsInChildren<Transform>();

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].transform.CompareTag("Passenger"))
            {
                _passengers.Add(array[i].gameObject);
            }
        }
        _passengerCountText.text = _passengers.Count.ToString();
    }


    IEnumerator PassengersMove()
    {
        #region reference
        GameObject busObject = GameObject.FindGameObjectWithTag("Player"); 
        _busSwerve = busObject.GetComponent<SwerveHorizontal>();
        _busSc = busObject.GetComponent<BusSc>();
        _busFollower = busObject.GetComponent<SplineFollower>();
        #endregion

        frontCollider.enabled = false;
        _busFollower.follow = false;
        _busSwerve._changeLine = false;
        float currentTime = Time.time;
        VectorsDetermine();
        int outPassengerCount = 0;//Random.Range(0, _busSc.GetCurrentPassengerAmount()+1);
        foreach (GameObject g in _passengers)
        {
            g.transform.DOMove(toBoardPassengersTarget, _passengersEndTime);
            g.GetComponent<Animator>().SetBool("Walk", true);
        }
        for (int i = 0; i < outPassengerCount; i++)
        {
            int x = Random.Range(0, outPassengers.Length);
            GameObject g = Instantiate(outPassengers[x], new Vector3(outPassengersSpawnPosition.x, 0, outPassengersSpawnPosition.z), _outPassengerTarget.rotation);
            g.GetComponent<Collider>().enabled = false;
            g.transform.DOMove(new Vector3(_outPassengerTarget.position.x, 0, _outPassengerTarget.position.z), _passengersEndTime);
            g.GetComponent<Animator>().SetBool("Walk", true);
            _busSc.currentPassengerUpdate(-1);
            yield return new WaitForSeconds(.15f);
        }
        yield return new WaitForSeconds(_passengersEndTime - (Time.time - currentTime));
        if (_busSc.isEnding == false)
        {
            _busFollower.follow = true;
            _busSwerve._changeLine = true;
        }
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
