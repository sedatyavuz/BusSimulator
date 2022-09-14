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

    [Header("PassengerElement")]
    [SerializeField] GameObject capacityPanel;
    private int _passengerCapacity;
    [SerializeField] private TextMeshProUGUI passengerCapacityText;
    [SerializeField] private TextMeshProUGUI _currentPassengerAmountText;
    private int _currentPassengerAmount=0;
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
    private int totalFuel;
    private int currentFuel;
    [Header("CarElements")]
    SplineFollower[] carFollowers;
    #endregion

    void Start()
    {

        _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _follower = GetComponent<SplineFollower>();
        carFollowers = GameObject.Find("CarParent").GetComponentsInChildren<SplineFollower>();
        setCurrentPassengerAmountText();
        DOTween.Init();
    }

    
    void Update()
    {

        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Station"))
        {
            other.GetComponent<StationSc>().targetAndSpawn(_rightFrontDoor, _leftFrontDoor, _rightBackDoor, _leftBackDoor);
        }
        if (other.transform.CompareTag("Passenger"))
        {
            Destroy(other.gameObject);
            currentPassengerUpdate(true);
        }
        if (other.transform.CompareTag("Car"))
        {
            gameFailed();
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
        if (_follower.followSpeed > 0)
        {
            currentFuel--;
            if (currentFuel <= -1)
            {
                gameFailed();
            }
            else
            {
                float curentFillAmount = fuelFilledImage.fillAmount;
                for (int i = 0; i < fuelFillAmountSmooth; i++)
                {
                    fuelFilledImage.fillAmount = Mathf.Lerp(curentFillAmount, (float)currentFuel / totalFuel, (float)i / fuelFillAmountSmooth);
                    yield return new WaitForSeconds((float)1 / fuelFillAmountSmooth);
                }
                StartCoroutine(FuelUpdateMethod());
            }
        }
        else
        {
            yield return new WaitForSeconds(1);
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
}
