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


/* 1. смотрим, где есть свободные €чейки                                                            done
   1a. получаем их координаты                                                                       done
   2. все элементы, которые наход€тс€ сверху над свободными позици€ми, должны упасть вниз
      а) создаю несколько массивов (максимум _row). Ёто будут колонны, участвующие в падении
      б) далее надо добавить в List<> те спрайты, которые
         наход€тс€ ¬џЎ≈ последнего значени€ Vector2Int.y в этой колонке.
         Ќужно сохранить 2 пол€: количество пустых €чеек в этой колонке

   3. после тотального падени€, надо найти координаты новых свободных €чеек
   4. и наконец, надо заполнить игровое поле
 }*/
    private void FallDownItems()
    {
        var emptyCoordinates = GetEmptyCellsGridsCoordinates();
    }

    public List<Vector2Int> GetItemsCoordinatesForFalling(List<Vector2Int> emptyCoordinates)//done
    {
        List<Vector2Int> returnList = new();
        Queue<Vector2Int> queueCoordinates = new();
        for (int i = 0; i < emptyCoordinates.Count; i++)
        {
            queueCoordinates.Enqueue(emptyCoordinates.ElementAt(i));
        }

        Vector2Int currentEmptyCoordinate;
        Vector2Int nextEmptyCoordinate;
        
        while (queueCoordinates.Count > 0)
        {
            currentEmptyCoordinate = queueCoordinates.Dequeue();
            if (queueCoordinates.Count > 0)
            {
                nextEmptyCoordinate = queueCoordinates.Peek();

                if (currentEmptyCoordinate.x != nextEmptyCoordinate.x)
                {
                    int row = currentEmptyCoordinate.x;
                    for (int j = currentEmptyCoordinate.y + 1; j < _column; j++)
                    {
                        returnList.Add((new Vector2Int(row, j)));
                    }
                }
            }
            else
            {
                int row = currentEmptyCoordinate.x;
                for (int j = currentEmptyCoordinate.y + 1; j < _column; j++)
                {
                    returnList.Add((new Vector2Int(row, j)));
                } 
            }
        }

        return returnList;
    }

    public List<Vector2Int> GetEmptyCellsGridsCoordinates() // done
    {
        List<Vector2Int> emptyCellsCoordinates = new();

        for (int i = 0; i < _itemsList.GetLength(0); i++)
        {
            for (int j = 0; j < _itemsList.GetLength(1); j++)
            {
                var currentItem = _itemsList[i, j];
                if (currentItem == null)
                {
                    var emptyPosition = new Vector2Int(i, j);
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