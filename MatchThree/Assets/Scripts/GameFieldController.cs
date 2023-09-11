using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using Random = System.Random;

public class GameFieldController : MonoBehaviour
{
    public Action TouchedTheItem;

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
    private bool[,] _currentEmptiesCells;
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
        _currentEmptiesCells = new bool[_row, _column]; // свободна €чейка? -> true, иначе false.
        // x и y €вл€ютс€ координатами дл€ Vector3Int

        InitializationActualItemsCompleted?.Invoke();
    }

    public void FillGameBoardWithTiles() // update, need check it
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
                
                _itemsMatrix[i, j].ItemSettings = currentSettings; // am I need both?
                _spriteRenderersMatrix[i, j].sprite = currentSettings.Icon; // am I need both?


                var targetScale = tile.transform.localScale;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(targetScale, 0.3f).SetDelay(0.2f);
            }
        }

        FilledGameBoard?.Invoke();
    }

    private int[] GetActualNameID()
    {
        int[] actualNameID = new int[5];
        for (int i = 0; i < 5; i++)
        {
            actualNameID[i] = _actualItemsList[i].NameID;
        }

        return actualNameID;
    } // done

    private int GetCurrentSettingsIndex(int x, int y)// done
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
            if (checkList.Contains(currentID))
                continue;
            else return index;
        }
    }

/* 1. смотрим, где есть свободные €чейки                                                                                 done
   1a. получаем их координаты                                                                                            done
   2. все элементы, которые наход€тс€ сверху над свободными позици€ми, должны упасть вниз                                done
   2a. найти координаты этих Ёлементов.                                                                                  done
   2б. Ќадо как-то разделить элементы-дл€-падени€ на колонки, чтобы анимаци€ вычисл€лась дл€ каждой колонки отдельно     done


   3а. после тотального падени€, надо найти координаты новых свободных €чеек (€ использую   _currentEmptiesCells)         done
   3б. нужно проверить, нет ли совпадений матч-3
   4. и наконец, надо заполнить игровое поле
 }*/
    public void FallDownItems() //                                         done
    {
        var emptyCoordinates = GetAllEmptyCoordinates();
        var itemCoordinatesForFalling = GetItemsCoordinatesForFalling(emptyCoordinates);

        for (int i = 0; i < emptyCoordinates.Count; i++)
        {
            if (emptyCoordinates[i].Count == 0)
            {
                continue;
            }
            else
            {
                List<Vector3Int> newEmptyCoordinates = emptyCoordinates[i];

                for (int k = 0; k < itemCoordinatesForFalling[i].Count; k++)
                {
                    newEmptyCoordinates.Add(itemCoordinatesForFalling[i][k]);
                }

                var sortedCoordinates = newEmptyCoordinates.OrderBy(vector => vector.y).ToList();

                if (itemCoordinatesForFalling[i].Count > 0)
                {
                    for (int n = 0; n < itemCoordinatesForFalling[i].Count; n++)
                    {
                        FallingAnimation(itemCoordinatesForFalling, i, n, sortedCoordinates);
                    }
                }
                else
                {
                    var firstEmptyCell = emptyCoordinates[i][0];
                    FillCurrentEmptiesCellsArray(firstEmptyCell, true);
                }
            }
        }
    }

    private void FallingAnimation(List<List<Vector3Int>> itemCoordinatesForFalling, int i, int j,
        List<Vector3Int> sortedCoordinates) //                                                               done
    {
        var tileForFalling =
            _spriteRenderersMatrix[itemCoordinatesForFalling[i][j].x,
                itemCoordinatesForFalling[i][j].y]; // right
        var targetPoint = sortedCoordinates[j];
        tileForFalling.transform.DOMove(_grid.CellToWorld(targetPoint), 0.5f);

        if (itemCoordinatesForFalling[i].Count - j == 1)
        {
            FillCurrentEmptiesCellsArray(sortedCoordinates[j]);
        }
    }

    private void FillCurrentEmptiesCellsArray(Vector3Int point, bool isFirst = false) // done
    {
        if (isFirst)
        {
            int x = point.x;
            for (int y = point.y; y < _column; y++)
            {
                _currentEmptiesCells[x, y] = true;
            }
        }
        else
        {
            int x = point.x;
            for (int y = point.y + 1; y < _column; y++)
            {
                _currentEmptiesCells[x, y] = true;
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

        int currentIndex = 0;

        for (int i = currentIndex; i < _row; i++)
        {
            var currentList = GetEmptyCoordinatesInCurrentColumnInGridNotation(i);
            returnList.Add(currentList);
        }

        return returnList;
    }

    private List<Vector3Int> GetEmptyCoordinatesInCurrentColumnInGridNotation(int rowIndex) // done. Use it 
    {
        List<Vector3Int> arrayForReturn = new();
        for (int j = 0; j < _column; j++)
        {
            var currentItem = _spriteRenderersMatrix[rowIndex, j];
            if (currentItem.enabled == false)
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
                if (!_currentEmptiesCells[i, j])
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

    private void CheckValidate(HashSet<Vector3Int> currentPointsForDestroy, HashSet<Vector3Int> returnHashSet)
    {
        if (currentPointsForDestroy.Count > 2)
        {
            foreach (var point in currentPointsForDestroy)
            {
                returnHashSet.Add(point);
            }

            currentPointsForDestroy.Clear();
        }
    }

    private void CheckVertical(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy)
    {
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int j = y + 1; j < _column; j++)
        {
            int currentID = _itemsMatrix[x, j].ItemSettings.NameID; // падает ошибка IndexOutOfRange
            if (currentID == ID)
            {
                pointsForDestroy.Add(new Vector3Int(x, j));
            }
            else break;
        }

        for (int j = y - 1; j >= 0; j--)
        {
            int currentID = _itemsMatrix[x, j].ItemSettings.NameID;
            if (currentID == ID)
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

    private void CheckHorizontal(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy)
    {
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int i = x + 1; i < _row; i++)
        {
            int currentID = _itemsMatrix[i, y].ItemSettings.NameID;
            if (currentID == ID)
            {
                pointsForDestroy.Add(new Vector3Int(i, y));
            }
            else break;
        }

        for (int i = x - 1; i >= 0; i--)
        {
            int currentID = _itemsMatrix[i, y].ItemSettings.NameID;
            if (currentID == ID)
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


    /// <summary>
    /// return choose player's set with 5 random element.
    /// After fill the game field with this elements
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
}