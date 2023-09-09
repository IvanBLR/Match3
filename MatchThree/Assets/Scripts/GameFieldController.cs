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
    private Item[,] _itemsMatrixArray;
    private int _itemCollectionsNumber;
    private int _row;
    private int _column;


    private List<List<Vector3Int>> Z = new();

    private void Start()
    {
        // _gameFieldSettings.GameSettingsAccepted += InitializeActualItemsListClassFields;      € их подписал ->
        // _gameFieldSettings.GameSettingsAccepted += FillGameBoardWithTiles;                    в √еймћенеджере
    }

    public void InitializeActualItemsListClassFields() // done
    {
        _actualItemsList = GetActualItemsList();
        _row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        _column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        _parent.position = _grid.transform.position;
        _itemsMatrixArray = new Item[_row, _column];

        InitializationActualItemsCompleted?.Invoke();
    }

    public void FillGameBoardWithTiles() // done
    {
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++)
            {
                var position = _grid.CellToLocal(new Vector3Int(i, j));
                var tile = Instantiate(_itemPrefab, _parent);
                tile.transform.localPosition = position + _grid.cellGap;

                var targetScale = tile.transform.localScale;
                tile.transform.localScale = Vector3.zero;
                tile.transform.DOScale(targetScale, 0.3f).SetDelay(0.2f);

                _itemsMatrixArray[i, j] = tile.GetComponent<Item>();
            }
        }

        FilledGameBoard?.Invoke();
    }


/* 1. смотрим, где есть свободные €чейки                                                                                 done
   1a. получаем их координаты                                                                                            done
   2. все элементы, которые наход€тс€ сверху над свободными позици€ми, должны упасть вниз                                done
   2a. найти координаты этих Ёлементов.                                                                                  done
   2б. Ќадо как-то разделить элементы-дл€-падени€ на колонки, чтобы анимаци€ вычисл€лась дл€ каждой колонки отдельно     done
   
   3. после тотального падени€, надо найти координаты новых свободных €чеек
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

                for (int n = 0; n < itemCoordinatesForFalling[i].Count; n++)
                {
                    var tileForFalling =
                        _itemsMatrixArray[itemCoordinatesForFalling[i][n].x,
                            itemCoordinatesForFalling[i][n].y]; // right
                    var targetPoint = sortedCoordinates[n];
                    tileForFalling.transform.DOMove(_grid.CellToWorld(targetPoint), 0.5f);
                }
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
            var currentItem = _itemsMatrixArray[rowIndex, j];
            if (currentItem.GetComponent<SpriteRenderer>().enabled == false)
            {
                var emptyPosition = new Vector3Int(rowIndex, j);
                arrayForReturn.Add(emptyPosition);
            }
        }

        return arrayForReturn;
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