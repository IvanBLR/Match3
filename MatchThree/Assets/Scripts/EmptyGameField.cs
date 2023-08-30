using UnityEngine;

public class EmptyGameField : MonoBehaviour
{
    [SerializeField]
    private GameObject _emptyTilePrefab;

    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private GameFieldSettings _gameFieldSettings;

    private void Start()
    {
        int x = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_ROW);
        int y = PlayerPrefs.GetInt(PlayerSettingsConst.GAME_FIELD_COLUMN);

        GenerateGameField(x, y);

        _gameFieldSettings.GameFieldColumnSizeChanged += GenerateGameField;
        _gameFieldSettings.GameFieldRawSizeChanged += GenerateGameField;
    }

    private void GenerateGameField(int sizeGameFieldX, int sizeGameFieldY)
    {
        CleanField();

        var cellGap = _grid.cellGap.x;
        var tilesAmount = sizeGameFieldX;
        var tilesSize = _grid.cellSize.x;

        var screenWidth = PlayerSettingsConst.SCREEN_WIDTH;

        var offset = GetOffset(screenWidth, tilesSize, tilesAmount, cellGap);

        _grid.transform.position = new Vector3(PlayerSettingsConst.START_GRID_POSITION.X, PlayerSettingsConst.START_GRID_POSITION.Y, PlayerSettingsConst.START_GRID_POSITION.Z);
        _grid.transform.position += new Vector3(offset, 0, 0);
        for (int i = 0; i < sizeGameFieldX; i++)
        {
            for (int j = 0; j < sizeGameFieldY; j++)
            {
                var localPosition = _grid.CellToLocal(new Vector3Int(i, j));
                var tile = Instantiate(_emptyTilePrefab, _grid.transform);
                tile.transform.localPosition = localPosition + _grid.cellGap;
            }
        }
    }
    private float GetOffset(float screenWidth, float tilesSize, int tilesAmount, float cellGap)
    {
        return ((screenWidth - ((tilesSize * tilesAmount) + (cellGap * (tilesAmount - 1)))) / 2) + tilesSize / 2;
    }
    private void CleanField()
    {
        int amount = _grid.transform.childCount;
        for (int i = 0; i < amount; i++)
        {
            Destroy(_grid.transform.GetChild(i).gameObject);
        }
    }
    private void OnDestroy()
    {
        _gameFieldSettings.GameFieldColumnSizeChanged -= GenerateGameField;
        _gameFieldSettings.GameFieldRawSizeChanged -= GenerateGameField;
    }
}
