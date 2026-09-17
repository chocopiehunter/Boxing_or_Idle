using UnityEngine;

public class MatchStepMover
{
    public bool TryMove(MatchDistanceModel distanceModel, MatchStepType playerStepType, MatchStepType opponentStepType, float playerMoveDistance, float opponentMoveDistance)
    {
        if (distanceModel == null || distanceModel.IsReady == false)
        {
            return false;
        }

        if (playerMoveDistance < 0f || opponentMoveDistance < 0f)
        {
            return false;
        }

        Vector2 playerPosition = distanceModel.PlayerPosition;
        Vector2 opponentPosition = distanceModel.OpponentPosition;
        Vector2 playerToOpponent = opponentPosition - playerPosition;

        if (playerToOpponent.sqrMagnitude <= 0f)
        {
            return false;
        }

        Vector2 playerDirection = playerToOpponent.normalized;
        Vector2 opponentDirection = -playerDirection;

        Vector2 nextPlayerPosition = CalculateNextPosition(playerPosition, playerDirection, playerStepType, playerMoveDistance, distanceModel.AreaCenter);
        Vector2 nextOpponentPosition = CalculateNextPosition(opponentPosition, opponentDirection, opponentStepType, opponentMoveDistance, distanceModel.AreaCenter);

        nextPlayerPosition = distanceModel.GetPositionInsideArea(nextPlayerPosition);
        nextOpponentPosition = distanceModel.GetPositionInsideArea(nextOpponentPosition);

        float nextDistance = Vector2.Distance(nextPlayerPosition, nextOpponentPosition);

        if (nextDistance < distanceModel.MinDistance)
        {
            if (playerStepType == MatchStepType.Forward)
            {
                nextPlayerPosition = playerPosition;
            }

            if (opponentStepType == MatchStepType.Forward)
            {
                nextOpponentPosition = opponentPosition;
            }
        }

        return distanceModel.TrySetPositions(nextPlayerPosition, nextOpponentPosition);
    }

    private Vector2 CalculateNextPosition(Vector2 currentPosition, Vector2 directionToOpponent, MatchStepType stepType, float moveDistance, Vector2 areaCenter)
    {
        if (stepType == MatchStepType.Forward)
        {
            return currentPosition + directionToOpponent * moveDistance;
        }

        if (stepType == MatchStepType.Back)
        {
            return currentPosition - directionToOpponent * moveDistance;
        }

        if (stepType == MatchStepType.CircleLeft)
        {
            Vector2 leftDirection = new Vector2(-directionToOpponent.y, directionToOpponent.x);

            return currentPosition + leftDirection * moveDistance;
        }

        if (stepType == MatchStepType.CircleRight)
        {
            Vector2 rightDirection = new Vector2(directionToOpponent.y, -directionToOpponent.x);

            return currentPosition + rightDirection * moveDistance;
        }

        if (stepType == MatchStepType.CageEscapeLeft)
        {
            Vector2 leftDirection = new Vector2(-directionToOpponent.y, directionToOpponent.x);
            Vector2 directionToCenter = (areaCenter - currentPosition).normalized;
            Vector2 escapeDirection = (leftDirection * 0.75f + directionToCenter * 0.25f).normalized;

            return currentPosition + escapeDirection * moveDistance;
        }

        if (stepType == MatchStepType.CageEscapeRight)
        {
            Vector2 rightDirection = new Vector2(directionToOpponent.y, -directionToOpponent.x);
            Vector2 directionToCenter = (areaCenter - currentPosition).normalized;
            Vector2 escapeDirection = (rightDirection * 0.75f + directionToCenter * 0.25f).normalized;

            return currentPosition + escapeDirection * moveDistance;
        }

        return currentPosition;
    }
}
