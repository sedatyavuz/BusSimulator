using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Dreamteck.Splines;

public class BusSc : MonoBehaviour
{
    [HideInInspector] public bool isEnding = false;
    #region private
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private GameManager _gameManager;
    private SplineFollower _follower;
    [SerializeField] private GameObject mainCamera;
    private CameraFollower cameraSc;
    private SwerveHorizontal swerveHorizontal;
    private Collider busCollider;
    [Header("PassengerElement")]
    [SerializeField] GameObject capacityPanel;
    private int _passengerCapacity;
    [SerializeField] private TextMeshProUGUI passengerCapacityText;
    [SerializeField] private TextMeshProUGUI _currentPassengerAmountText;
    private int _currentPassengerAmount = 0;
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
    [SerializeField] float currentFuelReduceTime;
    private int totalFuel;
    private float currentFuel;
    [Header("CarElements")]
    [SerializeField] GameObject carParent;
    SplineFollower[] carFollowers;
    [Header("CarCrush")]
    [SerializeField] private Transform rightCarJumpTransform;
    [SerializeField] private Transform leftCarJumpTransform;
    Shake cameraShake;
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
    [SerializeField] private Transform rightForceObject;
    [SerializeField] private Transform leftForceObject;
    [SerializeField] private float forceVectorYOffset;
    [SerializeField] private float force;
    Sequence accelerationSequence;
    float startFollowSpeed;
    [Header("BusExploidElemets")]
    [SerializeField] private GameObject busElements;
    [SerializeField] private GameObject busExploidObject;
    [SerializeField] private float busExploidAnimEndTime =1.5f;
    [SerializeField] private float passengerThrowForExploidTime;
    [SerializeField] private Transform cointainer;
    [SerializeField] private float expForce;
    [SerializeField] private float expPosRandomize;
    [SerializeField] private float expMassRandomize;
    #endregion
    private void OnEnable()
    {
        //_currentPassengerAmount = 12;
        swerveHorizontal = GetComponent<SwerveHorizontal>();
        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _follower = GetComponent<SplineFollower>();
        carFollowers = carParent.GetComponentsInChildren<SplineFollower>();
        busCollider = GetComponent<Collider>();
        cameraSc = mainCamera.GetComponent<CameraFollower>();
        cameraShake = mainCamera.GetComponent<Shake>();
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
            busCollider.enabled = false;
            fuelPanel.SetActive(false);
            capacityPanel.SetActive(false);
            other.GetComponent<EndingSc>().endingPassengersThrow(_currentPassengerAmount, transform.position);
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
    bool capacityCheck = false;
    public void currentPassengerUpdate(int amount)
    {
        lock (_lock)
        {
            _currentPassengerAmount += amount;
            if (_currentPassengerAmount > _passengerCapacity && capacityCheck == false)
            {
                busExploid();
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
    public void SetCurrentPassengerAmount(int amount)
    {
        _currentPassengerAmount = amount;
        setCurrentPassengerAmountText();
    }

    void gameFailed()
    {
        fuelPanel.SetActive(false);
        isEnding = true;
        _follower.follow = false;
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = false;
        }
        GameManager.Instance.LevelState(false);
        swerveHorizontal._changeLine = false;
    }
    #region Fuel
    IEnumerator FuelReduce()
    {
        if (_follower.follow)
        {
            currentFuel -= currentFuelReduceAmount;
            FuelCheck();
            FuelUpdateMethod();
        }
        yield return new WaitForSeconds(currentFuelReduceTime);
        StartCoroutine(FuelReduce());
    }
    void FuelCheck()
    {
        if (currentFuel <= 0)
        {
            gameFailed();
        }
    }
    void FuelUpdateMethod()
    {
        fuelFilledImage.fillAmount = Mathf.Lerp(fuelFilledImage.fillAmount,(float) currentFuel/totalFuel,1);
        fuelFilledImage.color = Color.Lerp(Color.red, Color.green, fuelFilledImage.fillAmount);
    }

    #endregion
    public void busForStartMethod()
    {
        swerveHorizontal.enabled = true;
        _passengerCapacity = PlayerPrefs.GetInt("totalCapacity");
        totalFuel = PlayerPrefs.GetInt("totalFuel");
        capacityPanel.SetActive(true);
        fuelPanel.SetActive(true);
        passengerCapacityText.text = _passengerCapacity.ToString();
        currentFuel = totalFuel;
        StartCoroutine(FuelReduce());
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = true;
        }
        _follower.follow = true;
        cameraSc.isCameraFollow = true;
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
    #region BusExploid

    void busExploid()
    {
        Invoke("passengerThorowExploidMethod", passengerThrowForExploidTime);
        Invoke("LevelStateFalse", busExploidAnimEndTime);
        _skinnedMeshRenderer.enabled = false;
        busElements.SetActive(false);
        capacityPanel.SetActive(false);
        busExploidObject.SetActive(true);
        capacityCheck = true;
        fuelPanel.SetActive(false);
        isEnding = true;
        _follower.follow = false;
        swerveHorizontal._changeLine = false;
        foreach (SplineFollower s in carFollowers)
        {
            s.follow = false;
        }
    }
    void passengerThorowExploidMethod()
    {
        for(int i = 0; i < _currentPassengerAmount; i++)
        {
            passengerThorowExploid();
        }
    }
    void passengerThorowExploid()
    {
        Vector3 forceVectorCopy = Vector3.zero;
        if (Random.Range(0,2)==0)
        {
            forceVectorCopy = new Vector3(Random.Range(leftForceObject.position.x/ expPosRandomize, leftForceObject.position.x* expPosRandomize)
                , Random.Range(leftForceObject.position.y, leftForceObject.position.y* expPosRandomize)
                , Random.Range(leftForceObject.position.z/ expPosRandomize, leftForceObject.position.z* expPosRandomize));
        }
        else
        {
            forceVectorCopy = new Vector3(Random.Range(rightForceObject.position.x, rightForceObject.position.x * expPosRandomize)
                , Random.Range(rightForceObject.position.y, rightForceObject.position.y * expPosRandomize)
                , Random.Range(rightForceObject.position.z/ expPosRandomize, rightForceObject.position.z * expPosRandomize));
        }
        Vector3 quternionVector = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        GameObject g = Instantiate(passengers[Random.Range(0, passengers.Length)], transform.position, Quaternion.Euler(quternionVector));
        g.transform.SetParent(cointainer);
        Rigidbody[] gRigis = g.GetComponentsInChildren<Rigidbody>();
        Collider[] gCols = g.GetComponentsInChildren<Collider>();
        foreach (Collider c in gCols)
        {
            c.isTrigger = true;
        }
        foreach (Rigidbody r in gRigis)
        {
            r.mass = Random.Range(r.mass / expMassRandomize, r.mass * expMassRandomize);
            r.AddForce((forceVectorCopy - g.transform.position) * expForce);
        }
        Destroy(g, 9);
    }
    void LevelStateFalse()
    {
        GameManager.Instance.LevelState(false);
    }

    #endregion


    #region CarCrush
    void carCrush(Transform carTransform)
    {
        SplineFollower carFollower = carTransform.GetComponent<SplineFollower>();
        float carDistance = carFollower.motion.offset.x + carFollower.offsetModifier.keys[0].offset.x;
        float busDistance = _follower.motion.offset.x + _follower.offsetModifier.keys[0].offset.x;
        //Debug.Log(Vector3.Distance(new Vector2(carDistance, 0), new Vector2(busDistance, 0)));
        cameraShake.StartShake();
        if (Vector3.Distance(new Vector2(carDistance,0),new Vector2(busDistance, 0)) <= .21f)
        {
            passengerThrowLine(false, true);
            if (carTransform.GetComponent<SplineFollower>().motion.offset.x >= 0)
            {
                carRotate(carTransform, true, carFollower);
                carJump(carTransform, false);
            }
            else
            {
                carRotate(carTransform, false, carFollower);
                carJump(carTransform, true);
            }
        }
        else if (busDistance > carDistance)
        {
            passengerThrowLine(false,false);

            carRotate(carTransform, false, carFollower);

            carJump(carTransform, true);
        }
        else if (busDistance < carDistance)
        {
            passengerThrowLine(true,false);

            carRotate(carTransform, true, carFollower);

            carJump(carTransform, false);
        }
    }

    void carRotate(Transform carTransform,bool negative , SplineFollower carFollowerCopy)
    {
        float zRotateCopy = zRotate;
        float currentZ = 0;
        if (carFollowerCopy.direction == Spline.Direction.Backward || negative)
        {
            zRotateCopy = Mathf.Abs(zRotate) * -1;
        }
        if(negative && carFollowerCopy.direction == Spline.Direction.Backward)
        {
            zRotateCopy = Mathf.Abs(zRotateCopy);
        }
        DOTween.To(x => currentZ = x, 0, zRotateCopy, jumpTime)
            .OnUpdate(() =>
            {
                carTransform.eulerAngles = new Vector3(carTransform.eulerAngles.x, carTransform.eulerAngles.y, currentZ);
            });
    }
    void carJump(Transform carTransform, bool negative)
    {
        Vector3 xDistanceCopy = rightCarJumpTransform.position;
        if (negative)
        {
            xDistanceCopy = leftCarJumpTransform.position;
        }
        carTransform.DOJump(new Vector3(xDistanceCopy.x, -2, xDistanceCopy.z), yDistance, 1, jumpTime)
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
        currentPassengerUpdate(-passengerReduceCopy);
        if (_currentPassengerAmount <= 0)
        {
            gameFailed();
        }
        for (int i = 0; i < passengerReduceCopy; i++)
        {
            if (directCrush)
            {
                if (Random.Range(0, 2) == 0)
                {
                    throwRight = !throwRight;
                }  
            }
            Vector3 forceVectorCopy = Vector3.zero;
            if (throwRight == false)
            {
                forceVectorCopy = new Vector3(leftForceObject.position.x, leftForceObject.position.y, leftForceObject.position.z);
            }
            else
            {
                forceVectorCopy = new Vector3(rightForceObject.position.x, rightForceObject.position.y, rightForceObject.position.z);
            }
            Vector3 quternionVector = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            GameObject g = Instantiate(passengers[Random.Range(0, passengers.Length)], transform.position, Quaternion.Euler(quternionVector));
            Rigidbody[] gRigis = g.GetComponentsInChildren<Rigidbody>();
            Collider[] gCols = g.GetComponentsInChildren<Collider>();
            foreach (Collider c in gCols)
            {
                c.isTrigger = true;
            }
            foreach (Rigidbody r in gRigis)
            {
                r.mass = Random.Range(r.mass/2, r.mass*2);
                r.AddForce((forceVectorCopy - g.transform.position) * force);
            }
            Destroy(g, 2);
        }
    }
    #endregion
}
