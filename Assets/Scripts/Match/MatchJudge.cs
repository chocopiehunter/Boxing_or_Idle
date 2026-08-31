using System.Collections.Generic;
using UnityEngine;

public class MatchJudge : IMatchJudge
{
    private const float DamageScoreMultiplier = 100f;
    private const float SignificantStrikeScore = 1f;
    private const float TakedownScore = 1f;

    public MatchResult JudgeRound(MatchRoundRecord roundRecord)
    {
        if (roundRecord == null)
        {
            Debug.LogError("라운드 판정 실패. MatchRoundRecord 없음");
            return MatchResult.None;
        }

        float playerDamageScore = roundRecord.OpponentHpLostRate * DamageScoreMultiplier;

        float opponentDamageScore = roundRecord.PlayerHpLostRate * DamageScoreMultiplier;

        float playerSignificantStrikeScore = roundRecord.PlayerSignificantStrikesSucceeded * SignificantStrikeScore;

        float opponentSignificantStrikeScore = roundRecord.OpponentSignificantStrikesSucceeded * SignificantStrikeScore;

        float playerTakedownScore = roundRecord.PlayerTakedownsSucceeded * TakedownScore;

        float opponentTakedownScore = roundRecord.OpponentTakedownsSucceeded * TakedownScore;

        float playerRoundScore = playerDamageScore + playerSignificantStrikeScore + playerTakedownScore;

        float opponentRoundScore = opponentDamageScore + opponentSignificantStrikeScore + opponentTakedownScore;

        if (Mathf.Approximately(playerRoundScore, opponentRoundScore))
        {
            return MatchResult.Draw;
        }

        if (playerRoundScore > opponentRoundScore)
        {
            return MatchResult.Win;
        }

        return MatchResult.Lose;
    }

    public MatchResult JudgeMatch(IReadOnlyList<MatchRoundRecord> roundRecords)
    {
        if (roundRecords == null || roundRecords.Count == 0)
        {
            Debug.LogError("최종 판정 실패. 라운드 기록 없음");
            return MatchResult.None;
        }

        int playerRoundWins = 0;
        int opponentRoundWins = 0;

        for (int i = 0; i < roundRecords.Count; i++)
        {
            MatchResult roundResult = JudgeRound(roundRecords[i]);

            if (roundResult == MatchResult.None)
            {
                Debug.LogError($"최종 판정 실패. {roundRecords[i]?.RoundNumber}라운드 판정 결과 없음");
                return MatchResult.None;
            }

            if (roundResult == MatchResult.Win)
            {
                playerRoundWins = playerRoundWins + 1;
            }
            else if (roundResult == MatchResult.Lose)
            {
                opponentRoundWins = opponentRoundWins + 1;
            }
        }

        if (playerRoundWins > opponentRoundWins)
        {
            return MatchResult.Win;
        }

        if (playerRoundWins  < opponentRoundWins)
        {
            return MatchResult.Lose;
        }

        return MatchResult.Draw;
    }
}
