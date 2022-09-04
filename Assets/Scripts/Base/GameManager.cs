using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public UnityEvent GameStart = new();
    [HideInInspector] public UnityEvent GameReady = new();
    [HideInInspector] public UnityEvent GameEnd = new();
    [HideInInspector] public UnityEvent LevelSuccess = new();
    [HideInInspector] public UnityEvent LevelFail = new();
    [HideInInspector] public UnityEvent OnMoneyChange = new();

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
    }

    public void LevelState(bool value)
    {
        if (value) LevelSuccess.Invoke();
        else LevelFail.Invoke();
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
}
