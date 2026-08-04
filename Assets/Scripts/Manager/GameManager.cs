using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // LoadSaveData();
    }

    public void StartGame()
    {

    }

    public void SaveData()
    {

    }

    public void SaveAndEndGame()
    {

    }

    private void LoadSaveData()
    {

    }
}
