using UnityEngine;

public static class TrainingResultCalculator
{
    public static TrainingStatValue Calculate(FighterModel fighter, TrainingData trainingData, TrainingFacilityData facilityData)
    {
        if (fighter == null || trainingData == null || facilityData == null)
        {
            return null;
        }

        TrainingStatValue finalValue = GetBaseValue(facilityData);
        TrainingStatValue coachBonus = GetCoachBonus(fighter, trainingData, facilityData);

        // 스타일, 성격 등이 추가되면 들어갈 곳
        finalValue.Add(coachBonus);
        // finalValue.Add(GetStyleBonus());
        return finalValue;
    }

    private static TrainingStatValue GetBaseValue(TrainingFacilityData facilityData)
    {
        return new TrainingStatValue
        {
            Hp = facilityData.Hp,
            Stamina = facilityData.Stamina,
            StandingOffense = facilityData.StandingOffense,
            StandingDefense = facilityData.StandingDefense,
            WrestlingOffense = facilityData.WrestlingOffense,
            WrestlingDefense = facilityData.WrestlingDefense,
            JiuJitsuOffense = facilityData.JiuJitsuOffense,
            JiuJitsuDefense = facilityData.JiuJitsuDefense
        };
    }

    private static TrainingStatValue GetCoachBonus(FighterModel fighter, TrainingData trainingData, TrainingFacilityData facilityData)
    {
        return new TrainingStatValue();
    }
}
