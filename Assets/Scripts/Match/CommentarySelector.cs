using System;
using System.Collections.Generic;

public class CommentarySelector
{
    private const string NoneValue = "None";
    private const string AnyValue = "Any";

    private readonly Random _random = new Random();

    public CommentaryData Select(Dictionary<string, CommentaryData> commentaryDataList, CombatActionResult actionResult, MatchCombatModel combatModel)
    {
        if (commentaryDataList == null || actionResult == null || actionResult.Action == null || actionResult.Action.SelectedSkill == null || combatModel == null)
        {
            return null;
        }

        List<CommentaryData> candidates = new List<CommentaryData>();

        int totalWeight = 0;

        foreach(KeyValuePair<string, CommentaryData> pair in commentaryDataList)
        {
            CommentaryData commentaryData = pair.Value;

            if (IsMatchingData(commentaryData, actionResult, combatModel) == false)
            {
                continue;
            }

            if (commentaryData.Weight <= 0 || string.IsNullOrEmpty(commentaryData.CommentaryText) == true)
            {
                continue;
            }

            candidates.Add(commentaryData);

            totalWeight += commentaryData.Weight;
        }

        if (candidates.Count == 0 || totalWeight <= 0)
        {
            return null;
        }

        int selectedWeight = _random.Next(0, totalWeight);

        int accumulatedWeight = 0;

        for (int i = 0; i < candidates.Count; i++)
        {
            CommentaryData candidate = candidates[i];

            accumulatedWeight += candidate.Weight;

            if (selectedWeight < accumulatedWeight)
            {
                return candidate;
            }
        }

        return candidates[candidates.Count - 1];
    }

    private bool IsMatchingData(CommentaryData commentaryData, CombatActionResult actionResult, MatchCombatModel combatModel)
    {
        if (commentaryData == null)
        {
            return false;
        }

        if (IsFilterMatch(commentaryData.ResultType, actionResult.ResultType.ToString()) == false)
        {
            return false;
        }
        if (IsFilterMatch(commentaryData.SkillId, actionResult.Action.SelectedSkill.Id) == false)
        {
            return false;
        }

        if (IsFilterMatch(commentaryData.MatchSituation, combatModel.CurrentSituation.ToString()) == false)
        {
            return false;
        }

        if (IsFilterMatch(commentaryData.WrestlingSituation, combatModel.CurrentWrestlingSituation.ToString()) == false)
        {
            return false;
        }

        if (IsFilterMatch(commentaryData.GroundPosition, combatModel.CurrentGroundPosition.ToString()) == false)
        {
            return false;
        }

        if (IsFilterMatch(commentaryData.SkillUserSide, actionResult.Action.SkillUserSide.ToString()) == false)
        {
            return false;
        }

        return true;
    }

    private bool IsFilterMatch(string filterValue, string currentValue)
    {
        if (string.IsNullOrEmpty(filterValue) == true)
        {
            return false;
        }

        if (filterValue == NoneValue || filterValue == AnyValue)
        {
            return true;
        }

        return filterValue == currentValue;
    }
}
