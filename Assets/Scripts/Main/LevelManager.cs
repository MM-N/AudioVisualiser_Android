using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static LevelManager levelManager;
    private string _currentLoadedLevel = "";

    private void Awake()
    {
        if (levelManager != null)
        {
            GameObject.Destroy(levelManager);
        }
        else
        {
            levelManager = this;
        }

        DontDestroyOnLoad(this);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
