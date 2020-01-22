using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }

    private enum State {
        Alive,
        Dead
    }

    private State state;
    private Vector2Int headPosition;    // Position of snake head
    private Direction headDirection;   // Direction of snake head
    private Direction lastestDirectionInput;
    private float gridMoveTimer;  // Time until next movement
    private float gridMoveTimerMax; // Amount of time between moves
    private LevelGrid levelGrid;    // Reference to current level grid
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
    }

    private void Awake() {
        headPosition = new Vector2Int(10,10);   // Head starts at center
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        headDirection = Direction.Right;   // Snake moves to the right at start
        lastestDirectionInput = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }
    
    private void Update() {
        switch (state) {
            case State.Alive:
                HandleInput();
                HandleMovement();
                break;
            case State.Dead:
                break;
        }
    }
    
    private void HandleInput() {
        // Change snake's direction on arrow key press
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (headDirection != Direction.Down) {    // Do not turn if currently going down
                lastestDirectionInput = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (headDirection != Direction.Up) {    // Do not turn if currently going up
                lastestDirectionInput = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (headDirection != Direction.Right) {    // Do not turn if currently going right
                lastestDirectionInput = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            if (headDirection != Direction.Left) {    // Do not turn if currently going left
                lastestDirectionInput = Direction.Right;
            }
        }
    }

    public void HandleMovement() {
        gridMoveTimer += Time.deltaTime;   // Update timer with time since last update

        if (gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;  // Refresh move timer
            headDirection = lastestDirectionInput;  // Set new head direction to latest direction input

            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);

            // Set current head position as first body position, if any body parts exist
            SnakeMovePosition prevSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0) {
                prevSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(prevSnakeMovePosition, headPosition, headDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            // Update position of head
            Vector2Int directionVector;
            switch (headDirection) {
                default:
                case Direction.Right:   directionVector = new Vector2Int(+1, 0); break;
                case Direction.Left:    directionVector = new Vector2Int(-1, 0); break;
                case Direction.Up:      directionVector = new Vector2Int(0, +1); break;
                case Direction.Down:    directionVector = new Vector2Int(0, -1); break;
            }
            headPosition += directionVector;

            // Grow snake if food was eaten
            bool snakeAteFood = levelGrid.TrySnakeEatFood(headPosition);
            if (snakeAteFood) {
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }
            // Delete the end of snake body when snake moves
            if (snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            // Apply position shift and rotate snake head
            transform.position = new Vector3(headPosition.x, headPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(directionVector) - 90);

            UpdateSnakeBodyParts();

            // Test if snake head hits wall
            Vector2Int gridDimensions = levelGrid.GetDimensions();
            if (headPosition.x < 0 || headPosition.x > gridDimensions.x - 1 ||
                headPosition.y < 0 || headPosition.y > gridDimensions.y - 1) {
                KillSnake();
            }

            // Test if snake head intersects body
            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList) {
                Vector2Int snakeBodyPartPosition = snakeBodyPart.GetGridPosition();
                if (headPosition == snakeBodyPartPosition) {
                    KillSnake();
                }
            }
        }
    }

    /*
     *  Kills the snake
     * */
    private void KillSnake() {
        state = State.Dead;
        GameHandler.SnakeDied();
        SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
    }

    /*
     *  Add a new snake body part into tracking list
     * */
    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    /*
     *  Update the positions of each body part
     * */
    private void UpdateSnakeBodyParts() {
        for (int i = 0; i < snakeBodyPartList.Count; i++) {
            snakeBodyPartList[i].SetMovePosition(snakeMovePositionList[i]);
        }
    }

    private float GetAngleFromVector(Vector2Int direction) {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    // Return a list of positions for all parts of the snake (head and body)
    public List<Vector2Int> GetFullSnakePositionList() {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { headPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList) {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    /*
     *  Nested class to handle snake body parts
     * */
    private class SnakeBodyPart {

        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetMovePosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(
                snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection()) {
                default:
                case Direction.Up:      // Currently going up
                    switch (snakeMovePosition.GetPrevDirection()) {
                        default: angle = 0; break;
                        case Direction.Right: angle = 0 - 45; break;        // Previously going right
                        case Direction.Left: angle = 0 + 45; break;         // Previously going left
                    }
                    break;

                case Direction.Down:    // Currently going down
                    switch (snakeMovePosition.GetPrevDirection()) {
                        default: angle = 180; break;
                        case Direction.Right: angle = 180 + 45; break;      // Previously going right
                        case Direction.Left: angle = 180 - 45; break;       // Previously going left
                    }
                    break;

                case Direction.Left:    // Currently going left
                    switch (snakeMovePosition.GetPrevDirection()) {
                        default: angle = 90; break;
                        case Direction.Up: angle = 90 - 45; break;          // Previously going up
                        case Direction.Down: angle = 90 + 45; break;        // Previously going down
                    }
                    break;

                case Direction.Right:   // Currently going right
                    switch (snakeMovePosition.GetPrevDirection()) {
                        default:    angle = -90; break;
                        case Direction.Up:      angle = -90 + 45; break;    // Previously going up
                        case Direction.Down:    angle = -90 - 45; break;    // Previously going down
                    }
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition() {
            return snakeMovePosition.GetGridPosition();
        }
    }

    /*
     *  Handles one move position of the snake
     * */
    private class SnakeMovePosition {

        private SnakeMovePosition prevMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition prevMovePosition, Vector2Int gridPosition, Direction direction) {
            this.prevMovePosition = prevMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() {
            return gridPosition;
        }

        public Direction GetDirection() {
            return direction;
        }

        public Direction GetPrevDirection() {
            if (prevMovePosition == null) {
                return Direction.Right; // Default direction
            }

            return prevMovePosition.GetDirection();
        }
    }
}
