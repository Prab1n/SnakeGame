using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    
    public static GameAssets instance; //static variable to call in other class

    private void Awake()
    {
        instance = this;
    }
    public Sprite snakeHeadSprite;
    public Sprite foodSprite;
    public Sprite snakeBodySprite;
}
