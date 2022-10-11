using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public UnityEvent GameStart = new();
    [HideInInspector] public UnityEvent GameReady = new();
    [HideInInspector] public UnityEvent GameEnd = new();
    [HideInInspector] public UnityEvent LevelSuccess = new();
    [HideInInspector] public UnityEvent LevelFail = new();
    [HideInInspector] public UnityEvent OnMoneyChange = new();

    [Header("FuelElements")]
    [SerializeField] private int startFuel;
    [SerializeField] private int fuelPlus;
    [SerializeField] private int startFuelPrice;
    [SerializeField] private int fuelPricePlus;
    [SerializeField] private TextMeshProUGUI currentFuelText;
    [SerializeField] private TextMeshProUGUI fuelUpgradePriceText;
    [SerializeField] private TextMeshProUGUI fuelPlusText;

    [Header("CapacityElemnts")]
    [SerializeField] private int startCapacity;
    [SerializeField] private int capacityPlus;
    [SerializeField] private int startCapacityPrice;
    [SerializeField] private int capacityPricePlus;
    [SerializeField] private TextMeshProUGUI currentCapacityText;
    [SerializeField] private TextMeshProUGUI capacityUpgradePriceText;
    [SerializeField] private TextMeshProUGUI capacityPlusText;

    private float playerMoney;
    public float PlayerMoney
    {
        get
        {
            return playerMoney;
        }
        set
        {
            playerMoney = value;
            OnMoneyChange.Invoke();
        }
    }

    private bool hasGameStart;
    public bool HasGameStart
    {
        get
        {
            return hasGameStart;
        }
        set
        {
            hasGameStart = value;
        }
    }

    private void Start()
    {
        LoadData();
        if (!PlayerPrefs.HasKey("totalFuel"))
        {
            PlayerPrefs.SetInt("totalFuel", startFuel);
        }
        if (!PlayerPrefs.HasKey("totalCapacity"))
        {
            PlayerPrefs.SetInt("totalCapacity", startCapacity);
        }
        if (!PlayerPrefs.HasKey("fuelPrice"))
        {
            PlayerPrefs.SetInt("fuelPrice", startFuelPrice);
        }
        if (!PlayerPrefs.HasKey("capacityPrice"))
        {
            PlayerPrefs.SetInt("capacityPrice", startCapacityPrice);
        }

        currentFuelText.text = getFuel().ToString() + "/Sn";
        fuelUpgradePriceText.text = getFuelPrice().ToString() + " $";
        fuelPlusText.text = "+"+fuelPlus.ToString() + "/Sn";

        currentCapacityText.text = getCapacity().ToString();
        capacityUpgradePriceText.text = getCapacityPrice().ToString() + " $";
        capacityPlusText.text = capacityPlus.ToString();
    }

    public void LevelState(bool value)
    {
       
        if (value)
        {
            LevelSuccess.Invoke();
        }
        else
        {
            LevelFail.Invoke();

        }
    }
        private void OnEnable()
    {
        GameStart.AddListener(() => hasGameStart = true);
        GameEnd.AddListener(() => hasGameStart = false);
    }

    private void OnDisable()
    {
        SaveData();
    }

    void LoadData()
    {
        PlayerMoney = PlayerPrefs.GetFloat("PlayerMoney", 0);
    }

    void SaveData()
    {
        PlayerPrefs.SetFloat("PlayerMoney", playerMoney);
    }

   
    public void fuelUpgrade()
    {
        if (playerMoney >= getFuelPrice())
        {
            playerMoney -= getFuelPrice();
            setFuel(fuelPlus);
            setFuelPrice(fuelPricePlus);
            currentFuelText.text = getFuel().ToString()+"/Sn";
            fuelUpgradePriceText.text = getFuelPrice().ToString()+" $";
            OnMoneyChange.Invoke();
        }
    }
    public void capacityUpgrade()
    {
        if (playerMoney >= getCapacityPrice())
        {
            playerMoney -= getCapacityPrice();
            setCapacity(capacityPlus);
            setCapacityPrice(capacityPricePlus);
            currentCapacityText.text = getCapacity().ToString();
            capacityUpgradePriceText.text = getCapacityPrice().ToString() + " $";
            OnMoneyChange.Invoke();
        }
    }

    int getFuelPrice()
    {
        return PlayerPrefs.GetInt("fuelPrice");
    }

    void setFuelPrice(int value)
    {
        PlayerPrefs.SetInt("fuelPrice", getFuelPrice() + value);
    }

    int getCapacityPrice()
    {
        return PlayerPrefs.GetInt("capacityPrice");
    }

    void setCapacityPrice(int value)
    {
        PlayerPrefs.SetInt("capacityPrice", getCapacityPrice() + value);
    }
    void setFuel(int value)
    {
        PlayerPrefs.SetInt("totalFuel", getFuel()+ value);
    }
    int getFuel()
    {
        return PlayerPrefs.GetInt("totalFuel");
    }

    int getCapacity()
    {
        return PlayerPrefs.GetInt("totalCapacity");
    }

    void setCapacity(int value)
    {
        PlayerPrefs.SetInt("totalCapacity", getCapacity() + value);
    }

    public void updateMoney()
    {
        OnMoneyChange.Invoke();
    }
}
