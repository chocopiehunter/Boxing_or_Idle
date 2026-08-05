using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
}
