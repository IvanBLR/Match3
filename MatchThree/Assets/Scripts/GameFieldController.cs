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
    public Action<int> ScoreChanged;
    public GameFieldController()
    {
        _someTechnicalCalculations = new SomeTechnicalCalculations(this);
    }

    public int TotalScore => _totalScore;
    public SomeTechnicalCalculations SomeTechnicalCalculations
    {
        get { return _someTechnicalCalculations; }
    }

    public List<ItemSettingsProvider> AllVariantsItemsCollections
    {
        set { _allVariantsItemsCollections = value; }
        get { return _allVariantsItemsCollections; }
    }

    public List<ItemScriptableObject> ActualItemsList
    {
        set { _actualItemsList = value; }
        get { return _actualItemsList; }
    }

    public int[] ActualNameID
    {
        set { _actualNameID = value; }
        get { return _actualNameID; }
    }

    public SpriteRenderer[,] SpriteRenderersMatrix
    {
        set { _spriteRenderersMatrix = value; }
        get { return _spriteRenderersMatrix; }
    }

    public Item[,] ItemsMatrix
    {
        set { _itemsMatrix = value; }
        get { return _itemsMatrix; }
    }

    public int ItemCollectionsNumber
    {
        set { _itemCollectionsNumber = value; }
        get { return _itemCollectionsNumber; }
    }

    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private List<ItemSettingsProvider> _allVariantsItemsCollections;
    [SerializeField] private Transform _parent;

    private List<ItemScriptableObject> _actualItemsList;
    private int[] _actualNameID = new int[5];
    private SpriteRenderer[,] _spriteRenderersMatrix;
    private Item[,] _itemsMatrix;

    private int _itemCollectionsNumber;
    private int _totalScore;
    
    private readonly SomeTechnicalCalculations _someTechnicalCalculations;

    public void InitializeActualItemsList() // done
    {
        _itemCollectionsNumber = PlayerPrefs.GetInt(PlayerSettingsConst.PLAYING_SET);
        _actualItemsList = SomeTechnicalCalculations.GetActualItemsList();
        _actualNameID = SomeTechnicalCalculations.GetActualNameID();

        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        _parent.position = _grid.transform.position;
        if (PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN) == 5)
        {
            _grid.transform.position += new Vector3(0, 1.5f, 0);
            _parent.position += new Vector3(0, 1.5f, 0);
        }

        _spriteRenderersMatrix = new SpriteRenderer[row, column];
        _itemsMatrix = new Item[row, column];

        InitializationActualItemsCompleted?.Invoke();
    }

    public void FillGameBoardWithTilesFirstTimeOnly() // done 
    {
        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                var position = _grid.CellToLocal(new Vector3Int(i, j));
                var tile = Instantiate(_itemPrefab, _parent);
                tile.transform.localPosition = position + _grid.cellGap;

                _spriteRenderersMatrix[i, j] = tile.GetComponent<SpriteRenderer>();
                _itemsMatrix[i, j] = tile.GetComponent<Item>();

                int indexCurrentSettings = SomeTechnicalCalculations.GetCurrentSettingsIndex(i, j);
                var currentSettings = _actualItemsList[indexCurrentSettings];

                _itemsMatrix[i, j].ItemSettings = currentSettings;
                _spriteRenderersMatrix[i, j].sprite = currentSettings.Icon;

                var targetScale = tile.transform.localScale;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(targetScale, 0.3f).SetDelay(PlayerSettingsConst.DELAY);
            }
        }

        FilledGameBoard?.Invoke();
    }

    public void Swap(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction) // done
    {
        HashSet<Vector3Int> match3 = new();

        if (direction == new Vector2(1, 0)) // to right
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = SomeTechnicalCalculations.GetMatchThreeOrMore();

            if (match3.Count > 0)
                StartCoroutine(RestoreGameField(match3));
            else
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
        }

        if (direction == new Vector2(-1, 0)) // to left
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = SomeTechnicalCalculations.GetMatchThreeOrMore();

            if (match3.Count > 0)
                StartCoroutine(RestoreGameField(match3));
            else
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
        }

        if (direction == new Vector2(0, 1)) // to up
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = SomeTechnicalCalculations.GetMatchThreeOrMore();

            if (match3.Count > 0)
                StartCoroutine(RestoreGameField(match3));
            else
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
        }

        if (direction == new Vector2(0, -1)) // to down
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = SomeTechnicalCalculations.GetMatchThreeOrMore();

            if (match3.Count > 0)
                StartCoroutine(RestoreGameField(match3));
            else
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
        }
    }

    private void SwapAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction)
    {
        if (hitDown.collider != null && hitUp.collider != null)
        {
            RectTransform firstTile = (RectTransform)hitDown.transform; // maybe easy Transform?
            RectTransform secondTile = (RectTransform)hitUp.transform; // maybe easy Transform?

            firstTile.DOMove(secondTile.position, 0.3f).SetDelay(PlayerSettingsConst.DELAY);
            secondTile.DOMove(firstTile.position, 0.3f).SetDelay(PlayerSettingsConst.DELAY);

            var tempItem = _itemsMatrix[x, y];
            _itemsMatrix[x, y] = _itemsMatrix[x + direction.x, y + direction.y]; // íå íàäî íàæèìàòü
            _itemsMatrix[x + direction.x, y + direction.y] = tempItem; // alt + enter

            var tempSpriteRenderer = _spriteRenderersMatrix[x, y];
            _spriteRenderersMatrix[x, y] = _spriteRenderersMatrix[x + direction.x, y + direction.y]; // íå íàäî íàæèìàòü
            _spriteRenderersMatrix[x + direction.x, y + direction.y] = tempSpriteRenderer; // alt + enter
        }
    }

    private void DeleteMatchThree(HashSet<Vector3Int> setForDelete) // done
    {
        GotMatchTree.Invoke();
        foreach (var point in setForDelete)
        {
            int x = point.x;
            int y = point.y;

            var localScale = _spriteRenderersMatrix[x, y].transform.localScale;
            _spriteRenderersMatrix[x, y].transform.DOScale(Vector3.zero, 0.3f)
                .OnComplete(() =>
                {
                    _spriteRenderersMatrix[x, y].gameObject.SetActive(false);
                    _spriteRenderersMatrix[x, y].transform.localScale = localScale;
                });
        }
    }

    private void FallDownItems() //   done
    {
        var emptyCoordinates = SomeTechnicalCalculations.GetAllEmptyCoordinates();
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

                if (itemCoordinatesForFalling[i].Count > 0)
                {
                    for (int n = 0; n < itemCoordinatesForFalling[i].Count; n++)
                    {
                        FallingAnimation(itemCoordinatesForFalling, i, n, sortedCoordinates);
                    }
                }
            }
        }
    }

    private void FallingAnimation(List<List<Vector3Int>> coordinatesForFalling, int i, int j,
        List<Vector3Int> sortedCoordinates) // done
    {
        var firstPoint = coordinatesForFalling[i][j];
        var secondPoint = sortedCoordinates[j];

        RectTransform tileForFalling = (RectTransform)_spriteRenderersMatrix[firstPoint.x, firstPoint.y].transform;
        RectTransform emptyTile = (RectTransform)_spriteRenderersMatrix[secondPoint.x, secondPoint.y].transform;

        tileForFalling.DOMove(_grid.CellToWorld(secondPoint), 0.5f);
        emptyTile.DOMove(_grid.CellToWorld(firstPoint), 0.5f);

        var tempSpriteRenderer = _spriteRenderersMatrix[firstPoint.x, firstPoint.y];
        _spriteRenderersMatrix[firstPoint.x, firstPoint.y] = _spriteRenderersMatrix[secondPoint.x, secondPoint.y];
        _spriteRenderersMatrix[secondPoint.x, secondPoint.y] = tempSpriteRenderer;

        var tempItem = _itemsMatrix[firstPoint.x, firstPoint.y];
        _itemsMatrix[firstPoint.x, firstPoint.y] = _itemsMatrix[secondPoint.x, secondPoint.y];
        _itemsMatrix[secondPoint.x, secondPoint.y] = tempItem;
    }

    private void FillEmptyCellsAfterMatch3() // done
    {
        var emptyCells = SomeTechnicalCalculations.GetAllEmptyCoordinates();
        HashSet<int> ID = new();

        for (int i = 0; i < emptyCells.Count; i++)
        {
            for (int j = 0; j < emptyCells[i].Count; j++)
            {
                int x = emptyCells[i][j].x;
                int y = emptyCells[i][j].y;

                int currentSettingsIndex = SomeTechnicalCalculations.GetCurrentSettingsIndex(x, y);
                _spriteRenderersMatrix[x, y].gameObject.SetActive(true);

                _itemsMatrix[x, y].ItemSettings = _actualItemsList[currentSettingsIndex];
                _spriteRenderersMatrix[x, y].sprite = _itemsMatrix[x, y].ItemSettings.Icon;

                var finishScale = _spriteRenderersMatrix[x, y].transform.localScale;
                _spriteRenderersMatrix[x, y].transform.localScale = Vector3.zero;
                _spriteRenderersMatrix[x, y].transform.DOScale(finishScale, 0.3f);
            }
        }
    }

    public void UseBomb(int x, int y)
    {
        List<Vector3Int> bomb = _someTechnicalCalculations.GetBombsCoordinates(x, y);
        HashSet<Vector3Int> bombCoordinates = new();
        
        _totalScore -= 3 * bomb.Count;
        if (_totalScore < 0)
            _totalScore = 0;
        
        for (int i = 0; i < bomb.Count; i++)
        {
            bombCoordinates.Add(bomb[i]);
        }
        BombUsed?.Invoke();
        StartCoroutine(RestoreGameField(bombCoordinates));
    }

    private IEnumerator RestoreGameField(HashSet<Vector3Int> match3)
    {
        yield return new WaitForSeconds(0.3f + PlayerSettingsConst.DELAY);
        DeleteMatchThree(match3);
        yield return new WaitForSeconds(0.3f + PlayerSettingsConst.DELAY);
        FallDownItems();
        yield return new WaitForSeconds(0.5f + PlayerSettingsConst.DELAY);

        _totalScore += match3.Count;
        ScoreChanged?.Invoke(_totalScore);
        
        var newMatch3 = SomeTechnicalCalculations.GetMatchThreeOrMore();
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
        yield return new WaitForSeconds(PlayerSettingsConst.DELAY + 0.3f); // is it right?
        SwapAnimation(hitDown, hitUp, x, y, direction);
    }
}