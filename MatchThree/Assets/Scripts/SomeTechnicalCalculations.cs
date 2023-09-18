using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class SomeTechnicalCalculations
{
    private GameFieldController _gameFieldController;

    public SomeTechnicalCalculations(GameFieldController gameFieldController)
    {
        _gameFieldController = gameFieldController;
    }

    /// <summary>
    /// return choose player's set with 5 random element.
    /// </summary>
    public List<ItemScriptableObject> GetActualItemsList() //done
    {
        List<ItemScriptableObject> actualItemsListForReturn = new();
        Random random = new Random();
        var actualItemSettingsProvider = _gameFieldController.AllVariantsItemsCollections.ElementAt(_gameFieldController.ItemCollectionsNumber);
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

    public int[] GetActualNameID() // done
    {
        int[] actualNameID = new int[5];
        for (int i = 0; i < 5; i++)
        {
            actualNameID[i] = _gameFieldController.ActualItemsList[i].NameID;
        }

        return actualNameID;
    }

    /// <summary>
    ///  Метод помогает заполнить пустые ячейки, вернув нужный IndexSettings,
    /// kоторый гарантирует, что нигде не будет match-3
    /// /// </summary>
    public int GetCurrentSettingsIndex(int x, int y) // done.
    {
        Random random = new Random();
        HashSet<int> checkList = new();
        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        int currentID = 0;
        int upID = 0;
        int downID = 0;
        int leftID = 0;
        int rightID = 0;
        if (y + 1 < column && _gameFieldController.ItemsMatrix[x, y + 1] is not null)
        {
            upID = _gameFieldController.ItemsMatrix[x, y + 1].ItemSettings.NameID;
        }

        if (y - 1 >= 0) // можно не вводить доп.проверок на null
        {
            downID = _gameFieldController.ItemsMatrix[x, y - 1].ItemSettings.NameID;
        }

        if (x + 1 < row && _gameFieldController.ItemsMatrix[x + 1, y] != null)
        {
            rightID = _gameFieldController.ItemsMatrix[x + 1, y].ItemSettings.NameID;
        }

        if (x - 1 >= 0) // можно не вводить доп.проверок на null
        {
            leftID = _gameFieldController.ItemsMatrix[x - 1, y].ItemSettings.NameID;
        }

        checkList.Add(upID);
        checkList.Add(downID);
        checkList.Add(leftID);
        checkList.Add(rightID);

        while (true)
        {
            int index = random.Next(0, 5);
            currentID = _gameFieldController.ActualNameID[index];
            if (!checkList.Contains(currentID)) return index;
        }
    }

    public List<List<Vector3Int>> GetAllEmptyCoordinates() // done
    {
        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        List<List<Vector3Int>> returnList = new();

        for (int i = 0; i < row; i++)
        {
            var currentList = GetEmptyCoordinatesInCurrentColumnInGridNotation(i);
            returnList.Add(currentList);
        }

        return returnList;
    }

    /// <summary>
    /// вспомогательный метод
    /// </summary>
    private List<Vector3Int> GetEmptyCoordinatesInCurrentColumnInGridNotation(int rowIndex) // done 
    {
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        List<Vector3Int> arrayForReturn = new();
        for (int j = 0; j < column; j++)
        {
            var currentSpriteRenderer = _gameFieldController.SpriteRenderersMatrix[rowIndex, j];
            if (!currentSpriteRenderer.gameObject.activeSelf) //currentSpriteRenderer.enabled == false)
            {
                var emptyPosition = new Vector3Int(rowIndex, j);
                arrayForReturn.Add(emptyPosition);
            }
        }

        return arrayForReturn;
    }

    public HashSet<Vector3Int> GetMatchThreeOrMore() // done
    {
        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        HashSet<Vector3Int> returnHashSet = new();
        HashSet<Vector3Int> currentPointsForDestroy = new();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (_gameFieldController.SpriteRenderersMatrix[i, j].gameObject.activeSelf)
                {
                    int currentSpriteId = _gameFieldController.ItemsMatrix[i, j].ItemSettings.NameID;
                    CheckVertical(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);

                    CheckHorizontal(i, j, currentSpriteId, currentPointsForDestroy);
                    CheckValidate(currentPointsForDestroy, returnHashSet);
                }
            }
        }

        return returnHashSet;
    }
    
    private void CheckVertical(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy) // done
    {
        int column = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int j = y + 1; j < column; j++)
        {
            int currentID = _gameFieldController.ItemsMatrix[x, j].ItemSettings.NameID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[x, j].gameObject.activeSelf)
            {
                pointsForDestroy.Add(new Vector3Int(x, j));
            }
            else break;
        }

        for (int j = y - 1; j >= 0; j--)
        {
            int currentID = _gameFieldController.ItemsMatrix[x, j].ItemSettings.NameID;
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

    private void CheckHorizontal(int x, int y, int ID, HashSet<Vector3Int> pointsForDestroy) //done
    {
        int row = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        pointsForDestroy.Add(new Vector3Int(x, y));
        for (int i = x + 1; i < row; i++)
        {
            int currentID = _gameFieldController.ItemsMatrix[i, y].ItemSettings.NameID;
            if (currentID == ID && _gameFieldController.SpriteRenderersMatrix[i, y].gameObject.activeSelf)
            {
                pointsForDestroy.Add(new Vector3Int(i, y));
            }
            else break;
        }

        for (int i = x - 1; i >= 0; i--)
        {
            int currentID = _gameFieldController.ItemsMatrix[i, y].ItemSettings.NameID;
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

}