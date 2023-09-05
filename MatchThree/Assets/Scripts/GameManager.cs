using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameFieldController _gameFieldController;
    [SerializeField] private GameFieldSettings _gameFieldSettings;
    [SerializeField] private EmptyGameField _emptyGameField;
    [SerializeField] private Grid _grid;

    private Vector2 _offset;

    private void Awake() //                                                  done
    {
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.InitializeActualItemsListClassFields;
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.FillGameBoardWithTiles;
        _gameFieldSettings.GameFieldRawSizeChanged += _emptyGameField.GenerateGameField;
        _gameFieldSettings.GameFieldColumnSizeChanged += _emptyGameField.GenerateGameField;


        _gameFieldController.InitializationActualItemsCompleted += DestroyAction;
        _gameFieldController.FilledGameBoard += DestroyAction;
    }


    private void Start()
    {
        _offset = new Vector2(_grid.cellSize.x / 2, _grid.cellSize.y / 2);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                var hitWorldCoordinates = hit.point;
                var hitGridCoordinates = _grid.WorldToCell(hitWorldCoordinates + _offset);


                /* тут надо вызывать метод из GameFieldController -> GetEmptyCellsGridsPositions(),
                 * в котором получаем координаты пустых ячеек.
                 * Используем либо Action, либо напрямую вызываем через ссылку на GameFieldController
                 */
            }
        }
    }

    private void DestroyAction() //                                          done
    {
        _gameFieldController.InitializationActualItemsCompleted -= DestroyAction;
        _gameFieldController.FilledGameBoard -= DestroyAction;
        _gameFieldController.FilledGameBoard -= _gameFieldController.FillGameBoardWithTiles;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.InitializeActualItemsListClassFields;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.FillGameBoardWithTiles;
        _gameFieldSettings.GameFieldRawSizeChanged -= _emptyGameField.GenerateGameField;
        _gameFieldSettings.GameFieldColumnSizeChanged -= _emptyGameField.GenerateGameField;
    }
}