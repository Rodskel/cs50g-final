using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineGeneratorScript : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _tilePrefab;
    [Space(10), Header("Environment")]
    [SerializeField] private GameObject _tileContainer;
    [SerializeField] private Transform _wallTop;
    [SerializeField] private Transform _wallLeft;
    [SerializeField] private Transform _wallRight;
    [SerializeField] private Transform _wallBottom;
    [SerializeField] private Transform _terrain;
    [Tooltip("Empty space beyond the boundaries. (1f = 10u)")]
    [SerializeField] private float _spaceBeyond;

    public TileScript[,] GenerateMineField(int height, int width, int mines)
    {
        // GENERATING EMPTY TILE GRID.
        // Local tile array to store our minefield.
        TileScript[,] tileArray = new TileScript[height,width];
        // A placeholder tile game object.
        GameObject tile = new GameObject();
        // Generating empty field.
        for (int x = 0; x < height; x++)
        {
            for (int z = 0; z < width; z++)
            {
                // Instantiating a tile at current coordinates, rotation had to be down (up was inverted for some reason), and parent it to tile container.
                tile = GameObject.Instantiate(_tilePrefab, new Vector3(x, 0, z), Quaternion.LookRotation(Vector3.down), _tileContainer.transform);
                // Assigning current tile to tile array
                tileArray[x, z] = tile.GetComponent<TileScript>();
                // Giving tile a specific name.
                tileArray[x, z].name = $"tile_x{x}_z{z}";
            }
        }
        // ARRANGING WALLS & TERRAIN.
        // Top wall's position & scale, also texture scale.
        _wallTop.position = new Vector3(-0.5f, 0.5f, (width / 2f) - 0.5f);
        _wallTop.localScale = new Vector3(width, 1f, 1f);
        _wallTop.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(_wallTop.localScale.x, _wallTop.localScale.z));
        // Left wall's position & scale, also texture scale.
        _wallLeft.position = new Vector3((height / 2f) - 0.5f, 0.5f, -0.5f);
        _wallLeft.localScale = new Vector3(height, 1f, 1f);
        _wallLeft.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(_wallLeft.localScale.x, _wallLeft.localScale.z));
        // Right wall's position & scale, also texture scale.
        _wallRight.position = new Vector3((height / 2f) - 0.5f, 0.5f, width - 0.5f);
        _wallRight.localScale = new Vector3(height, 1f, 1f);
        _wallRight.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(_wallRight.localScale.x, _wallRight.localScale.z));
        // Bottom wall's position & scale, also texture scale.
        _wallBottom.position = new Vector3(height - 0.5f, 0.5f, (width / 2f) - 0.5f);
        _wallBottom.localScale = new Vector3(width, 1f, 1f);
        _wallBottom.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(_wallBottom.localScale.x, _wallBottom.localScale.z));
        // Adjusting terrain size and texture scale.
        _terrain.position = new Vector3((height / 2f) - 0.5f, -0.001f, (width / 2f) - 0.5f);
        _terrain.localScale = new Vector3((height / 10f) + _spaceBeyond, 1f, (width / 10f) + _spaceBeyond);
        _terrain.gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(_terrain.localScale.x * 10f, _terrain.localScale.z * 10f));
        // ASSIGNING MINES.
        // Counter for mines that are going to be generated.
        int mineCounter = 0;
        // Variables for random X & Z coordinates.
        int randX;
        int randZ;
        // While loop will go on until we have generated enough mines.
        while (mineCounter < mines)
        {
            // Assinging random values for our variables.
            randX = Mathf.RoundToInt(Random.Range(0, height - 1));
            randZ = Mathf.RoundToInt(Random.Range(0, width - 1));
            // If tile isn't already a mine.
            if (!tileArray[randX, randZ].IsMine)
            {
                // Making tile a mine.
                tileArray[randX, randZ].IsMine = true;
                // Increasing mine counter by 1.
                mineCounter++;
                // Going through all the adjacent tiles.
                for (int x = Mathf.Max(randX - 1, 0); x <= Mathf.Min(randX + 1, height - 1); x++)
                {
                    for (int z = Mathf.Max(randZ - 1, 0); z <= Mathf.Min(randZ + 1, width - 1); z++)
                    {
                        // If referanced tile is in the game field.
                        if (x >= 0 && x < height && z >= 0 && z < width)
                        {
                            // Getting that tile's TileScript and increasing nearbyMines by 1.
                            tileArray[x, z].NearbyMines++;
                        }
                    }
                }
            }
        }
        // Returning final tile array.
        return tileArray;
    }
}