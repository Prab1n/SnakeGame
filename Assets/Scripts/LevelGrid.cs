using CodeMonkey;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LevelGrid
{
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;

        

        
    }

    public void Setup(Snake snake)
    {
        this.snake = snake;

        SpawnFood();
    }
    public void SpawnFood()
    {
        do {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        }
        while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);/* snake.GetGridPosition()==foodGridPosition);*/
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
        /*Debug.Log("yeta samma print bhako hora?" + foodGameObject);*/
    }

   /* public void SnakeMoved(Vector2Int snakeGridPosition)
    {
        if(snakeGridPosition ==  foodGridPosition)
        {
            Debug.Log("yesma value " + foodGameObject);
            Object.Destroy(foodGameObject);
            SpawnFood();
            CMDebug.TextPopupMouse("Food eaten");
        }
    }*/

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Debug.Log("yo snake ko position" + snakeGridPosition + "yo food ko position" + foodGridPosition);

            Debug.Log("game object ma yo cha " + foodGameObject);
            
            if (foodGameObject == null)
            {
                Debug.Log("food k xa" + foodGameObject);

            }

            Object.Destroy(foodGameObject);
            SpawnFood();
            GameHandler.AddScore();
            return true;

        }
        else
        {
            return false;
        }
    }
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0) //for left
        {
            gridPosition.x = width - 0;
            
        }
        if (gridPosition.x > width - 0) //for right
        {
            gridPosition.x = 0;
        }
        if (gridPosition.y < 0) //for down
        {
            gridPosition.y = height - 0;
            
        }
        if(gridPosition.y > height - 0) //for up
        {
            gridPosition.y = 0;
        }
        return gridPosition;
    }
}

