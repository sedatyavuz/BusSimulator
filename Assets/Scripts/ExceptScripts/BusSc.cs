using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Dreamteck.Splines;

public class BusSc : MonoBehaviour
{
    #region private
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private GameManager _gameManager;
    private SplineFollower _follower;
    private CameraFollower cameraSc;
    private SwerveHorizontal swerveHorizontal;
    private Collider busCollider;
    [Header("PassengerElement")]
    [SerializeField] GameObject capacityPanel;
    private int _passengerCapacity;
    [SerializeField] private TextMeshProUGUI passengerCapacityText;
    [SerializeField] private TextMeshProUGUI _currentPassengerAmountText;
    private int _currentPassengerAmount = 0;
    [SerializeField] private int passengerGainCoinAmount;
    [Header("DoorTransforms")]
    [SerializeField] private Transform _rightFrontDoor;
    [SerializeField] private Transform _leftFrontDoor;
    [SerializeField] private Transform _rightBackDoor;
    [SerializeField] private Transform _leftBackDoor;
    [Header("FuelElements")]
    [SerializeField] GameObject fuelPanel;
    [SerializeField] Image fuelFilledImage;
    [SerializeField] int fuelFillAmountSmooth;
    [SerializeField] float currentFuelReduceAmount;
    private int totalFuel;
    private float currentFuel;
    [Header("CarElements")]
    SplineFollower[] carFollowers;
    [Header("CarCrush")]
    Shake cameraShake;
    [SerializeField] private float xDistance;
    [SerializeField] private float yDistance;
    [SerializeField] private float zRotate;
    [SerializeField] private float jumpTime;
    [SerializeField] private float accelerationLineTime;
    [SerializeField] private float accelerationSpeedTime;
    [HideInInspector] public bool acceleration = false;
    [Header("CarCrushPassengerElements")]
    [SerializeField] private Vector3 passengerSpawnOffset;
    [SerializeField] private int passengerReduce;
    [SerializeField] private GameObject[] passengers;
    [SerializeField] private Vector3 forceVector;
    [SerializeField] private float force;
    Sequence accelerationSequence;
    float startFollowSpeed;

    #endregion

    void Start()
    {
        swerveHorizontal = GetComponent<SwerveHorizontal>();
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _follower = GetComponent<SplineFollower>();
        carFollowers = GameObject.Find("CarParent").GetComponentsInChildren<SplineFollower>();
        cameraSc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollower>();
        busCollider = GetComponent<Collider>();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Shake>();
        accelerationSequence = DOTween.Sequence();
        setCurrentPassengerAmountText();
        DOTween.Init();
        StartCoroutine(CarsFolloweSetFalse());
        startFollowSpeed = _follower.followSpeed;
    }

    
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.CompareTag("Car"))
        {
            other.GetComponent<SplineFollower>().follow = false;
            carCrush(other.transform);
            BusAcceleration();
        }
        if (other.transform.CompareTag("Ending"))
        {
            capacityPanel.SetActive(false);
            other.GetComponent<EndingSc>().PassengersEndingMove(_rightBackDoor, _leftBackDoor);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Station") && swerveHorizontal._changeLine)
        {
            other.GetComponent<StationSc>().targetAndSpawn(_rightFrontDoor, _leftFrontDoor, _rightBackDoor, _leftBackDoor);
        }
    }

    private object _lock = new object();
    public void currentPassengerUpdate(bool plus)
    {
        lock (_lock)
        {
            if (plus)
            {
                _currentPassengerAmount++;
                if (_currentPassengerAmount > _passengerCapacity)
                {
                    gameFailed();
                }
            }
            else
            {
                _currentPassengerAmount--;
                _gameManager.PlayerMoney += passengerGainCoinAmount;
                _gameManager.updateMoney();

            }
            float x = 0;

            x = (100 / _passengerCapacity) * _currentPassengerAmount;

            _skinnedMeshRenderer.SetBlendShapeWeight(0, x);
            setCurrentPassengerAmountText();
        }
    }
    public void setCurrentPassengerAmountText()
    {
        _currentPassengerAmountText.text = _currentPassengerAmount.ToString();
    }
    public int GetCurrentPassengerAmount()
    {
        return _currentPassengerAmount;
    }

    void gameFailed()
    {
        _follower.follow = false;
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = false;
        }
        GameManager.Instance.LevelState(false);
    }

    IEnumerator FuelUpdateMethod()
    {
        if (_follower.follow)
        {
            currentFuel -= currentFuelReduceAmount;
            if (currentFuel <= currentFuelReduceAmount*-1)
            {
                gameFailed();
            }
            else
            {
                float curentFillAmount = fuelFilledImage.fillAmount;
                for (int i = 0; i < fuelFillAmountSmooth; i++)
                {
                    fuelFilledImage.fillAmount = Mathf.Lerp(curentFillAmount, (float)currentFuel / totalFuel, (float)i / fuelFillAmountSmooth);
                    fuelFilledImage.color = Color.Lerp(Color.red, Color.green, fuelFilledImage.fillAmount);
                    yield return new WaitForSeconds((float)currentFuelReduceAmount / fuelFillAmountSmooth);
                }
                StartCoroutine(FuelUpdateMethod());
            }
        }
        else
        {
            yield return new WaitForSeconds(currentFuelReduceAmount);
            StartCoroutine(FuelUpdateMethod());
        }
    }

    public void busForStartMethod()
    {
        _passengerCapacity = PlayerPrefs.GetInt("totalCapacity");
        totalFuel = PlayerPrefs.GetInt("totalFuel");
        capacityPanel.SetActive(true);
        fuelPanel.SetActive(true);
        passengerCapacityText.text = _passengerCapacity.ToString();
        currentFuel = totalFuel;
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = true;
        }
        _follower.follow = true;
        cameraSc.isCameraFollow = true;
        StartCoroutine(FuelUpdateMethod());
    }

    public int TotalFuel
    {
        get
        {
            return totalFuel;
        }
        set
        {
            totalFuel = value;
        }
    }
    public int PassengerCapacity
    {
        get
        {
            return _passengerCapacity;
        }
        set
        {
            _passengerCapacity = value;
        }
    }

    IEnumerator CarsFolloweSetFalse()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = false;
        }
    }


    #region CarCrush
    void carCrush(Transform carTransform)
    {
        cameraShake.StartShake();
        if (Vector3.Distance(new Vector3(transform.position.x, 0, 0), new Vector3(carTransform.position.x, 0, 0)) <= .2f)
        {
            passengerThrowLine(false, true);
            if (carTransform.GetComponent<SplineFollower>().motion.offset.x >= 0)
            {
                carRotate(carTransform, true);
                carJump(carTransform, false);
            }
            else
            {
                carRotate(carTransform, false);
                carJump(carTransform, true);
            }
        }
        else if (transform.position.x > carTransform.position.x)
        {
            passengerThrowLine(false,false);

            carRotate(carTransform, false);

            carJump(carTransform, true);
        }
        else if (transform.position.x < carTransform.position.x)
        {
            passengerThrowLine(true,false);

            carRotate(carTransform, true);

            carJump(carTransform, false);
        }
    }

    void carRotate(Transform carTransform,bool negative)
    {
        float zRotateCopy = zRotate;
        float currentZ = 0;
        if (carTransform.eulerAngles.y == 180 || negative)
        {
            zRotateCopy = Mathf.Abs(zRotate) * -1;
        }
        DOTween.To(x => currentZ = x, 0, zRotateCopy, jumpTime)
            .OnUpdate(() =>
            {
                carTransform.eulerAngles = new Vector3(carTransform.eulerAngles.x, carTransform.eulerAngles.y, currentZ);
            });
    }
    void carJump(Transform carTransform, bool negative)
    {
        float xDistanceCopy = xDistance;
        if (negative)
        {
            xDistanceCopy = -xDistance;
        }
        carTransform.DOJump(new Vector3(carTransform.position.x + xDistanceCopy, -2, carTransform.position.z), yDistance, 1, jumpTime)
                   .OnComplete(() =>
                   {
                       Destroy(carTransform.gameObject);
                   });
    }

    void BusAcceleration()
    {
        accelerationSequence.Kill();
        float currentSpeed = 0;
        accelerationSequence.Append(DOTween.To(x => currentSpeed = x, startFollowSpeed/3, startFollowSpeed, accelerationSpeedTime)
            .OnUpdate(() =>
            {
                _follower.followSpeed = currentSpeed;
            }));
        
        if (swerveHorizontal._changeLine == false)
        {
            acceleration = true;
            float currentX = 0;
            DOTween.To(x => currentX = x, _follower.motion.offset.x, swerveHorizontal.nextXPosition, accelerationLineTime)
            .OnUpdate(() =>
            {
                _follower.motion.offset = new Vector2(currentX, _follower.motion.offset.y);

            }).OnComplete(() =>
            {
                acceleration = false;
                swerveHorizontal._changeLine = true;
            });
        }
    }

    void passengerThrowLine(bool throwRight,bool directCrush)
    {
        int passengerReduceCopy = 0;
        if (_currentPassengerAmount >= passengerReduce)
        {
            passengerReduceCopy = _currentPassengerAmount / passengerReduce;
        }
        else if(_currentPassengerAmount != 0)
        {
            passengerReduceCopy = 1;
        }
        Debug.Log(passengerReduceCopy);
        _currentPassengerAmount -= passengerReduceCopy;
        setCurrentPassengerAmountText();
        for (int i = 0; i < passengerReduceCopy; i++)
        {
            if (directCrush)
            {
                if (Random.Range(0, 2) == 0)
                {
                    throwRight = !throwRight;
                }  
            }
            Vector3 forceVectorCopy;
            if (throwRight == false)
            {
                forceVectorCopy = new Vector3(forceVector.x * -1, forceVector.y, forceVector.z);
            }
            else
            {
                forceVectorCopy = forceVector;
            }
            Vector3 quternionVector = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            GameObject g = Instantiate(passengers[Random.Range(0, passengers.Length)], transform.position + passengerSpawnOffset, Quaternion.Euler(quternionVector));
            Rigidbody[] gRigis = g.GetComponentsInChildren<Rigidbody>();
            Collider[] gCols = g.GetComponentsInChildren<Collider>();
            foreach (Collider c in gCols)
            {
                c.isTrigger = true;
            }
            foreach (Rigidbody r in gRigis)
            {
                r.mass = Random.Range(r.mass / 2, r.mass * 2);
                r.AddForce(forceVectorCopy * force);
            }
            Destroy(g, 4);
        }
    }
    #endregion
}
