using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTextureSwap : MonoBehaviour
{
    public Texture[] textures;
    private Renderer rend;
    
    private const int NUM_TEXTURES = 6;
    private const int baseCode = (int)KeyCode.Alpha1;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < NUM_TEXTURES; i++)
        {
            KeyCode codeToCheck = (KeyCode)(CatTextureSwap.baseCode + i);
            if (Input.GetKeyDown(codeToCheck))
            {
                if (SetTexture(i))
                {
                    break;
                }
            }
        }
    }

    private bool SetTexture(int index)
    {
        if (index >= 0 && index < NUM_TEXTURES)
        {
            rend.material.mainTexture = textures[index];
            return true;
        }
        return false;
    }
}