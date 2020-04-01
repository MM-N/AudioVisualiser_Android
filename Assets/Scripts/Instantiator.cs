using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//** Spawns GameObjects in a circle **//
public class Instantiator : MonoBehaviour
{
    public AudioManager _audioManager;

    public GameObject _prefabToSpawn;

    public float _maxScale = 1000.0f;

    private int _numObjectsToSpawn = 512;
    public int NumObjectsToSpawn
    {
        get { return _numObjectsToSpawn; }
        set { _numObjectsToSpawn = value; }
    }

    GameObject[] _spawnedObjectSample = new GameObject[512];
    float angleBetween;
    float radius = 100.0f;

    // Start is called before the first frame update

    private void Awake()
    {
       // _spawnedObjectSample = new GameObject[NumObjectsToSpawn];
        angleBetween = NumObjectsToSpawn / 360.0f;
    }
    void Start()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Missing reference to Audio Manager");
            Application.Quit();
        }

        for (int i = 0; i < NumObjectsToSpawn; i++)
        {
            GameObject _objectInstance = (GameObject)Instantiate(_prefabToSpawn);
            _objectInstance.transform.position = this.transform.position;
            _objectInstance.transform.parent = this.transform;
            _objectInstance.name = "Object_" + i;

            this.transform.eulerAngles = new Vector3(0.0f, -angleBetween * i, 0.0f);
            _objectInstance.transform.position = Vector3.forward * radius;
            _spawnedObjectSample[i] = _objectInstance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < NumObjectsToSpawn; i++)
        {
            if (_spawnedObjectSample != null)
            {
                _spawnedObjectSample[i].transform.localScale = new Vector3(1, (_audioManager._samples[i] * _maxScale) + 2, 1) ;
            }
        }
    }
}
