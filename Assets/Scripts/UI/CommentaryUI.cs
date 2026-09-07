using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CommentaryUI : MonoBehaviour
{
    private sealed class ActiveCommentaryRow
    {
        public CommentaryRowUI Row;
        public float RemainingHoldTime;
        public bool IsFading;

        public ActiveCommentaryRow(CommentaryRowUI row, float holdDuration)
        {
            Row = row;

            RemainingHoldTime = holdDuration;

            IsFading = false;
        }
    }

    [SerializeField] private RectTransform Transform_Content;
    [SerializeField] private CommentaryRowUI Prefab_Row;

    [SerializeField] private int InitialPoolSize = 6;
    [SerializeField] private int MaxPoolSize = 12;
    [SerializeField] private float HoldDuration = 2.5f;
    [SerializeField] private float RowSpacing = 5f;

    private ObjectPool<CommentaryRowUI> _rowPool;
    private readonly List<ActiveCommentaryRow> _activeRows = new List<ActiveCommentaryRow>();

    private void Awake()
    {
        InitializePool();
        CreateInitialPoolRows();
    }

    private void Update()
    {
        UpdateRowLifetimes();
    }

    public void ShowCommentary(string message)
    {
        if (string.IsNullOrEmpty(message) == true)
        {
            return;
        }

        CommentaryRowUI row = _rowPool.Get();

        row.transform.SetAsLastSibling();

        row.Setup(message);

        row.SetPositionImmediately(0f);

        ActiveCommentaryRow activeRow = new ActiveCommentaryRow(row, HoldDuration);

        _activeRows.Insert(0, activeRow);

        RefreshRowPositions();
    }

    public void ClearCommentary()
    {
        ReleaseAllRows();
    }

    private void InitializePool()
    {
        int initialPoolSize = Mathf.Max(1, InitialPoolSize);

        int maxPoolSize = Mathf.Max(initialPoolSize, MaxPoolSize);

        _rowPool = new ObjectPool<CommentaryRowUI>(CreateRow, TakeRowFromPool, ReturnRowToPool, DestroyRow, true, initialPoolSize, maxPoolSize);
    }

    private void CreateInitialPoolRows()
    {
        List<CommentaryRowUI> initialRows = new List<CommentaryRowUI>();

        int poolSize = Mathf.Max(1, InitialPoolSize);

        for (int i = 0; i < poolSize; i++)
        {
            CommentaryRowUI row = _rowPool.Get();

            initialRows.Add(row);
        }

        for (int i = 0; i < initialRows.Count; i++)
        {
            _rowPool.Release(initialRows[i]);
        }
    }

    private CommentaryRowUI CreateRow()
    {
        CommentaryRowUI row = Instantiate(Prefab_Row, Transform_Content);

        row.gameObject.SetActive(false);

        return row;
    }

    private void TakeRowFromPool(CommentaryRowUI row)
    {
        row.gameObject.SetActive(true);
    }

    private void ReturnRowToPool(CommentaryRowUI row)
    {
        row.ResetRow();

        row.gameObject.SetActive(false);
    }

    private void DestroyRow(CommentaryRowUI row)
    {
        if (row == null)
        {
            return;
        }

        Destroy(row.gameObject);
    }

    private void UpdateRowLifetimes()
    {
        float deltaTime = Time.unscaledDeltaTime;

        for (int i = _activeRows.Count - 1; i >= 0; i--)
        {
            ActiveCommentaryRow activeRow = _activeRows[i];

            if (activeRow.IsFading == true)
            {
                continue;
            }

            activeRow.RemainingHoldTime -= deltaTime;

            if (activeRow.RemainingHoldTime > 0f)
            {
                continue;
            }

            activeRow.IsFading = true;

            activeRow.Row.FadeOut(OnRowFadeCompleted);
        }
    }

    private void RefreshRowPositions()
    {
        float nextY = 0f;

        for (int i = 0; i < _activeRows.Count; i++)
        {
            CommentaryRowUI row = _activeRows[i].Row;

            row.MoveTo(-nextY);

            nextY += row.Height + RowSpacing;
        }
    }

    private void OnRowFadeCompleted(CommentaryRowUI completedRow)
    {
        for (int i = _activeRows.Count - 1; i >= 0; i--)
        {
            if (_activeRows[i].Row != completedRow)
            {
                continue;
            }

            _activeRows.RemoveAt(i);

            _rowPool.Release(completedRow);

            RefreshRowPositions();

            return;
        }
    }

    private void ReleaseAllRows()
    {
        if (_rowPool == null)
        {
            return;
        }

        for (int i = _activeRows.Count - 1; i >= 0; i--)
        {
            _rowPool.Release(_activeRows[i].Row);
        }

        _activeRows.Clear();
    }

    private void OnDisable()
    {
        ReleaseAllRows();
    }

    private void OnDestroy()
    {
        ReleaseAllRows();

        if (_rowPool != null)
        {
            _rowPool.Clear();
        }
    }
}
