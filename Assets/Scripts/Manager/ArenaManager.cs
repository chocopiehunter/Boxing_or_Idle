using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public static ArenaManager Instance { get; private set; }

    [SerializeField] private GameObject GymRoot;
    [SerializeField] private GameObject ArenaRoot;

    public bool IsReady
    {
        get
        {
            return GymRoot != null && ArenaRoot != null;
        }
    }

    private void Awake()
    {
        Instance = this;

        if (IsReady == false)
        {
            Debug.LogError("ArenaManager 초기화 실패. GymRoot 또는 MatchArenaRoot가 연결되지 않음");
            return;
        }

        GymRoot.SetActive(true);
        ArenaRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool TryEnterArena()
    {
        if (IsReady == false)
        {
            Debug.LogError("경기장 전환 실패. GymRoot 또는 MatchArenaRoot가 연결되지 않음");
            return false;
        }

        GymRoot.SetActive(false);
        ArenaRoot.SetActive(true);
        return true;
    }

    public bool TryReturnToGym()
    {
        if (IsReady == false)
        {
            Debug.LogError("체육관 복귀 실패. GymRoot 또는 MatchArenaRoot가 연결되지 않음");
            return false;
        }

        ArenaRoot.SetActive(false);
        GymRoot.SetActive(true);
        return true;
    }
}
