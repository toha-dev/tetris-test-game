using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : ITickable
{
    private GameObject _instance;
    private GameObject _tilePrefab;

    private Vector2Int _size;
    public Vector2Int Size => _size;

    private GameObject[,] _boardTiles;
    private int[,] _board;

    private GameShape _activeShape = null;
    private Vector2Int _activeShapeBoardPosition;

    private bool _isGameOver = false;
    private bool _isGameStarted = false;

    private KeyDownWrapper _keyADownWrapper = new KeyDownWrapper(KeyCode.A);
    private KeyDownWrapper _keyDDownWrapper = new KeyDownWrapper(KeyCode.D);

    public event Action OnGameEnded;
    public event Action OnLinePopped;

    public GameBoard(Vector2Int size)
    {
        _size = size;

        _instance = ObjectLoader.FindInScene("Game Board");
        _tilePrefab = ObjectLoader.LoadFromResources<GameObject>(
            @"Prefabs\Game Tile");

        Logger.LogErrorIfNull(_instance);
        Logger.LogErrorIfNull(_tilePrefab);

        BuildBoard();
    }

    private void BuildBoard()
    {
        _boardTiles = new GameObject[_size.y, _size.x];
        _board = new int[_size.y, _size.x];

        Vector2 offset = new Vector2(
            (_size.x - 1) * _tilePrefab.transform.localScale.x / 2f, 
            (_size.y - 1) * _tilePrefab.transform.localScale.y / 2f);

        for (int y = 0; y < _size.y; ++y)
        {
            for (int x = 0; x < _size.x; ++x)
            {
                GameObject tile = GameObject.Instantiate(_tilePrefab);

                tile.transform.SetParent(_instance.transform);
                tile.transform.localPosition = new Vector3(
                    (x * _tilePrefab.transform.localScale.x - offset.x), 
                    (y * _tilePrefab.transform.localScale.y - offset.y), 
                    0f);

                _boardTiles[y, x] = tile;
            }
        }
    }

    public void Start()
    {
        _isGameStarted = true;
    }

    private T SelectRandomEnumElement<T>()
    {
        Array values = Enum.GetValues(typeof(T));

        return (T)values.GetValue(
            UnityEngine.Random.Range(0, values.Length));
    }

    private bool TryGenerateRandomShape()
    {
        if (_activeShape != null)
            GameObject.Destroy(_activeShape.Instance.gameObject);
        
        _activeShape = new GameShape(SelectRandomEnumElement<GameShapeType>());
        _activeShape.SetOrientation(SelectRandomEnumElement<GameShapeOrientation>());

        GameObject instance = _activeShape.Instance;
        
        instance.transform.SetParent(_instance.transform);
        UpdateActiveShapePosition(new Vector2Int(
            _size.x / 2 - _activeShape.Size / 2,
            _size.y - 1));

        return CheckIfActiveShapeCanSpawn();
    }

    private bool CheckIfActiveShapeCanSpawn()
    {
        return CheckIfCanMoveWithOffset(0, 0);
    }

    private void UpdateActiveShapePosition(Vector2Int newPosition)
    {
        _activeShapeBoardPosition = newPosition;

        _activeShape.Instance.transform.position =
                _boardTiles[_activeShapeBoardPosition.y, _activeShapeBoardPosition.x]
                .transform.position;
    }

    public void GameTick()
    {
        if (_isGameOver || !_isGameStarted)
            return;

        if (_activeShape == null)
        {
            if (!TryGenerateRandomShape())
                GameOver();
        }
        else
        {
            if (!TryFallDownActiveShape())
            {
                CombineShapeWithBoard();
                TryRemoveCompletedLines();

                if (!TryGenerateRandomShape())
                    GameOver();
            }
        }
    }

    public void StopGame()
    {
        GameOver();
    }

    private void GameOver()
    {
        _isGameOver = true;

        OnGameEnded?.Invoke();
    }

    public void FixedUpdate()
    {
        if (_isGameOver || !_isGameStarted)
            return;

        if (_keyADownWrapper.IsKeyDown())
            TryMoveLeft();

        if (_keyDDownWrapper.IsKeyDown())
            TryMoveRight();
    }

    private bool TryMoveLeft()
    {
        if (_activeShapeBoardPosition.x <= 0)
            return false;

        if (!CheckIfCanMoveWithOffset(-1, 0))
            return false;

        UpdateActiveShapePosition(new Vector2Int(
            _activeShapeBoardPosition.x - 1, _activeShapeBoardPosition.y));

        return true;
    }

    private bool TryMoveRight()
    {
        if (!CheckIfCanMoveWithOffset(1, 0))
            return false;

        UpdateActiveShapePosition(new Vector2Int(
            _activeShapeBoardPosition.x + 1, _activeShapeBoardPosition.y));

        return true;
    }

    private bool CheckIfCanMoveWithOffset(int xOffset, int yOffset)
    {
        for (int y = 0; y < _activeShape.Size; ++y)
        {
            for (int x = 0; x < _activeShape.Size; ++x)
            {
                if (_activeShape.Body[y, x] != 1)
                    continue;

                int boardY = _activeShapeBoardPosition.y - y + yOffset;
                int boardX = _activeShapeBoardPosition.x + x + xOffset;

                if (boardY < 0 || boardY >= _size.y)
                    return false;

                if (boardX < 0 || boardX >= _size.x)
                    return false;

                if (_board[boardY, boardX] == 1)
                    return false;
            }
        }

        return true;
    }

    private bool TryFallDownActiveShape()
    {
        if (!CheckIfCanMoveWithOffset(0, -1))
            return false;

        UpdateActiveShapePosition(new Vector2Int(
            _activeShapeBoardPosition.x, _activeShapeBoardPosition.y - 1));

        return true;
    }

    private void CombineShapeWithBoard()
    {
        for (int y = 0; y < _activeShape.Size; ++y)
        {
            for (int x = 0; x < _activeShape.Size; ++x)
            {
                if (_activeShape.Body[y, x] != 1)
                    continue;

                int boardY = _activeShapeBoardPosition.y - y;
                int boardX = _activeShapeBoardPosition.x + x;

                _board[boardY, boardX] = 1;
                CopySpriteRendererParameters(
                    GetTileSpriteRendererObject(_activeShape.ShapeTilePrefab),
                    GetTileSpriteRendererObject(_boardTiles[boardY, boardX]));
            }
        }
    }

    private bool TryRemoveCompletedLines()
    {
        for (int y = 0; y < _size.y; ++y)
        {
            bool completedLine = true;
            for (int x = 0; x < _size.x; ++x)
            {
                if (_board[y, x] != 1)
                {
                    completedLine = false;
                    break;
                }
            }

            if (completedLine)
                RemoveCompletedLineFromY(y--);
        }

        return true;
    }

    private void RemoveCompletedLineFromY(int fromY)
    {
        for (int y = fromY; y < _size.y - 1; ++y)
        {
            for (int x = 0; x < _size.x; ++x)
            {
                _board[y, x] = _board[y + 1, x];

                CopySpriteRendererParameters(
                    GetTileSpriteRendererObject(_boardTiles[y + 1, x]),
                    GetTileSpriteRendererObject(_boardTiles[y, x]));
            }
        }

        OnLinePopped?.Invoke();
    }

    private void CopySpriteRendererParameters(GameObject from, GameObject to)
    {
        SpriteRenderer fromSpireRenderer =
            from.GetComponent<SpriteRenderer>();

        SpriteRenderer toSpireRenderer =
            to.GetComponent<SpriteRenderer>();

        toSpireRenderer.sprite = fromSpireRenderer.sprite;
        toSpireRenderer.color = fromSpireRenderer.color;
    }

    private GameObject GetTileSpriteRendererObject(GameObject parent)
    {
        return parent?.transform.GetChild(0)?.gameObject;
    }
}
