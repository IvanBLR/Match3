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
    private Item[,] _itemsList;
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
        _itemsList = new Item[_row, _column];

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

                _itemsList[i, j] = tile.GetComponent<Item>();
            }
        }

        FilledGameBoard?.Invoke();
    }


/* 1. смотрим, где есть свободные €чейки                                                                                 done
   1a. получаем их координаты                                                                                            done
   2. все элементы, которые наход€тс€ сверху над свободными позици€ми, должны упасть вниз
   2a. найти координаты этих Ёлементов.                                                                                  done
   2б. Ќадо как-то разделить элементы-дл€-падени€ на колонки, чтобы анимаци€ вычисл€лась дл€ каждой колонки отдельно
   3. после тотального падени€, надо найти координаты новых свободных €чеек
   4. и наконец, надо заполнить игровое поле
 }*/
    public void FallDownItems() //                                                                   need check it
    {
       // var X = GetAllEmptyCoordinates();
       // for (int i = 0; i < X.Count; i++)
       // for (int j = 0; j < X[i].Count; j++)
       //     Debug.Log(X[i][j]);


        // var emptyCoordinates = GetEmptyCoordinatesInGridNotation();
        // var itemCoordinatesForFalling = GetItemsCoordinatesForFallingInGridNotation(emptyCoordinates);
//
        // //List<Vector3> newEmptyCoordinates = new();
//
//
        // if (emptyCoordinates.Count >= itemCoordinatesForFalling.Count)
        // {
        //     for (int i = 0; i < emptyCoordinates.Count; i++)
        //     {
        //         var tileForFalling = _itemsList[itemCoordinatesForFalling.ElementAt(i).x,
        //             itemCoordinatesForFalling.ElementAt(i).y];
//
        //         var targetPositionWorld = _grid.CellToWorld(emptyCoordinates[i]);
//
        //         tileForFalling.transform.DOMove(targetPositionWorld, 1f);
        //     }
        // }
        // else
        // {
        // }
    }

    private List<Vector3Int> GetItemsCoordinatesForFallingInGridNotation(List<Vector3Int> emptyCoordinates) // done
    {
        List<Vector3Int> returnList = new();
        Queue<Vector3Int> queueCoordinates = new();
        for (int i = 0; i < emptyCoordinates.Count; i++)
        {
            queueCoordinates.Enqueue(emptyCoordinates.ElementAt(i));
        }

        Vector3Int currentEmptyCoordinate;
        Vector3Int nextEmptyCoordinate;

        while (queueCoordinates.Count > 0)
        {
            currentEmptyCoordinate = queueCoordinates.Dequeue();
            if (queueCoordinates.Count > 0)
            {
                nextEmptyCoordinate = queueCoordinates.Peek();

                if (currentEmptyCoordinate.x != nextEmptyCoordinate.x)
                {
                    var row = currentEmptyCoordinate.x;
                    for (var j = currentEmptyCoordinate.y + 1; j < _column; j++)
                    {
                        returnList.Add((new Vector3Int(row, j)));
                    }
                }
            }
            else
            {
                var row = currentEmptyCoordinate.x;
                for (var j = currentEmptyCoordinate.y + 1; j < _column; j++)
                {
                    returnList.Add((new Vector3Int(row, j)));
                }
            }
        }

        return returnList;
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
            var currentItem = _itemsList[rowIndex, j];
            if (currentItem.GetComponent<SpriteRenderer>().enabled == false)
            {
                var emptyPosition = new Vector3Int(rowIndex, j);
                arrayForReturn.Add(emptyPosition);
            }
        }

        return arrayForReturn;
    }

    private List<Vector3Int> GetEmptyCoordinatesInGridNotation() // done, but dont' use it
    {
        List<Vector3Int> emptyCellsCoordinates = new();

        for (int i = 0; i < _itemsList.GetLength(0); i++)
        {
            for (int j = 0; j < _itemsList.GetLength(1); j++)
            {
                var currentItem = _itemsList[i, j];
                if (currentItem.GetComponent<SpriteRenderer>().enabled == false)
                {
                    var emptyPosition = new Vector3Int(i, j);
                    emptyCellsCoordinates.Add(emptyPosition);
                }
            }
        }

        return emptyCellsCoordinates;
    }

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
            actualItemsListForReturn.Add(actualItemsCollection.ElementAt(index));
        }

        return actualItemsListForReturn;
    }
}