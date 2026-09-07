using UnityEngine;

public class CommentaryUI : MonoBehaviour
{
    [SerializeField] private RectTransform Transform_Content;
    [SerializeField] private CommentaryRowUI Prefab_Row;

    private void Awake()
    {
        
    }

    private void Update()
    {
        
    }

    public void ClearCommentary()
    {

    }

    private void InitializePool()
    {

    }

    private CommentaryRowUI CreateRow()
    {
        CommentaryRowUI row = Instantiate(Prefab_Row, Transform_Content);

        return row;
    }

    private void TakeFromPool()
    {

    }

    private void ReturnToPool()
    {

    }

    private void DestroyRow()
    {

    }

    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
