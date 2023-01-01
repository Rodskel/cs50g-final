using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileScript : MonoBehaviour {
    
    [Header("Covered/Uncovered")]
    [Tooltip("Is this tile covered?")]
    public bool IsCovered = true;
    [Tooltip("Material to swap when tile's uncovered.")]
    [SerializeField] private Material _uncoveredMaterial;
    [Space(10), Header("Adjacency")]
    [Tooltip("Number of mines adjacent to this tile.")]
    public int NearbyMines;
    [Tooltip("Text GameObject to show number of nearby mines.")]
    [SerializeField] private GameObject _nearbyText;
    [Tooltip("Text GameObject's TMP component.")]
    [SerializeField] private TextMeshPro _nearbyTextTMP;
    [Tooltip("Distance between text & player.")]
    [SerializeField] private float _distanceFromPlayer;
    [Tooltip("Distance text starts to fade away.")]
    [SerializeField] private int _distanceMin;
    [Tooltip("Distance text is completely disappears.")]
    [SerializeField] private int _distanceMax;
    [Tooltip("Color of numbers (0 is empty)")]
    [SerializeField] private Color[] _numberColorsArray = new Color[9];
    [Space(10), Header("Mine & Explosion")]
    [Tooltip("Is this tile a mine?")]
    public bool IsMine = false;
    [Tooltip("Reference to mine capsule trigger.")]
    [SerializeField] private CapsuleCollider _mineTrigger;
    [Tooltip("Explosion particle effects prefab.")]
    [SerializeField] private GameObject _explosionParticleFX;
    [Tooltip("Is this tile's mine exploding?")]
    public bool IsExploding = false;
    [Tooltip("Explosion force to apply on Rigidbodies.")]
    [SerializeField] private float _explosionForce = 10f;
    [Tooltip("Upwards force to apply on Rigidbodies.")]
    [SerializeField] private float _upwardsForce = 0.5f;
    [Tooltip("Explosion blast radius.")]
    [SerializeField] private float _blastRadius = 10f;
    [Tooltip("Maximum explosion damage.")]
    [SerializeField] private float _maxExplosionDamage = 110f;
    [Tooltip("Minimum explosion damage.")]
    [SerializeField] private float _minExplosionDamage = 0f;
    [Tooltip("Damage dropoff over distance.")]
    [SerializeField] private AnimationCurve _damageCurve;
    [Tooltip("Is this tile's mine exploded?")]
    public bool IsMineExploded = false;
    [Tooltip("Material to swap when mine explodes.")]
    [SerializeField] private Material _explodedMaterial;
    [Tooltip("Is this tile has been flagged?")]
    public bool IsFlagged = false;
    [Space(10), Header("Other")]
    [Tooltip("Sprite Renderer component for minimap projection.")]
    [SerializeField] private SpriteRenderer _minimapSpriteRenderer;
    [SerializeField] private Sprite[] _spritesArray;
    [Tooltip("Tile's Audio Source component.")]
    [SerializeField] private AudioSource _audioSource;
    [Tooltip("Audio Clips for tile.")]
    [SerializeField] private AudioClip[] _audioClipsArray;

    private void Start() {
        this.GetComponent<Renderer>().material.EnableKeyword ("_NORMALMAP");
        this.GetComponent<Renderer>().material.EnableKeyword ("_SPECULARMAP");
    }
    

    void Update()
    {
        if (_nearbyTextTMP.text == "#")
        {
            if (NearbyMines > 0 && !IsMine)
            {
                // Assigning number of nearby mines to text component
                _nearbyTextTMP.text = NearbyMines.ToString();
                // Changing color of text depending on number written
                _nearbyTextTMP.color = _numberColorsArray[NearbyMines];
            }
            else
            {
                _nearbyTextTMP.text = null;
                _nearbyTextTMP.gameObject.SetActive(false);
            }
        }
        // Getting player's distance from this tile
        _distanceFromPlayer = Vector3.Distance(this.transform.position, GameManager.Instance.PlayerScript.transform.position);

        if (IsMineExploded)
        {
            // If the mine is exploded setting exploded texture and appropriate minimap sprite
            this.GetComponent<Renderer>().material = _explodedMaterial;
            _minimapSpriteRenderer.sprite = _spritesArray[10];
        }
        else if (!IsCovered)
        {
            // If the tile is uncovered setting texture and minimap sprite according to nearby mines
            this.GetComponent<Renderer>().material = _uncoveredMaterial;
            //this.GetComponent<Renderer>().material.SetTexture("_MainTex", _uncoveredTexture);
            //this.GetComponent<Renderer>().material.SetTexture("_BumpMap", _uncoveredNormal);
            //this.GetComponent<Renderer>().material.SetTexture("_HeightMap", _uncoveredHeight);
            _minimapSpriteRenderer.sprite = _spritesArray[NearbyMines];
            // Optimizing performance with deactivating far text objects
            if (NearbyMines > 0 && !IsMine)
            {
                if (_distanceFromPlayer < _distanceMin)
                {
                    _nearbyText.SetActive(true);
                    _nearbyText.transform.localScale = Vector3.one;
                    _nearbyTextTMP.alpha = 1f;
                }
                else if (_distanceFromPlayer >= _distanceMin && _distanceFromPlayer < _distanceMax)
                {
                    // Setting text active when it's within render distance
                    _nearbyText.SetActive(true);
                    // Lerping scale & opacity by distance from player
                    _nearbyText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, (_distanceFromPlayer - _distanceMin) / (_distanceMax - _distanceMin));
                    _nearbyTextTMP.alpha = Mathf.Lerp(1f, 0f, (_distanceFromPlayer - _distanceMin) / (_distanceMax - _distanceMin));
                }
                else if (_distanceFromPlayer > _distanceMax)
                {
                    // Deactivating farthest text objects to gain performance
                    _nearbyText.SetActive(false);
                }
            }
            else
            {
                _nearbyText.SetActive(false);
            }
        }
        else
        {
            // If tile is covered, deactivate text object
            _nearbyText.SetActive(false);
            if (IsFlagged)
            {
                _minimapSpriteRenderer.sprite = _spritesArray[11];
            }
            else
            {
                _minimapSpriteRenderer.sprite = _spritesArray[9];
            }
        }
        // Making text above the tile always face the camera
        _nearbyText.transform.LookAt(Camera.main.transform);
        // Fixing rotation errors
        _nearbyText.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        // Mine explosion logic
        if (IsExploding && !IsMineExploded)
        {
            _audioSource.PlayOneShot(_audioClipsArray[0], GameManager.Instance.AudioEffectVolume);
            Invoke("ExplodeMine", 1f);
            IsExploding = false;
            IsMineExploded = true;
        }
    }

    public void ExplodeMine()
    {
        GameObject explosion = Instantiate(_explosionParticleFX);
        explosion.transform.position = transform.position;
        explosion.transform.rotation = Quaternion.identity;
        explosion.transform.parent = transform;

        _audioSource.PlayOneShot(_audioClipsArray[1], GameManager.Instance.AudioEffectVolume);
        //
        IsCovered = false;
        GameManager.Instance.MineControllerScript.FlaggedTilesCounter --;
        //
        Collider[] colliders = Physics.OverlapSphere(transform.position, _blastRadius);
        //
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (rb.gameObject.tag == "Player")
                {
                    float distance = Vector3.Distance(this.transform.position, rb.transform.position);
                    rb.GetComponent<PlayerScript>().DamagePlayer(Mathf.Lerp(_maxExplosionDamage, _minExplosionDamage, _damageCurve.Evaluate(distance / _blastRadius)));
                    //Debug.Log($"Player got {Mathf.Lerp(_maxExplosionDamage, _minExplosionDamage, _damageCurve.Evaluate(distance / _blastRadius))} damage.");
                    // If player is dead
                    if (rb.GetComponent<PlayerScript>().Health <= 0)
                    {
                        rb.constraints = RigidbodyConstraints.None;
                        rb.inertiaTensorRotation = Quaternion.identity;
                    }
                }
                rb.AddExplosionForce(_explosionForce, this.transform.position, _blastRadius, _upwardsForce, ForceMode.Impulse);
            }
        }
    }
}