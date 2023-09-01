using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
/*using CodeMonkeyUtils;*/

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;

    private static int score;

    [SerializeField] private Snake snake;

    private LevelGrid levelGrid;

    private void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    private void Start()

    {

        Debug.Log("GameHandler chalyo");

        levelGrid = new LevelGrid(20, 20);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 100;
    }
}

        /* int number = 0;
         FunctionPeriodic.Create(() =>
         {
             CMDebug.TextPopupMouse("Ding! " + number);
             number++;
         }, .3f);*/

       /* GameObject snakeHeadGameObject = new GameObject();
        SpriteRenderer snakeSpriteRenderer = snakeHeadGameObject.AddComponent<SpriteRenderer>();
        snakeSpriteRenderer.sprite = GameAssets.instance.snakeHeadSprite;*/


    

    // Update is called once per frame
    /*void Update()
    {
        
    }
}*/
