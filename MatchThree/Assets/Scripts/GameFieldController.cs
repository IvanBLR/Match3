using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameFieldController : MonoBehaviour
{
    public Action InitializationActualItemsCompleted;
    public Action FilledGameBoard;
    public Action GotMatchTree;
    public Action WrongMatch3;
    public Action BombUsed;
    public Action SimpleBombUsed;
    public Action<int> ScoreChanged;

    public GameFieldController()
    {
        _someTechnicalCalculations = new SomeTechnicalCalculations(this);
    }

    public List<ItemSettingsProvider> AllVariantsItemsCollections
    {
        get => _allVariantsItemsCollections;
    }

    public List<ItemScriptableObject> ActualItemsList { get; private set; }
    public int[] ActualNameID { get; private set; } = new int[5];
    public SpriteRenderer[,] SpriteRenderersMatrix { get; private set; }
    public ItemSettings[,] ItemsMatrix { get; private set; }
    public int ItemCollectionsNumber { get; private set; }

    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private List<ItemSettingsProvider> _allVariantsItemsCollections;
    [SerializeField] private Transform _parent;

    private int _totalScore;

    private readonly SomeTechnicalCalculations _someTechnicalCalculations;

    
    public void UseSimpleBomb(int x, int y)
    {
        HashSet<Vector3Int> bombCoordinates = new();
        bombCoordinates.Add(new Vector3Int(x, y, 0));

        _totalScore -= 2;
        if (_totalScore < 0)
            _totalScore = -1;
        
        SimpleBombUsed?.Invoke();
        StartCoroutine(RestoreGameField(bombCoordinates));
    }

    public void UseBomb(int x, int y)
    {
        List<Vector3Int> bomb = _someTechnicalCalculations.GetBombsCoordinates(x, y);
        HashSet<Vector3Int> bombCoordinates = new();

        _totalScore -= 3 * bomb.Count;
        if (_totalScore < 0)
            _totalScore = -4;

        for (int i = 0; i < bomb.Count; i++)
        {
            bombCoordinates.Add(bomb[i]);
        }

        BombUsed?.Invoke();
        StartCoroutine(RestoreGameField(bombCoordinates)); 
    }
    public void InitializeActualItemsList()
    {
        _someTechnicalCalculations.InitializeRowAndColumn();

        ItemCollectionsNumber = PlayerPrefs.GetInt(PlayingSettingsConstant.PLAYING_SET);
        ActualItemsList = _someTechnicalCalculations.GetActualItemsList();
        ActualNameID = _someTechnicalCalculations.GetActualNameID();

        int row = PlayerPrefs.GetInt(PlayingSettingsConstant.GAME_FIELD_ROW);
        int column = PlayerPrefs.GetInt(PlayingSettingsConstant.GAME_FIELD_COLUMN);
        _parent.position = _grid.transform.position;
        if (PlayerPrefs.GetInt(PlayingSettingsConstant.GAME_FIELD_COLUMN) == 5)
        {
            _grid.transform.position += new Vector3(0, 1.5f, 0);
            _parent.position += new Vector3(0, 1.5f, 0);
        }

        SpriteRenderersMatrix = new SpriteRenderer[row, column];
        ItemsMatrix = new ItemSettings[row, column];

        InitializationActualItemsCompleted?.Invoke();
    }

    public void FillGameBoardWithTilesFirstTimeOnly()
    {
        int row = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_ROW);
        int column = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var position = _grid.CellToLocal(new Vector3Int(i, j));
                var tile = Instantiate(_itemPrefab, _parent);
                tile.transform.localPosition = position + _grid.cellGap;

                SpriteRenderersMatrix[i, j] = tile.GetComponent<SpriteRenderer>();
                ItemsMatrix[i, j] = tile.GetComponent<ItemSettings>();

                int indexCurrentSettings = _someTechnicalCalculations.GetCurrentSettingsIndex(i, j);
                var currentSettings = ActualItemsList[indexCurrentSettings];

                ItemsMatrix[i, j].ItemsSettings = currentSettings;
                SpriteRenderersMatrix[i, j].sprite = currentSettings.Icon;

                var targetScale = tile.transform.localScale;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(targetScale, 0.3f).SetDelay(PlayingSettingsConstant.DELAY);
            }
        }

        FilledGameBoard?.Invoke();
    }

    public void Swap(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction) // done
    {
        if (direction == new Vector2(1, 0)) // to right
            StartSwapAnimation(hitDown, hitUp, x, y, direction);

        if (direction == new Vector2(-1, 0)) // to left
            StartSwapAnimation(hitDown, hitUp, x, y, direction);

        if (direction == new Vector2(0, 1)) // to up
            StartSwapAnimation(hitDown, hitUp, x, y, direction);

        if (direction == new Vector2(0, -1)) // to down
            StartSwapAnimation(hitDown, hitUp, x, y, direction);
    }

    private void StartSwapAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction)
    {
        SwapAnimation(hitDown, hitUp, x, y, direction);

        HashSet<Vector3Int> match3 = _someTechnicalCalculations.GetMatchThreeOrMore();

        if (match3.Count > 0)
            StartCoroutine(RestoreGameField(match3));
        else
            StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
    }

    private void SwapAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction)
    {
        if (hitDown.collider != null && hitUp.collider != null)
        {
            RectTransform firstTile = (RectTransform)hitDown.transform;
            RectTransform secondTile = (RectTransform)hitUp.transform;

            firstTile.DOMove(secondTile.position, 0.3f).SetDelay(PlayingSettingsConstant.DELAY);
            secondTile.DOMove(firstTile.position, 0.3f).SetDelay(PlayingSettingsConstant.DELAY);

            var tempItem = ItemsMatrix[x, y];
            ItemsMatrix[x, y] = ItemsMatrix[x + direction.x, y + direction.y]; // íå íàäî íàæèìàòü
            ItemsMatrix[x + direction.x, y + direction.y] = tempItem; // alt + enter

            var tempSpriteRenderer = SpriteRenderersMatrix[x, y];
            SpriteRenderersMatrix[x, y] = SpriteRenderersMatrix[x + direction.x, y + direction.y]; // íå íàäî íàæèìàòü
            SpriteRenderersMatrix[x + direction.x, y + direction.y] = tempSpriteRenderer; // alt + enter
        }
    }

    private void DeleteMatchThree(HashSet<Vector3Int> setForDelete)
    {
        GotMatchTree.Invoke();
        foreach (var point in setForDelete)
        {
            int x = point.x;
            int y = point.y;

            var localScale = SpriteRenderersMatrix[x, y].transform.localScale;
            SpriteRenderersMatrix[x, y].transform.DOScale(Vector3.zero, 0.3f)
                .OnComplete(() =>
                {
                    SpriteRenderersMatrix[x, y].gameObject.SetActive(false);
                    SpriteRenderersMatrix[x, y].transform.localScale = localScale;
                });
        }
    }

    private void FallDownItems()
    {
        var emptyCoordinates = _someTechnicalCalculations.GetAllEmptyCoordinates();
        var itemCoordinatesForFalling = _someTechnicalCalculations.GetItemsCoordinatesForFalling(emptyCoordinates);

        for (int i = 0; i < emptyCoordinates.Count; i++)
        {
            if (emptyCoordinates[i].Count != 0)
            {
                List<Vector3Int> newEmptyCoordinates = emptyCoordinates[i];

                for (int k = 0; k < itemCoordinatesForFalling[i].Count; k++)
                {
                    newEmptyCoordinates.Add(itemCoordinatesForFalling[i][k]);
                }

                var sortedCoordinates = newEmptyCoordinates.OrderBy(point => point.y).ToList();

                if (itemCoordinatesForFalling[i].Count <= 0) continue;
                for (int n = 0; n < itemCoordinatesForFalling[i].Count; n++)
                {
                    FallingAnimation(itemCoordinatesForFalling, i, n, sortedCoordinates);
                }
            }
        }
    }

    private void FallingAnimation(List<List<Vector3Int>> coordinatesForFalling, int i, int j,
        List<Vector3Int> sortedCoordinates)
    {
        var firstPoint = coordinatesForFalling[i][j];
        var secondPoint = sortedCoordinates[j];

        RectTransform tileForFalling = (RectTransform)SpriteRenderersMatrix[firstPoint.x, firstPoint.y].transform;
        RectTransform emptyTile = (RectTransform)SpriteRenderersMatrix[secondPoint.x, secondPoint.y].transform;

        tileForFalling.DOMove(_grid.CellToWorld(secondPoint), 0.5f);
        emptyTile.DOMove(_grid.CellToWorld(firstPoint), 0.5f);

        var tempSpriteRenderer = SpriteRenderersMatrix[firstPoint.x, firstPoint.y]; // íå íàäî íàæèìàòü alt+enter
        SpriteRenderersMatrix[firstPoint.x, firstPoint.y] = SpriteRenderersMatrix[secondPoint.x, secondPoint.y];
        SpriteRenderersMatrix[secondPoint.x, secondPoint.y] = tempSpriteRenderer;

        var tempItem = ItemsMatrix[firstPoint.x, firstPoint.y]; // íå íàäî íàæèìàòü alt+enter
        ItemsMatrix[firstPoint.x, firstPoint.y] = ItemsMatrix[secondPoint.x, secondPoint.y];
        ItemsMatrix[secondPoint.x, secondPoint.y] = tempItem;
    }

    private void FillEmptyCellsAfterMatch3()
    {
        var emptyCells = _someTechnicalCalculations.GetAllEmptyCoordinates();
        HashSet<int> ID = new();

        for (int i = 0; i < emptyCells.Count; i++)
        {
            for (int j = 0; j < emptyCells[i].Count; j++)
            {
                int x = emptyCells[i][j].x;
                int y = emptyCells[i][j].y;

                int currentSettingsIndex = _someTechnicalCalculations.GetCurrentSettingsIndex(x, y);
                SpriteRenderersMatrix[x, y].gameObject.SetActive(true);

                ItemsMatrix[x, y].ItemsSettings = ActualItemsList[currentSettingsIndex];
                SpriteRenderersMatrix[x, y].sprite = ItemsMatrix[x, y].ItemsSettings.Icon;

                var finishScale = SpriteRenderersMatrix[x, y].transform.localScale;
                SpriteRenderersMatrix[x, y].transform.localScale = Vector3.zero;
                SpriteRenderersMatrix[x, y].transform.DOScale(finishScale, 0.3f);
            }
        }
    }

    private IEnumerator RestoreGameField(HashSet<Vector3Int> match3)
    {
        yield return new WaitForSeconds(0.3f + PlayingSettingsConstant.DELAY);
        DeleteMatchThree(match3);
        yield return new WaitForSeconds(0.3f + PlayingSettingsConstant.DELAY);
        FallDownItems();
        yield return new WaitForSeconds(0.5f + PlayingSettingsConstant.DELAY);

        _totalScore += match3.Count;
        ScoreChanged?.Invoke(_totalScore);

        var newMatch3 = _someTechnicalCalculations.GetMatchThreeOrMore();
        if (newMatch3.Count > 2)
        {
            StartCoroutine(RestoreGameField(match3));
        }

        FillEmptyCellsAfterMatch3();
    }

    private IEnumerator SwapBackAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction)
    {
        WrongMatch3?.Invoke();
        yield return null; // is it right?
        yield return new WaitForSeconds(PlayingSettingsConstant.DELAY + 0.3f); // is it right?
        SwapAnimation(hitDown, hitUp, x, y, direction);
    }
}