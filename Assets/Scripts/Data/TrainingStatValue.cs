using System;
using UnityEngine;

// 훈련 완료로 증가할 8개의 스탯 값만 담는 클래스
[Serializable]
public class TrainingStatValue
{
    public float Hp;
    public float Stamina;
    public float StandingOffense;
    public float StandingDefense;
    public float WrestlingOffense;
    public float WrestlingDefense;
    public float JiuJitsuOffense;
    public float JiuJitsuDefense;

    public void Add(TrainingStatValue value)
    {
        if (value == null)
        {
            return;
        }

        Hp = Hp + value.Hp;
        Stamina = Stamina + value.Stamina;
        StandingOffense = StandingOffense + value.StandingOffense;
        StandingDefense = StandingDefense + value.StandingDefense;
        WrestlingOffense = WrestlingOffense + value.WrestlingOffense;
        WrestlingDefense = WrestlingDefense + value.WrestlingDefense;
        JiuJitsuOffense = JiuJitsuOffense + value.JiuJitsuOffense;
        JiuJitsuDefense = JiuJitsuDefense + value.JiuJitsuDefense;
    }
}
