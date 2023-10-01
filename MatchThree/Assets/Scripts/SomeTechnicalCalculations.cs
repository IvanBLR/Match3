using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SomeTechnicalCalculations
{
    public SomeTechnicalCalculations(GameFieldController gameFieldController)
    {
        _gameFieldController = gameFieldController;
    }

    private readonly GameFieldController _gameFieldController;

    private int _row = 0;
    private int _column;

    public void InitializeRowAndColumn()
    {
        _row = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_ROW);
        _column = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN);
    }

    public List<Vector3Int> GetBombsCoordinates(int x, int y)
    {
        List<Vector3Int> bombList = new List<Vector3Int>
        {
            new(0, 0, 0), new(0, 1, 0), new(1, 0, 0), new(1, 1, 0)
        };

        int startX = _row - 1 > x ? x : _row - 2;
        int startY = _column - 1 > y ? y : _column - 2;

        Vector3Int currentPoint = new Vector3Int(startX, startY, 0);
        for (int i = 0; i < bombList.Count; i++)
        {
            bombList[i] += currentPoint;
        }

        return bombList;
    }

    /// <summary>
    /// return choose player's set with 5 random element.
    /// </summary>
    public List<ItemScriptableObject> GetActualItemsList()
    {
        List<ItemScriptableObject> actualItemsListForReturn = new();
        var actualItemSettingsProvider =
            _gameFieldController.AllVariantsItemsCollections[_gameFieldController.ItemCollectionsNumber];
        var actualItemsCollection = actualItemSettingsProvider.ItemsList;
        int[] uniqueIndexes = Enumerable.Range(0, actualItemsCollection.Count)
            .OrderBy(x => Random.Range(0, actualItemsCollection.Count))
            .Take(5)
            .ToArray();
        for (int i = 0; i < 5; i++)
        {
            int index = uniqueIndexes[i];
            actualItemsListForReturn.Add(actualItemsCollection[index]);
        }

        return actualItemsListForReturn;
    }

    public int[] GetActualNameID()
    {
        int[] actualNameID = new int[5];
        for (int i = 0; i < 5; i++)
        {
            actualNameID[i] = _gameFieldController.ActualItemsList[i].ID;
        }

        return actualNameID;
    }

    /// <summary>
    ///  Метод помогает заполнить пустые ячейки, вернув нужный IndexSettings,
    /// kоторый гарантирует, что нигде не будет match-3
    /// /// </summary>
    public int GetCurrentSettingsIndex(int x, int y)
    {
        HashSet<int> checkList = new();
        int currentID = 0;
        int upID = 0;
        int downID = 0;
        int leftID = 0;
        int rightID = 0;
        if (y + 1 < _column && _gameFieldController.ItemsMatrix[x, y + 1] is not null)
        {
            upID = _gameFieldController.ItemsMatrix[x, y + 1].ItemsSettings.ID;
        }

        if (y - 1 >= 0) // можно не вводить доп.проверок на null
        {
            downID = _gameFieldController.ItemsMatrix[x, y - 1].ItemsSettings.ID;
        }

        if (x + 1 < _row && _gameFieldController.ItemsMatrix[x + 1, y] != null)
        {
            rightID = _gameFieldController.ItemsMatrix[x + 1, y].ItemsSettings.ID;
        }

        if (x - 1 >= 0) // можно не вводить доп.проверок на null
        {
            leftID = _gameFieldController.ItemsMatrix[x - 1, y].ItemsSettings.ID;
        }

        checkList.Add(upID);
        checkList.Add(downID);
        checkList.Add(leftID);
        checkList.Add(rightID);

        while (true)
        {
            int index = Random.Range(0, 5);
            currentID = _gameFieldController.ActualNameID[index];
            if (!checkList.Contains(currentID)) return index;
        }
    }

    public List<List<Vector3Int>> GetItemsCoordinatesForFalling(List<List<Vector3Int>> allEmptyCoordinates)
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

    public List<List<Vector3Int>> GetAllEmptyCoordinates()
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
    private List<Vector3Int> GetEmptyCoordinatesInCurrentColumnInGridNotation(int rowIndex)
    {
        List<Vector3Int> arrayForReturn = new();
        for (int j = 0; j < _column; j++)
        {
            var currentSpriteRenderer = _gameFieldController.SpriteRenderersMatrix[rowIndex, j];
            if (currentSpriteRenderer.gameObject.activeSelf) continue;
            var emptyPosition = new Vector3Int(rowIndex, j);
            arrayForReturn.Add(emptyPosition);
        }

        return arrayForReturn;
    }

    public HashSet<Vector3Int> GetMatchThreeOrMore()
    {
        HashSet<Vector3Int> returnHashSet = new();
        HashSet<Vector3Int> currentPointsForDestroy = new();

        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++)
            {
                if (_gameFieldController.SpriteRenderersMatrix[i, j].gameObject.activeSelf)
                {
                    int currentSpriteId = _gameFieldController.ItemsMatrix[i, j].ItemsSettings.ID;
                    CheckVertical(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);

                    CheckHorizontal(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);
                }
            }
        }

        return returnHashSet;
    }

    private void CheckVertical(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy)
    {
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int j = y + 1; j < _column; j++)
        {
            int currentID = _gameFieldController.ItemsMatrix[x, j].ItemsSettings.ID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[x, j].gameObject.activeSelf)
            {
                pointsForDestroy.Add(new Vector3Int(x, j));
            }
            else break;
        }

        for (int j = y - 1; j >= 0; j--)
        {
            int currentID = _gameFieldController.ItemsMatrix[x, j].ItemsSettings.ID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[x, j].gameObject.activeSelf)
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
            int currentID = _gameFieldController.ItemsMatrix[i, y].ItemsSettings.ID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[i, y].gameObject.activeSelf)
            {
                pointsForDestroy.Add(new Vector3Int(i, y));
            }
            else break;
        }

        for (int i = x - 1; i >= 0; i--)
        {
            int currentID = _gameFieldController.ItemsMatrix[i, y].ItemsSettings.ID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[i, y].gameObject.activeSelf)
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
    /// метод дополнительной проверки корректности полученных координат Match-3
    /// </summary>
    private void CheckValidate(HashSet<Vector3Int> currentPointsForDestroy, HashSet<Vector3Int> returnHashSet)
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
}