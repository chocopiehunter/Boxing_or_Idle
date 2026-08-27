using System.Collections.Generic;

public interface IMatchJudge
{
    MatchResult JudgeRound(MatchRoundRecord roundRecord);

    MatchResult JudgeMatch(IReadOnlyList<MatchRoundRecord> roundRecords);
}
