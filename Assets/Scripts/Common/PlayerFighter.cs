using UnityEngine;

public class PlayerFighter : MonoBehaviour
{
    public FighterModel Model { get; private set; }

    public void Bind(FighterModel model)
    {
        Model = model;
    }

    // 나중에 HUD넣을 곳
}
