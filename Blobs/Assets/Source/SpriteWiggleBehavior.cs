using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteWiggleBehavior : MonoBehaviour
{
    public Sprite[] mySprites;
    int mySpritePos = 0;
    public float wiggleSpeed;
    SpriteRenderer mySpriteRenderer;

    // Use this for initialization
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("Wiggling", wiggleSpeed);
    }

    void Wiggling()
    {
        if (mySpritePos >= mySprites.Length - 1)
        {
            mySpritePos = 0;
        }
        else
        {
            mySpritePos += 1;
        }

        mySpriteRenderer.sprite = mySprites[mySpritePos]; //Change sprite to next sprite
        Invoke("Wiggling", wiggleSpeed);
    }
}
