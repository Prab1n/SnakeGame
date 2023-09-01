using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }

    private State state;
    private Direction gridMoveDirection; //variable to store which direction
    private Vector2Int gridPosition; // to hold grid position in int using vector2int
    private float gridMoveTimer;    // this is set to count the time to move the snake automatically.
    private float gridMoveTimerMax; // this contains the max time taken to move from one distance to other.
    private LevelGrid levelGrid;
    private int snakeBodySize; //stores the snake body size
    private List<SnakeMovePosition> snakeMovePositionList; //this stores where snake has been + add size to snake list 
     List<SnakeBodyPart> snakeBodyPartList;
/*    private float snakeMovePosition;*/

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10); //setting the grid position
        gridMoveTimerMax = 0.3f; //snake moves from 1 grid to another in this set time.
        gridMoveTimer = gridMoveTimerMax; //setting the move timer same as move timer max.
        gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 10;

        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandlerGridMovement();
                break;
            case State.Dead:
                break;
        }
       
       
    }


    //Setting key for the snake's movement.
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
                
            }



        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
               
            }

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
                
            }

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
                
            }

        }
    }
    private void HandlerGridMovement()
    {
        gridMoveTimer += Time.deltaTime; //helps snake to move every second.
        if (gridMoveTimer >= gridMoveTimerMax) //if grid move timer is more than grid move timer max than enter inside the function.
        {

            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
           
            
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                    case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                    case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                    case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
                    case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }


            gridPosition += gridMoveDirectionVector;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                //snake body size increases
                snakeBodySize++;
                CreateSnakeBodyPart();
            }




            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1); ;
            }

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                print("kati?" + snakeBodyPartList.Count);
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartGridPosition)
                {
                    CMDebug.TextPopup("DEAD", transform.position);
                    state = State.Dead;
                }
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y); //placing the snake correcting in the grid i.e in the middle of the grid
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) -90);

            UpdateSnakeBodyPart();

            


           
        }

    }

    /*private void UpdateSnakeBodyPart()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            *//*Vector3 snakeBodyPosition = new Vector3(snakeMovePositionList[i].x, snakeMovePositionList[i].y);*//*
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
            Debug.Log("asdgasdgsa"+ snakeBodyPartList.Count);
        }
    }*/
    private void UpdateSnakeBodyPart()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            int snakeMoveIndex = i; // Get the index for snakeMovePositionList
            if (snakeMoveIndex < snakeMovePositionList.Count) // Check if snakeMoveIndex is within bounds
            {
                snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[snakeMoveIndex]);
            }
        }
    }






    /*private void CreateSnakeBodyPart()
    {
        *//*GameObject snakeBodyGameObject = new GameObject("Snake Body", typeof(SpriteRenderer));
        snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
        snakeBodyTransformList.Add(snakeBodyGameObject.transform);
        snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = - snakeBodyTransformList.Count;
        *//*Debug.Log("Body create bhairacha ta?");*//*

        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }*/

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count, snakeMovePositionList[0])); // Pass the initial move position
    }


    private float GetAngleFromVector (Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }


    //Returns the full list of position occupied by snake  
    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
/*        gridPositionList.AddRange(snakeMovePositionList);*/
        return gridPositionList;
    }

    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex, SnakeMovePosition initialMovePosition)
        {
            GameObject snakeBodyGameObject = new GameObject("Snake Body", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;

            snakeMovePosition = initialMovePosition; // Initialize the snakeMovePosition field
        }

        // Rest of the class remains the same...
    


    public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
           this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle ;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up:
                    switch (snakeMovePosition.GetPreviousDirection())  //Currently going up
                    {
                        default:
                            angle = 0; break;
                        case Direction.Left: // Previously going Left
                            angle = 0 + 45; break;
                        case Direction.Right: //Previously going Right
                            angle = 0 - 45;  break;

                    }
                    break;

                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection()) //Currently going Down
                    {
                        default:
                            angle = 180; break;
                        case Direction.Left: // Previously going Left
                            angle = 180 + 45; break;
                        case Direction.Right: //Previously going Right
                            angle = 180 - 45; break;

                    }
                    break;

                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection())  //Currently Going left
                    {
                        default:
                            angle = -90; break;
                        case Direction.Down: // Previously going down
                            angle = -45; break;
                        case Direction.Up: //Previously going up
                            angle = 45;  break;

                    }
                    break;

                case Direction.Right: //Now going to Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90; break;
                        case Direction.Down: // Previously going down
                            angle = 45; break;
                        case Direction.Up:
                            angle = -45;  break;
                           
                    }
                    
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);


        }
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }

    //Handles a single move position of snake
    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if(previousSnakeMovePosition == null)
            {
                return Direction.Right;
                
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
            
        }
    }
}
