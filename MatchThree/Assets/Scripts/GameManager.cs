using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameFieldController _gameFieldController;
    [SerializeField] private GameFieldSettings _gameFieldSettings;
    [SerializeField] private EmptyGameField _emptyGameField;
    [SerializeField] private SoundsManager _soundsManager;
    [SerializeField] private UI_controller _UI;
    [SerializeField] private Grid _grid;
    [SerializeField] private Button _bomb;
    [SerializeField] private Button _restart;
    [SerializeField] private TextMeshProUGUI _score;

    private float _click;
    private Vector2 _offset;
    private Vector3Int _hitPointDown;
    private Vector3Int _hitPointUp;
    private Vector3 _delta;
    private bool _isBombActive;
    private RaycastHit2D _raycastHitDown;
    private RaycastHit2D _raycastHitUp;

    [UsedImplicitly]
    public void BombActivation() => _isBombActive = true;

    [UsedImplicitly]
    public void ActivateRestartButton() => _restart.gameObject.SetActive(true);
    

    [UsedImplicitly]
    public void ActivateAdditionalOptions()
    {
        _bomb.gameObject.SetActive(true);
        _score.gameObject.SetActive(true);
    }

    private void Awake() //                                                  done
    {
        _score.gameObject.SetActive(false);
        _bomb.gameObject.SetActive(false);
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.InitializeActualItemsList;
        _gameFieldSettings.GameSettingsAccepted += _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
        _gameFieldSettings.GameFieldRawSizeChanged += _emptyGameField.GenerateGameField;
        _gameFieldSettings.GameFieldColumnSizeChanged += _emptyGameField.GenerateGameField;
        _gameFieldSettings.ActivatedEasyLevel += _UI.ActivateChoosenLevel;
        _gameFieldSettings.ActivetedMiddleLevel += _UI.ActivateChoosenLevel;
        _gameFieldSettings.ActivatedHardLevel += _UI.ActivateChoosenLevel;
        _gameFieldSettings.GameSettingsAccepted += _UI.DeactivateChoosenViewUILevels;

        _gameFieldController.GotMatchTree += _soundsManager.DropItems;
        _gameFieldController.WrongMatch3 += _soundsManager.SwapBack;
        _gameFieldController.BombUsed += _soundsManager.BombActivate;
        _gameFieldController.ScoreChanged += UpdateScore;

        _gameFieldController.InitializationActualItemsCompleted += DestroyAction;
        _gameFieldController.FilledGameBoard += DestroyAction;
    }

    private void Start()
    {
        _offset = new Vector2(_grid.cellSize.x / 2, _grid.cellSize.y / 2);
    }

    private void Update()
    {
        _click += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _click >= SettingsConstant.MIN_CLICK_INTERVAL)
        {
            if (!_isBombActive)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                _raycastHitDown = Physics2D.Raycast(ray.origin, ray.direction);
                _hitPointDown = GetHitPointCoordinateInGrid(_raycastHitDown);

                _click = 0;
            }
        }

        if (Input.GetMouseButtonUp(0) && _click >= SettingsConstant.MIN_CLICK_INTERVAL)
        {
            if (!_isBombActive)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                _raycastHitUp = Physics2D.Raycast(ray.origin, ray.direction);
                _hitPointUp = GetHitPointCoordinateInGrid(_raycastHitUp);

                int deltaX = _hitPointUp.x - _hitPointDown.x;
                int deltaY = _hitPointUp.y - _hitPointDown.y;

                CalculateInputDirectionAndStartSwap(deltaX, deltaY);
            }
        }

        if (Input.GetMouseButtonDown(0) && _isBombActive)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                var point = GetHitPointCoordinateInGrid(hit);
                _gameFieldController.UseBomb(point.x, point.y);
                _isBombActive = false;
            }
        }
    }

    private void CalculateInputDirectionAndStartSwap(int deltaX, int deltaY)
    {
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

    private Vector3Int GetHitPointCoordinateInGrid(RaycastHit2D raycastHit2D)
    {
        var hitPointInGridsCoordinate = Vector3Int.zero;
        if (raycastHit2D.collider != null)
        {
            var hitWorldCoordinates = raycastHit2D.point;
            hitPointInGridsCoordinate = _grid.WorldToCell(hitWorldCoordinates + _offset);
            return hitPointInGridsCoordinate;
        }
        else return new Vector3Int(100, 100, 100);
    }
    private void UpdateScore(int score)
    {
        _score.text = score.ToString();
    }
    private void DestroyAction() //          done
    {
        _gameFieldController.InitializationActualItemsCompleted -= DestroyAction;
        _gameFieldController.FilledGameBoard -= DestroyAction;
        _gameFieldController.FilledGameBoard -= _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.InitializeActualItemsList;
        _gameFieldSettings.GameSettingsAccepted -= _gameFieldController.FillGameBoardWithTilesFirstTimeOnly;
        _gameFieldSettings.GameFieldRawSizeChanged -= _emptyGameField.GenerateGameField;
        _gameFieldSettings.GameFieldColumnSizeChanged -= _emptyGameField.GenerateGameField;
    }

    private void OnDestroy()
    {
        _gameFieldController.GotMatchTree -= _soundsManager.DropItems;
        _gameFieldController.WrongMatch3 -= _soundsManager.SwapBack;
        _gameFieldController.BombUsed -= _soundsManager.BombActivate;
        _gameFieldController.ScoreChanged -= UpdateScore;
        _gameFieldSettings.ActivatedEasyLevel -= _UI.ActivateChoosenLevel;
        _gameFieldSettings.ActivetedMiddleLevel -= _UI.ActivateChoosenLevel;
        _gameFieldSettings.ActivatedHardLevel -= _UI.ActivateChoosenLevel;
        _gameFieldSettings.GameSettingsAccepted += _UI.DeactivateChoosenViewUILevels;
    }
}