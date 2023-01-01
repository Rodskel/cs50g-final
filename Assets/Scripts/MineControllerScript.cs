using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineControllerScript : MonoBehaviour
{
    [Header("Script & Prefab References")]
    [Tooltip("Player GameObject.")]
    [SerializeField] private GameObject _player;
    [Tooltip("Mine Generator Script.")]
    [SerializeField] private MineGeneratorScript _mineGeneratorScript;
    [Tooltip("Player Prefab.")]
    [SerializeField] private GameObject _flagPolePrefab;
    [Space(10), Header("Map Attributes")]
    private TileScript[,] _tileArray;
    [Tooltip("How long should the map go on the X axis.")]
    public int _MapHeight {get; private set;}
    [Tooltip("How long should the map go on the Z axis.")]
    public int _MapWidth {get; private set;}
    [Tooltip("Used to calculate total mine number. ((Height x Width) x Diff)")]
    public float _DifficultyModifier {get; private set;}
    [Tooltip("Total number of mines. (Read Only)")]
    public int _MineTotal {get; private set;}
    [Tooltip("Number of mines that are flagged.")]
    public int FlaggedTilesCounter;

    /*private void OnEnable()
    {
        SetupField();
    }*/

    private void Start() {
        if (Application.isPlaying)
        {
            SetupField();
        }
    }

    private void SetupField()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager exist");
        }
        _MapHeight = GameManager.Instance.MapHeightSetting;
        _MapWidth = GameManager.Instance.MapWidthSetting;
        _DifficultyModifier = GameManager.Instance.DifficultySetting;

        _MineTotal = Mathf.RoundToInt((_MapHeight * _MapWidth) * _DifficultyModifier);

        _tileArray = _mineGeneratorScript.GenerateMineField(_MapHeight, _MapWidth, _MineTotal);

        
        // Assigning total number of mines to flag counter, it will count down with each flagged tile.
        FlaggedTilesCounter = _MineTotal;

        // Finding an empty tile so we can place the player.
        FindEmptyTile();
    }

    private void Update()
    {
        // Check if all mines are flagged or exploded, or all tiles except mines are uncovered
        if (AreAllMinesFlaggedOrExploded() || AreAllTilesExceptMinesUncovered())
        {
            // All mines are flagged or exploded, or all tiles except mines are uncovered, move to victory function
            GameManager.Instance.StopTimer();
            UncoverAllTilesAndFlagAllMines();
            StartCoroutine(GameManager.Instance.GameOver(true));
        }
        else if (FlaggedTilesCounter < 0)
        {
            // There are more flags than there should be
        }
        //
        if (GameManager.Instance.PlayerScript.Health <= 0f)
        {
            // Player's dead
            GameManager.Instance.StopTimer();
            StartCoroutine(GameManager.Instance.GameOver(false));
        }
    }

    void FindEmptyTile()
    {
        // Flag for our empty tile finder.
        bool emptyTileFound = false;
        // Counter for number of tries.
        int counter = 0;
        // Using while loop to find a suitable empty tile for player placement.
        while (!emptyTileFound)
        {
            // Checking random X & Z locations throughout our tile grid.
            int x = Mathf.RoundToInt(Random.Range(0, _MapHeight - 1));
            int z = Mathf.RoundToInt(Random.Range(0, _MapWidth - 1));
            // If "nearbyMines" is 0 that means that tile and all adjacent tiles are empty.
            if (_tileArray[x, z].NearbyMines == 0)
            {
                // Turning on our tile finder flag.
                emptyTileFound = true;
                
                Debug.Log("Found suitable location for player placement.");

                // Placing player and uncovering surrounding tiles.
                UncoverTile(new Vector2Int(x, z));
                PlacePlayer(new Vector2Int(x, z));
            }
            // If we failed at finding an empty tile.
            else
            {
                // Increasing counter with every failed attempt.
                counter++;
                // If the counter has exceeded total tile count and we couldn't found an empty tile.
                if (counter >= _MapHeight * _MapWidth && !emptyTileFound)
                {
                    Debug.Log("Could not found suitable location for player placement, restarting scene.");

                    StartCoroutine(GameManager.Instance.LoadScene("Play"));
                }
            }
        }
    }

    void PlacePlayer(Vector2Int location)
    {
        // Instantiating our player, placing it at X & Z coordinates, a little above the ground.
        _player.transform.SetPositionAndRotation(new Vector3(location.x, 1f, location.y), Quaternion.identity);
        _player.SetActive(true);
        Debug.Log(string.Format("Player placed at {0}.", _player.transform.position));
        GameManager.Instance.StartTimer();
    }

    public void UncoverTile(Vector2Int tile)
    {
        // If tile is covered and not flagged.
        if (_tileArray[tile.x, tile.y].IsCovered && !_tileArray[tile.x, tile.y].IsFlagged)
        {
            // Uncovering tile.
            _tileArray[tile.x, tile.y].IsCovered = false;
            // If tile is a mine, exploding it.
            if (_tileArray[tile.x, tile.y].IsMine)
            {
                _tileArray[tile.x, tile.y].IsExploding = true;
            }
            // If tile is empty, uncovering all nearby tiles.
            if (_tileArray[tile.x, tile.y].NearbyMines == 0)
            {
                // We're checking all adjacent tiles to our referenced tile and uncovering them.
                for (int x = Mathf.Max(tile.x - 1, 0); x <= Mathf.Min(tile.x + 1, _MapHeight - 1); x++)
                {
                    for (int z = Mathf.Max(tile.y - 1, 0); z <= Mathf.Min(tile.y + 1, _MapWidth - 1); z++)
                    {
                        // If the tile is within the play area and is covered & not a mine.
                        if (_tileArray[x, z].IsCovered && !_tileArray[x, z].IsMine)
                        {
                            // If this tile is also empty, recursively call this function again with new coordinates.
                            if (_tileArray[x, z].NearbyMines == 0)
                            {
                                UncoverTile(new Vector2Int(x, z));
                            }
                            // Uncover this tile
                            _tileArray[x, z].IsCovered = false;
                        }
                    }
                }
            }
        }
        // If tile is an uncovered number tile, sending it to multi-uncoverer.
        else if (!_tileArray[tile.x, tile.y].IsCovered && !_tileArray[tile.x, tile.y].IsMine 
        && _tileArray[tile.x, tile.y].NearbyMines != 0)
        {
            MultiUncoverNumberedTiles(new Vector2Int(tile.x, tile.y));
        }
    }

    public void MultiUncoverNumberedTiles(Vector2Int tile)
    {
        // Counter for flagged tiles adjacent to this tile.
        int flagCounter = 0;
        // List of covered adjacent tiles.
        List<TileScript> coveredAdjacentTiles = new List<TileScript>();
        // Loop through all adjacent tiles within the play area.
        for (int x = Mathf.Max(tile.x - 1, 0); x <= Mathf.Min(tile.x + 1, _MapHeight - 1); x++)
        {
            for (int z = Mathf.Max(tile.y - 1, 0); z <= Mathf.Min(tile.y + 1, _MapWidth - 1); z++)
            {
                // If mine is exploded already or flag is raised.
                if (_tileArray[x, z].IsMineExploded || _tileArray[x, z].IsFlagged)
                {
                    // Increase flag counter.
                    flagCounter++;
                }
                // Or else if the tile is covered.
                else if (_tileArray[x, z].IsCovered)
                {
                    // Add that tile to our covered tiles list.
                    coveredAdjacentTiles.Add(_tileArray[x, z]);
                }
            }
        }
        // If our flag counter is equal to our tile's nearbt mines number.
        if (_tileArray[tile.x, tile.y].NearbyMines == flagCounter)
        {
            // Go through our covered tiles list.
            foreach (TileScript aTile in coveredAdjacentTiles)
            {
                // If that tile is empty.
                if (aTile.NearbyMines == 0)
                {
                    // Uncover surrounding tiles.
                    UncoverTile(new Vector2Int((int)aTile.transform.position.x, (int)aTile.transform.position.z));
                }
                // Uncover tile.
                aTile.IsCovered = false;
                // If that tile is a mine.
                if (aTile.IsMine)
                {
                    // Explode that mine.
                    aTile.IsExploding = true;
                    //
                    //
                    //
                }
            }
        }
    }

    public void FlaggingTile(Vector2Int tile)
    {
        // Empty flag GameObject.
        GameObject flag;
        // If referenced tile is covered and not flagged.
        if (_tileArray[tile.x, tile.y].IsCovered && !_tileArray[tile.x, tile.y].IsFlagged &&
            !_tileArray[tile.x, tile.y].IsExploding && !_tileArray[tile.x, tile.y].IsMineExploded)
        {
            // Turning on tile's flagged state.
            _tileArray[tile.x, tile.y].IsFlagged = true;
            // Decreasing flagCounter.
            FlaggedTilesCounter--;
            // Instantiating flagPrefab, getting posititon from tile & rotating flag straight up.
            flag = GameObject.Instantiate(_flagPolePrefab, new Vector3(_tileArray[tile.x, tile.y].transform.position.x, 0f,
                _tileArray[tile.x, tile.y].transform.position.z), Quaternion.Euler(Vector3.up));
            // Parenting flag to the tile underneath it.
            flag.transform.parent = _tileArray[tile.x, tile.y].transform;
        }
        // If referenced tile is covered and flagged.
        else if (_tileArray[tile.x, tile.y].IsCovered && _tileArray[tile.x, tile.y].IsFlagged)
        {
            // Turning off tile's flagged state.
            _tileArray[tile.x, tile.y].IsFlagged = false;
            // Increasing flagCounter.
            FlaggedTilesCounter++;
            // Destroying instantiated flag.
            Destroy(_tileArray[tile.x, tile.y].transform.Find("FlagPole(Clone)").gameObject, 0f);
        }
        // If we're flagging uncovered number tile.
        else if (!_tileArray[tile.x, tile.y].IsCovered && !_tileArray[tile.x, tile.y].IsMine && _tileArray[tile.x, tile.y].NearbyMines != 0)
        {
            // Sending tile to multiflagger.
            MultiFlagTiles(new Vector2Int(tile.x, tile.y));
        }
    }

    public void MultiFlagTiles(Vector2Int tile)
    {
        // Counter for covered tiles adjacent to this tile.
        int coveredCounter = 0;
        // List of covered adjacent tiles.
        List<TileScript> coveredAdjacentTiles = new List<TileScript>();
        // Loop through all adjacent tiles.
        for (int x = tile.x - 1; x <= tile.x + 1; x++)
        {
            for (int z = tile.y - 1; z <= tile.y + 1; z++)
            {
                // If tile is within the play area
                if (x >= 0 && x < _MapHeight && z >= 0 && z < _MapWidth)
                {
                    // If mine is exploded already or flag is raised
                    if (_tileArray[x, z].IsCovered || _tileArray[x, z].IsMineExploded)
                    {
                        // Increase covered tiles counter
                        coveredCounter++;
                        // Add that tile to our covered tiles list
                        coveredAdjacentTiles.Add(_tileArray[x, z]);
                    }
                }
            }
        }
        // If our covered counter is equal to our tile's nearby mines number
        if (_tileArray[tile.x, tile.y].NearbyMines == coveredCounter)
        {
            // Go through our covered tiles list
            foreach (TileScript aTile in coveredAdjacentTiles)
            {
                // If tile is not already flagged
                if (!aTile.IsFlagged)
                {
                    // Send tiles to flagger
                    FlaggingTile(new Vector2Int((int)aTile.transform.position.x, (int)aTile.transform.position.z));
                }
            }
        }
    }

    bool AreAllMinesFlaggedOrExploded()
    {
        // Iterate through all tiles in the grid
        foreach (TileScript tile in _tileArray)
        {
            if (tile.IsMine && !tile.IsFlagged && !tile.IsMineExploded)
            {
                // If the tile is a mine and it is not flagged or exploded, return false
                return false;
            }
        }
        // All mines are flagged or exploded, return true
        return true;
    }

    bool AreAllTilesExceptMinesUncovered()
    {
        // Iterate through all tiles in the grid
        foreach (TileScript tile in _tileArray)
        {
            // If the tile is not a mine and it is still covered, return false
            if (!tile.IsMine && tile.IsCovered)
            {
                return false;
            }
        }
        // All tiles except mines are uncovered, return true
        return true;
    }

    private void UncoverAllTilesAndFlagAllMines()
    {
        Vector2Int tilePosition = new Vector2Int();
        for (int i = 0; i < _tileArray.GetLength(0); i++)
        {
            for (int j = 0; j < _tileArray.GetLength(1); j++)
            {
                TileScript tile = _tileArray[i, j];
                tilePosition.x = (int)tile.transform.position.x;
                tilePosition.y = (int)tile.transform.position.z;

                if (!tile.IsMine)
                {
                    UncoverTile(tilePosition);
                    if (tile.IsFlagged)
                    {
                        FlaggingTile(tilePosition);
                    }
                }
                else
                {
                    if (!tile.IsFlagged)
                    {
                        FlaggingTile(tilePosition);
                    }
                }
            }
        }
    }
}
