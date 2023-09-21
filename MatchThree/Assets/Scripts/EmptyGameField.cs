using UnityEngine;

public class EmptyGameField : MonoBehaviour
{
    [SerializeField] private GameObject _emptyTilePrefab;
    [SerializeField] private Grid _grid;

    private void Start()
    {
        int x = PlayerPrefs.GetInt(SettingsConstant.GAME_FIELD_ROW);
        int y = PlayerPrefs.GetInt(SettingsConstant.GAME_FIELD_COLUMN);

        GenerateGameField(x, y);
    }

    public void GenerateGameField(int sizeGameFieldX, int sizeGameFieldY)
    {
        CleanField();

        var cellGap = _grid.cellGap.x;
        var tilesAmount = sizeGameFieldX;
        var tilesSize = _grid.cellSize.x;

        var screenWidth = SettingsConstant.SCREEN_WIDTH;

        var offset = GetOffset(screenWidth, tilesSize, tilesAmount, cellGap);

        _grid.transform.position = new Vector3(SettingsConstant.START_GRID_POSITION.X,
                                       SettingsConstant.START_GRID_POSITION.Y,
                                       SettingsConstant.START_GRID_POSITION.Z)
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
}