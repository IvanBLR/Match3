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

    private const float _minClickInterval = 0.4f;

    private float _click;
    private Vector2 _offset;
    private Vector3Int _hitPointDown;
    private Vector3Int _hitPointUp;
    private Vector3 _delta;
  
    private RaycastHit2D _raycastHitDown;
    private RaycastHit2D _raycastHitUp;

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
        _click += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.A))
        {
            //_gameFieldController.FallDownItems();
           // _gameFieldController.SSS();
            //_gameFieldController.Check();
        }


        if (Input.GetMouseButtonDown(0) && _click >= _minClickInterval)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _raycastHitDown = Physics2D.Raycast(ray.origin, ray.direction);
            _hitPointDown = GetHitPointCoordinateInGrid(_raycastHitDown);


            _click = 0;
        }

        if (Input.GetMouseButtonUp(0) && _click >= _minClickInterval)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _raycastHitUp = Physics2D.Raycast(ray.origin, ray.direction);
            _hitPointUp = GetHitPointCoordinateInGrid(_raycastHitUp);

            int deltaX = _hitPointUp.x - _hitPointDown.x;
            int deltaY = _hitPointUp.y - _hitPointDown.y;
           
            if (Math.Abs(deltaX) > Math.Abs(deltaY)) // horisontal swap
            {
                if (deltaX == 1 && deltaY == 0) // to right
                {
                    _gameFieldController.Swap(_raycastHitDown, _raycastHitUp, _hitPointDown.x,
                        _hitPointDown.y,
                        new Vector2Int(1, 0));
                }

                if (deltaX == -1 && deltaY == 0) // to left
                {
                   
                    _gameFieldController.Swap(_raycastHitDown, _raycastHitUp, _hitPointDown.x,
                        _hitPointDown.y,
                        new Vector2Int(-1, 0));
                }
            }
            else // vertical swap
            {
                if (deltaX == 0 && deltaY == 1) // to up
                {
                    _gameFieldController.Swap(_raycastHitDown, _raycastHitUp, _hitPointDown.x,
                        _hitPointDown.y,
                        new Vector2Int(0, 1));
                }

                if (deltaX == 0 && deltaY == -1) // to down
                {
                  
                    _gameFieldController.Swap(_raycastHitDown, _raycastHitUp, _hitPointDown.x,
                        _hitPointDown.y,
                        new Vector2Int(0, -1));
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            //  var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //  _raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
            //  var coordinate = _grid.WorldToCell(_raycastHit2D.point + _offset);
            // 
            //  Debug.Log(coordinate);
            //  if (_raycastHit2D.collider != null)
            //  {
            //      Debug.Log(_raycastHit2D.transform.GetComponent<Item>().ItemSettings.Icon.name);
            //  }
            //  Debug.Log($"tile LOCAL coordinate is {_gameFieldController.IIII[coordinate.x, coordinate.y].transform.localPosition}");
            //  Debug.Log($"tile world coordinate is {_gameFieldController.IIII[coordinate.x, coordinate.y].transform.position}");
        }
    }

    private Vector3Int GetHitPointCoordinateInGrid(RaycastHit2D raycastHit2D)
    {
        var hitPointInGridsCoordinate = Vector3Int.zero;
        // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (raycastHit2D.collider != null)
        {
            var hitWorldCoordinates = raycastHit2D.point;
            hitPointInGridsCoordinate = _grid.WorldToCell(hitWorldCoordinates + _offset);
            return hitPointInGridsCoordinate;
            //  Debug.Log($"grid coordinate = {_hitPointInGridsCoordinate.x}, {_hitPointInGridsCoordinate.y}");
        }
        else return new Vector3Int(100, 100, 100);
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