using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CodeMonkey;

public class LevelGrid {

    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;

    public LevelGrid(int width, int height) {
        this.width = width;
        this.height = height;
    }

    public void Setup(Snake snake) {
        this.snake = snake;
        
        SpawnFood();
    }

    private void SpawnFood() {
        // Create a range of valid grid indices (i.e: grid spaces not occupied by the snake)
        ISet<int> exclude = new HashSet<int>();
        foreach (Vector2Int position in snake.GetFullSnakePositionList()) {
            exclude.Add(ConvertPositionToIndex(position));
        }
        IEnumerable<int> range = Enumerable.Range(0, width*height).Where(i => !exclude.Contains(i));

        // Pick a random element of the range for food position
        int foodIndex = range.ElementAt(Random.Range(0, (width * height) - exclude.Count));
        foodGridPosition = ConvertIndexToPosition(foodIndex);

        // Create food gameObject
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition) {
        // If snake head is on top of food, consume food
        if (snakeGridPosition == foodGridPosition) {
            Object.Destroy(foodGameObject);
            SpawnFood();    // Spawn new food
            Score.IncrementScore();
            return true;
        }
        else {
            return false;
        }
    }

    public Vector2Int GetDimensions() {
        return new Vector2Int(width, height);
    }

    private int ConvertPositionToIndex(Vector2Int position) {
        return (width * position.x) + position.y;
    }

    private Vector2Int ConvertIndexToPosition(int index) {
        int x = index / width;
        int y = index % width;
        return new Vector2Int(x, y);
    }
}
