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
    private Vector3 _hitPoint;
    private Vector3 _pointUp;
    private Vector3 _delta;
    private Vector3Int _hitPointInGridsCoordinate;

    private void Awake() //                                                  done
    {
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.InitializeActualItemsListClassFields;
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            _gameFieldController.FallDownItems();
            _gameFieldController.Check();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CalculateHitPointCoordinate();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _pointUp = Input.mousePosition;
            _delta = _pointUp - _hitPoint;

            if (Mathf.Abs(_delta.x) > Mathf.Abs(_delta.y)) // horisontal
            {
                if (_delta.x > 0) // to right
                {
                    _gameFieldController.Swap(_hitPointInGridsCoordinate.x, _hitPointInGridsCoordinate.y,
                        new Vector2Int(1, 0));
                }
                else // to left
                {
                    _gameFieldController.Swap(_hitPointInGridsCoordinate.x, _hitPointInGridsCoordinate.y,
                        new Vector2Int(-1, 0));
                }
            }
            else // vertical
            {
                if (_delta.y > 0) // to up
                {
                    _gameFieldController.Swap(_hitPointInGridsCoordinate.x, _hitPointInGridsCoordinate.y,
                        new Vector2Int(0, 1));
                }
                else // to down
                {
                    _gameFieldController.Swap(_hitPointInGridsCoordinate.x, _hitPointInGridsCoordinate.y,
                        new Vector2Int(0, -1));
                }
            }
        }
    }

    private void CalculateHitPointCoordinate()
    {
        _hitPoint = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(_hitPoint);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null)
        {
            var hitWorldCoordinates = hit.point;
            _hitPointInGridsCoordinate = _grid.WorldToCell(hitWorldCoordinates + _offset);
        }
    }

    private void DestroyAction() //                                          done
    {
        _gameFieldController.InitializationActualItemsCompleted -= DestroyAction;
        _gameFieldController.FilledGameBoard -= DestroyAction;
        _gameFieldController.FilledGameBoard -= _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.InitializeActualItemsListClassFields;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
        _gameFieldSettings.GameFieldRawSizeChanged -= _emptyGameField.GenerateGameField;
        _gameFieldSettings.GameFieldColumnSizeChanged -= _emptyGameField.GenerateGameField;
    }
}