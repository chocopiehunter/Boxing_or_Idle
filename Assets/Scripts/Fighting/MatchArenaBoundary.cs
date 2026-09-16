using UnityEngine;

public class MatchArenaBoundary : MonoBehaviour
{
    private const int GizmoSegmentCount = 64;

    [SerializeField] private Vector2 CenterOffset = new Vector2(0f, -0.13f);
    [SerializeField] private float HorizontalRadius = 0.9f;
    [SerializeField] private float VerticalRadius = 0.625f;

    public bool TryGetBoundary(out Vector2 areaCenter, out Vector2 areaRadii)
    {
        areaCenter = Vector2.zero;
        areaRadii = Vector2.zero;

        if (transform.parent == null)
        {
            return false;
        }

        if (HorizontalRadius <= 0f || VerticalRadius <= 0f)
        {
            return false;
        }

        if (Mathf.Approximately(transform.localScale.x, 0f) || Mathf.Approximately(transform.localScale.y, 0f))
        {
            return false;
        }

        if (Quaternion.Angle(transform.localRotation, Quaternion.identity) > 0.01f)
        {
            return false;
        }

        Vector2 scaledCenterOffset = new Vector2(CenterOffset.x * transform.localScale.x, CenterOffset.y * transform.localScale.y);

        areaCenter = (Vector2)transform.localPosition + scaledCenterOffset;

        areaRadii = new Vector2(Mathf.Abs(HorizontalRadius * transform.localScale.x), Mathf.Abs(VerticalRadius * transform.localScale.y));

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (HorizontalRadius <= 0f || VerticalRadius <= 0f)
        {
            return;
        }

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.cyan;

        Vector3 previousPoint = GetEllipsePoint(0f);

        for (int index = 1; index <= GizmoSegmentCount; index++)
        {
            float angle = Mathf.PI * 2f * index / GizmoSegmentCount;
            Vector3 currentPoint = GetEllipsePoint(angle);

            Gizmos.DrawLine(previousPoint, currentPoint);

            previousPoint = currentPoint;
        }

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }

    private Vector3 GetEllipsePoint(float angle)
    {
        float x = CenterOffset.x + Mathf.Cos(angle) * HorizontalRadius;
        float y = CenterOffset.y + Mathf.Sin(angle) * VerticalRadius;

        return new Vector3(x, y, 0f);
    }
}
