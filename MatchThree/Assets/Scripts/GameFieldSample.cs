using UnityEngine;

public class GameFieldSample : MonoBehaviour
{
    [SerializeField] private GameObject _emptyTilePrefab;
    [SerializeField] private Grid _grid;

    public void OnRestartInvoke()
    {
        int row = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_ROW);
        int column = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN);
        GenerateGameFieldSample(row, column);
    }
    public void GenerateGameFieldSample(int sizeGameFieldX, int sizeGameFieldY)
    {
        CleanGameFieldSample();

        var cellGap = _grid.cellGap.x;
        var tilesAmount = sizeGameFieldX;
        var tilesSize = _grid.cellSize.x;

        var screenWidth = PlayingSettingsConstant.SCREEN_WIDTH;

        var offset = GetOffset(screenWidth, tilesSize, tilesAmount, cellGap);

        _grid.transform.position = new Vector3(PlayingSettingsConstant.START_GRID_POSITION.X,
                                       PlayingSettingsConstant.START_GRID_POSITION.Y,
                                       PlayingSettingsConstant.START_GRID_POSITION.Z)
                                   + new Vector3(offset, 0, 0);

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

    public void CleanGameFieldSample()
    {
        int amount = _grid.transform.childCount;
        for (int i = 0; i < amount; i++)
        {
            Destroy(_grid.transform.GetChild(i).gameObject);
        }
    }

    private void Start()
    {
        int x = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_ROW);
        int y = PrefsManager.GetDataInt(PlayingSettingsConstant.GAME_FIELD_COLUMN);

        GenerateGameFieldSample(x, y);
    }

    private float GetOffset(float screenWidth, float tilesSize, int tilesAmount, float cellGap)
    {
        return ((screenWidth - ((tilesSize * tilesAmount) + (cellGap * (tilesAmount - 1)))) / 2) + tilesSize / 2;
    }
}