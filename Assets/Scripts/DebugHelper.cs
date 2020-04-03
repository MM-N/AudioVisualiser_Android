using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    // Start is called before the first frame update
    public static DebugHelper debugHelper;

    //** Change to OnValidate if needed in editor **//
    private void Awake()
    {
        if (debugHelper != null)
        {
            GameObject.Destroy(debugHelper);
        }
        else {
            debugHelper = this;
        }

        DontDestroyOnLoad(this);
    }

    public bool IsZeroValue(float valueIn)
    {
        if (valueIn == 0.0f)
        {
            return true;
        }
        else
        {
            Debug.LogError("Attempting to divide by 0.");
            return false;
        }
    }

}
