using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTextureSwap : MonoBehaviour
{
    public Texture[] textures;
    private Renderer rend;
    
    private const int NUM_TEXTURES = 6;
    
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for key presses from 1-6 and sets the texture accordingly
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTexture(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTexture(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTexture(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetTexture(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetTexture(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetTexture(5);
        }

    }
    
    void SetTexture(int index)
    {
        if (index >= 0 && index < NUM_TEXTURES)
        {
            rend.material.mainTexture = textures[index];
        }
    }
}
