using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = System.Random;

public class GameFieldController : MonoBehaviour
{
    public Action InitializationActualItemsCompleted;
    public Action FilledGameBoard;
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _itemPrefab;

    [SerializeField] private List<ItemSettingsProvider> _allVariantsItemsCollections;
    [SerializeField] private Transform _parent;

    private List<ItemScriptableObject> _actualItemsList;
    private int[] _actualNameID = new int[5];
    private SpriteRenderer[,] _spriteRenderersMatrix;
    private Item[,] _itemsMatrix;

    private readonly float _delay = 0.1f;
    private int _itemCollectionsNumber;
    private int _row;
    private int _column;

    private void Start()
    {
        // _gameFieldSettings.GameSettingsAccepted += InitializeActualItemsListClassFields;      € их подписал ->
        // _gameFieldSettings.GameSettingsAccepted += FillGameBoardWithTiles;                    в √еймћенеджере
    }

    public void InitializeActualItemsListClassFields() // done
    {
        _actualItemsList = GetActualItemsList();
        _actualNameID = GetActualNameID();

        _row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        _column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        _parent.position = _grid.transform.position;
        _spriteRenderersMatrix = new SpriteRenderer[_row, _column];
        _itemsMatrix = new Item[_row, _column];

        InitializationActualItemsCompleted?.Invoke();
    }

    public void FillGameBoardWithTilesFirstTimeOnly() // done 
    {
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++)
            {
                var position = _grid.CellToLocal(new Vector3Int(i, j));
                var tile = Instantiate(_itemPrefab, _parent);
                tile.transform.localPosition = position + _grid.cellGap;

                _spriteRenderersMatrix[i, j] = tile.GetComponent<SpriteRenderer>();
                _itemsMatrix[i, j] = tile.GetComponent<Item>();

                int indexCurrentSettings = GetCurrentSettingsIndex(i, j);
                var currentSettings = _actualItemsList[indexCurrentSettings];

                _itemsMatrix[i, j].ItemSettings = currentSettings;
                _spriteRenderersMatrix[i, j].sprite = currentSettings.Icon;

                var targetScale = tile.transform.localScale;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(targetScale, 0.3f).SetDelay(_delay);
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
            match3 = GetMatchThreeOrMore();

            if (match3.Count > 0)
            {
                StartCoroutine(RestoreGameField(match3));
            }
            else
            {
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
            }
        }

        if (direction == new Vector2(-1, 0)) // to left
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = GetMatchThreeOrMore();

            if (match3.Count > 0)
            {
                StartCoroutine(RestoreGameField(match3));
            }
            else
            {
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
            }
        }

        if (direction == new Vector2(0, 1)) // to up
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = GetMatchThreeOrMore();

            if (match3.Count > 0)
            {
                StartCoroutine(RestoreGameField(match3));
            }
            else
            {
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
            }
        }

        if (direction == new Vector2(0, -1)) // to down
        {
            SwapAnimation(hitDown, hitUp, x, y, direction);
            match3 = GetMatchThreeOrMore();

            if (match3.Count > 0)
            {
                StartCoroutine(RestoreGameField(match3));
            }
            else
            {
                StartCoroutine(SwapBackAnimation(hitDown, hitUp, x, y, direction));
            }
        }
    }

    private void SwapAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y,
        Vector2Int direction) // done
    {
        if (hitDown.collider != null && hitUp.collider != null)
        {
            RectTransform firstTile = (RectTransform)hitDown.transform; // maybe easy Transform?
            RectTransform secondTile = (RectTransform)hitUp.transform; // maybe easy Transform?

            firstTile.DOMove(secondTile.position, 0.3f).SetDelay(_delay);
            secondTile.DOMove(firstTile.position, 0.3f).SetDelay(_delay);

            var tempItem = _itemsMatrix[x, y];
            _itemsMatrix[x, y] = _itemsMatrix[x + direction.x, y + direction.y]; // не надо нажимать
            _itemsMatrix[x + direction.x, y + direction.y] = tempItem; // alt + enter

            var tempSpriteRenderer = _spriteRenderersMatrix[x, y];
            _spriteRenderersMatrix[x, y] = _spriteRenderersMatrix[x + direction.x, y + direction.y]; // не надо нажимать
            _spriteRenderersMatrix[x + direction.x, y + direction.y] = tempSpriteRenderer; // alt + enter
        }
    }

    /// <summary>
    /// return choose player's set with 5 random element.
    /// After -> fill the gameField with the elements
    /// </summary>
    private List<ItemScriptableObject> GetActualItemsList() //done
    {
        _itemCollectionsNumber = PlayerPrefs.GetInt(PlayerSettingsConst.PLAYING_SET);
        List<ItemScriptableObject> actualItemsListForReturn = new();
        Random random = new Random();
        var actualItemSettingsProvider = _allVariantsItemsCollections.ElementAt(_itemCollectionsNumber);
        var actualItemsCollection = actualItemSettingsProvider.ItemsList;
        int[] unicIndexes = Enumerable.Range(0, actualItemsCollection.Count)
            .OrderBy(x => random.Next())
            .Take(5)
            .ToArray();
        for (int i = 0; i < 5; i++)
        {
            int index = unicIndexes[i];
            actualItemsListForReturn.Add(actualItemsCollection[index]);
        }

        return actualItemsListForReturn;
    }

    private int[] GetActualNameID() // done. ¬спомогательный метод
    {
        int[] actualNameID = new int[5];
        for (int i = 0; i < 5; i++)
        {
            actualNameID[i] = _actualItemsList[i].NameID;
        }

        return actualNameID;
    }

    /// <summary>
    ///  ћетод помогает заполнить пустые €чейки, вернув нужный IndexSettings, kоторый гарантирует, что нигде не будет match-3
    /// /// </summary>
    private int GetCurrentSettingsIndex(int x, int y) // done.
    {
        Random random = new Random();
        HashSet<int> checkList = new();
        int currentID = 0;
        int upID = 0;
        int downID = 0;
        int leftID = 0;
        int rightID = 0;
        if (y + 1 < _column && _itemsMatrix[x, y + 1] is not null)
        {
            upID = _itemsMatrix[x, y + 1].ItemSettings.NameID;
        }

        if (y - 1 >= 0) // можно не вводить доп.проверок на null
        {
            downID = _itemsMatrix[x, y - 1].ItemSettings.NameID;
        }

        if (x + 1 < _row && _itemsMatrix[x + 1, y] != null)
        {
            rightID = _itemsMatrix[x + 1, y].ItemSettings.NameID;
        }

        if (x - 1 >= 0) // можно не вводить доп.проверок на null
        {
            leftID = _itemsMatrix[x - 1, y].ItemSettings.NameID;
        }

        checkList.Add(upID);
        checkList.Add(downID);
        checkList.Add(leftID);
        checkList.Add(rightID);

        while (true)
        {
            int index = random.Next(0, 5);
            currentID = _actualNameID[index];
            if (!checkList.Contains(currentID)) return index;
        }
    }

    private void DeleteMatchThree(HashSet<Vector3Int> setForDelete) // done
    {
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

    private void FallDownItems() //                                         done
    {
        var emptyCoordinates = GetAllEmptyCoordinates();
        var itemCoordinatesForFalling = GetItemsCoordinatesForFalling(emptyCoordinates);

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
        var emptyCells = GetAllEmptyCoordinates();
        HashSet<int> ID = new();
        Random random = new Random();

        for (int i = 0; i < emptyCells.Count; i++)
        {
            for (int j = 0; j < emptyCells[i].Count; j++)
            {
                int x = emptyCells[i][j].x;
                int y = emptyCells[i][j].y;

                int currentSettingsIndex = GetCurrentSettingsIndex(x, y);
                _spriteRenderersMatrix[x, y].gameObject.SetActive(true);

                _itemsMatrix[x, y].ItemSettings = _actualItemsList[currentSettingsIndex];
                _spriteRenderersMatrix[x, y].sprite = _itemsMatrix[x, y].ItemSettings.Icon;
            }
        }
    }

    private List<List<Vector3Int>> GetItemsCoordinatesForFalling(List<List<Vector3Int>> allEmptyCoordinates) // done
    {
        List<List<Vector3Int>> finalReturnList = new();
        Queue<List<Vector3Int>> globalQueueWithListCoordinates = new();

        for (int i = 0; i < allEmptyCoordinates.Count; i++)
        {
            globalQueueWithListCoordinates.Enqueue(allEmptyCoordinates[i]); // Count == _row
        }

        List<Vector3Int> currentListWithEmptyCoordinates = new();

        while (globalQueueWithListCoordinates.Count > 0)
        {
            currentListWithEmptyCoordinates = globalQueueWithListCoordinates.Dequeue();
            List<Vector3Int> listForReturnList = new();
            Queue<Vector3Int> currentQueue = new();

            for (int j = 0; j < currentListWithEmptyCoordinates.Count; j++)
            {
                currentQueue.Enqueue(currentListWithEmptyCoordinates[j]);
            }

            while (currentQueue.Count > 0)
            {
                Vector3Int currentPoint = currentQueue.Dequeue();
                Vector3Int nextPoint;
                while (currentQueue.Count > 0)
                {
                    nextPoint = currentQueue.Dequeue();
                    if (nextPoint.y - currentPoint.y == 1)
                    {
                        currentPoint = nextPoint;
                    }
                    else
                    {
                        int start = currentPoint.y + 1;
                        int end = nextPoint.y - 1;
                        for (int index = start; index <= end; index++)
                        {
                            listForReturnList.Add(new Vector3Int(currentPoint.x, index));
                        }

                        currentPoint = nextPoint;
                    }
                }

                for (int y = currentPoint.y + 1; y < _column; y++)
                {
                    listForReturnList.Add(new Vector3Int(currentPoint.x, y));
                }
            }

            finalReturnList.Add(listForReturnList);
        }

        return finalReturnList;
    }

    private List<List<Vector3Int>> GetAllEmptyCoordinates() // done. Use it 
    {
        List<List<Vector3Int>> returnList = new();

        for (int i = 0; i < _row; i++)
        {
            var currentList = GetEmptyCoordinatesInCurrentColumnInGridNotation(i);
            returnList.Add(currentList);
        }

        return returnList;
    }

    /// <summary>
    /// вспомогательный метод
    /// </summary>
    private List<Vector3Int> GetEmptyCoordinatesInCurrentColumnInGridNotation(int rowIndex) // done. Use it 
    {
        List<Vector3Int> arrayForReturn = new();
        for (int j = 0; j < _column; j++)
        {
            var currentSpriteRenderer = _spriteRenderersMatrix[rowIndex, j];
            if (!currentSpriteRenderer.gameObject.activeSelf) //currentSpriteRenderer.enabled == false)
            {
                var emptyPosition = new Vector3Int(rowIndex, j);
                arrayForReturn.Add(emptyPosition);
            }
        }

        return arrayForReturn;
    }

    private HashSet<Vector3Int> GetMatchThreeOrMore() // done
    {
        HashSet<Vector3Int> returnHashSet = new();
        HashSet<Vector3Int> currentPointsForDestroy = new();

        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++)
            {
                if (_spriteRenderersMatrix[i, j].gameObject.activeSelf) //enabled == true)
                {
                    int currentSpriteId = _itemsMatrix[i, j].ItemSettings.NameID;
                    CheckVertical(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);

                    CheckHorizontal(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);
                }
            }
        }

        return returnHashSet;
    }

    /// <summary>
    /// метод дополнительной проверки корректности полученных координат Match-3
    /// </summary>
    private void CheckValidate(HashSet<Vector3Int> currentPointsForDestroy, HashSet<Vector3Int> returnHashSet) //done 
    {
        if (currentPointsForDestroy.Count > 2)
        {
            foreach (var point in currentPointsForDestroy)
            {
                returnHashSet.Add(point);
            }
        }

        currentPointsForDestroy.Clear();
    }

    private void CheckVertical(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy) // done
    {
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int j = y + 1; j < _column; j++)
        {
            int currentID = _itemsMatrix[x, j].ItemSettings.NameID;
            if (currentID == ID && _spriteRenderersMatrix[x, j].gameObject.activeSelf) //enabled == true)
            {
                pointsForDestroy.Add(new Vector3Int(x, j));
            }
            else break;
        }

        for (int j = y - 1; j >= 0; j--)
        {
            int currentID = _itemsMatrix[x, j].ItemSettings.NameID;
            if (currentID == ID && _spriteRenderersMatrix[x, j].gameObject.activeSelf) //enabled == true)
            {
                pointsForDestroy.Add(new Vector3Int(x, j));
            }
            else break;
        }

        if (pointsForDestroy.Count < 3)
        {
            pointsForDestroy.Clear();
        }
    }

    private void CheckHorizontal(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy) //done
    {
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int i = x + 1; i < _row; i++)
        {
            int currentID = _itemsMatrix[i, y].ItemSettings.NameID;
            if (currentID == ID && _spriteRenderersMatrix[i, y].gameObject.activeSelf) //enabled == true)
            {
                pointsForDestroy.Add(new Vector3Int(i, y));
            }
            else break;
        }

        for (int i = x - 1; i >= 0; i--)
        {
            int currentID = _itemsMatrix[i, y].ItemSettings.NameID;
            if (currentID == ID && _spriteRenderersMatrix[i, y].gameObject.activeSelf) //enabled == true)
            {
                pointsForDestroy.Add(new Vector3Int(i, y));
            }
            else break;
        }

        if (pointsForDestroy.Count < 3)
        {
            pointsForDestroy.Clear();
        }
    }


    private IEnumerator RestoreGameField(HashSet<Vector3Int> match3) // need re-build
    {
        yield return new WaitForSeconds(0.3f + _delay);
        DeleteMatchThree(match3);
        yield return new WaitForSeconds(0.3f + _delay);
        FallDownItems();
        yield return new WaitForSeconds(0.5f + _delay);

        var newMatch3 = GetMatchThreeOrMore();
        if (newMatch3.Count > 2)
        {
            StartCoroutine(RestoreGameField(match3));
        }

        FillEmptyCellsAfterMatch3();
    }

    private IEnumerator
        SwapBackAnimation(RaycastHit2D hitDown, RaycastHit2D hitUp, int x, int y, Vector2Int direction) //done
    {
        yield return null; // is it right?
        yield return new WaitForSeconds(_delay + 0.3f); // is it right?
        SwapAnimation(hitDown, hitUp, x, y, direction);
    }
}