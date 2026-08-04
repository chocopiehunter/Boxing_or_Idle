using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Inst { get; set; }

    private void Awake()
    {
        Inst = this;
        InitNetworkService();
    }

    private void InitNetworkService()
    {
        // 앞으로 네트워크 매니저에 사용할 다양한 서비스를 생성

    }
}
