using UnityEngine;

public class MatchStepCalculator
{
    private const float MinStep = 0f;
    private const float MaxStep = 100f;
    private const float SlowestIntervalMultiplier = 1.2f;
    private const float FastestIntervalMultiplier = 0.8f;
    private const float ShortestMoveDistanceMultiplier = 0.8f;
    private const float LongestMoveDistanceMultiplier = 1.2f;

    public bool TryCalculate(float step, float baseIntervalSeconds, float baseMoveDistance, out float intervalSeconds, out float moveDistance)
    {
        intervalSeconds = 0f;
        moveDistance = 0f;

        if (step < MinStep || step > MaxStep)
        {
            return false;
        }

        if (baseIntervalSeconds <= 0f || baseMoveDistance <= 0f)
        {
            return false;
        }

        float normalizedStep = Mathf.InverseLerp(MinStep, MaxStep, step);
        float intervalMultiplier = Mathf.Lerp(SlowestIntervalMultiplier, FastestIntervalMultiplier, normalizedStep);
        float moveDistanceMultiplier = Mathf.Lerp(ShortestMoveDistanceMultiplier, LongestMoveDistanceMultiplier, normalizedStep);

        intervalSeconds = baseIntervalSeconds * intervalMultiplier;
        moveDistance = baseMoveDistance * moveDistanceMultiplier;

        return true;
    }
}
