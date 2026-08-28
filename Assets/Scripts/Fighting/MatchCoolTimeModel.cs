using System.Collections.Generic;
using UnityEngine;

public class MatchCoolTimeModel
{
    private Dictionary<string, float> _remainingSecondsBySkillId;
    private List<string> _coolingSkillIds;

    public void Reset()
    {
        _remainingSecondsBySkillId.Clear();
        _coolingSkillIds.Clear();
    }
}
