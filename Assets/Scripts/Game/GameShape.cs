using System;
using System.Collections.Generic;
using UnityEngine;

public class GameShape
{
    private GameObject _instance;
    private GameObject _shapePrefab;
    private GameObject _shapeTilePrefab;

    public GameObject Instance => _instance;
    public GameObject ShapeTilePrefab => _shapeTilePrefab;

    private GameObject[] _drawedTiles;
    
    private GameShapeType _type;
    private GameShapeOrientation _orientation;

    private int[,] _body;

    public int[,] Body => _body;
    public int Size => GameShapesData.BodySize;
    
    private static class GameShapesData
    {
        public const int BodySize = 3;
        public static readonly Dictionary<GameShapeType, int[,]> NorthOrientedShapes =
            new Dictionary<GameShapeType, int[,]>()
            {
                {GameShapeType.I_BLOCK, new int[BodySize, BodySize] {
                    { 1, 0, 0 },
                    { 1, 0, 0 },
                    { 1, 0, 0 },
                }},

                {GameShapeType.O_BLOCK, new int[BodySize, BodySize]
                {
                    { 1, 1, 0 },
                    { 1, 1, 0 },
                    { 0, 0, 0 },
                }},
            };

        public static Vector2Int TransformToOrientaton(
            GameShapeOrientation targetOrientation, Vector2Int startPosition)
        {
            Vector2Int result = new Vector2Int(0, 0);

            switch (targetOrientation)
            {
                case GameShapeOrientation.NORTH:
                    result.y = startPosition.y;
                    result.x = startPosition.x;
                    break;
                case GameShapeOrientation.EAST:
                    result.y = startPosition.x;
                    result.x = BodySize - startPosition.y - 1;
                    break;
                case GameShapeOrientation.SOUTH:
                    result.y = BodySize - startPosition.y - 1;
                    result.x = BodySize - startPosition.x - 1;
                    break;
                case GameShapeOrientation.WEST:
                    result.y = BodySize - startPosition.x - 1;
                    result.x = startPosition.y;
                    break;
                default:
                    throw new NotImplementedException(
                        "GameShapeOrientation doesn't " +
                        "contain this type of orientation.");
            }

            return result;
        }
    }

    public GameShape(GameShapeType type)
    {
        _shapePrefab = ObjectLoader.LoadFromResources<GameObject>(
            @"Prefabs\Game Shape");
        _shapeTilePrefab = ObjectLoader.LoadFromResources<GameObject>(
            @"Prefabs\Shape Tile");

        _instance = GameObject.Instantiate(_shapePrefab);

        Logger.LogErrorIfNull(_shapePrefab);
        Logger.LogErrorIfNull(_shapeTilePrefab);
        Logger.LogErrorIfNull(_instance);

        _type = type;
        _body = GameShapesData.NorthOrientedShapes[type];

        UpdateView();
    }

    public void SetOrientation(GameShapeOrientation targetOrientation)
    {
        ClearView();

        _body = new int[GameShapesData.BodySize, GameShapesData.BodySize];

        for (int y = 0; y < GameShapesData.BodySize; ++y)
        {
            for (int x = 0; x < GameShapesData.BodySize; ++x)
            {
                Vector2Int targetPosition = GameShapesData.TransformToOrientaton(
                    targetOrientation, new Vector2Int(x, y));

                _body[targetPosition.y, targetPosition.x] = 
                    GameShapesData.NorthOrientedShapes[_type][y, x];
            }
        }

        MoveToTopLeftCorner();
        UpdateView();
    }

    private void MoveToTopLeftCorner()
    {
        Vector2Int moveOffset = new Vector2Int(0, 0);

        for (int y = 0; y < GameShapesData.BodySize; ++y)
        {
            bool emptyRow = true;
            for (int x = 0; x < GameShapesData.BodySize; ++x)
            {
                if (_body[y, x] == 1)
                {
                    emptyRow = false;
                    break;
                }
            }

            if (emptyRow)
                moveOffset.y++;
            else
                break;
        }

        for (int x = 0; x < GameShapesData.BodySize; ++x)
        {
            bool emptyColumn = true;
            for (int y = 0; y < GameShapesData.BodySize; ++y)
            {
                if (_body[y, x] == 1)
                {
                    emptyColumn = false;
                    break;
                }
            }

            if (emptyColumn)
                moveOffset.x++;
            else
                break;
        }

        if (moveOffset.x != 0 || moveOffset.y != 0)
        {
            for (int y = moveOffset.y; y < GameShapesData.BodySize; ++y)
            {
                for (int x = moveOffset.x; x < GameShapesData.BodySize; ++x)
                {
                    _body[y - moveOffset.y, x - moveOffset.x] = _body[y, x];
                    _body[y, x] = 0;
                }
            }
        }
    }

    private void UpdateView()
    {
        if (_drawedTiles != null)
        {
            ClearView();
        }
        else
        {
            _drawedTiles = new GameObject[
                GameShapesData.BodySize * GameShapesData.BodySize];
        }

        for (int y = 0; y < GameShapesData.BodySize; ++y)
        {
            for (int x = 0; x < GameShapesData.BodySize; ++x)
            {
                if (_body[y, x] == 1)
                {
                    GameObject tile = GameObject.Instantiate(_shapeTilePrefab);

                    tile.transform.SetParent(_instance.transform);
                    tile.transform.position = new Vector3(
                        _instance.transform.position.x + x * _shapeTilePrefab.transform.localScale.x,
                        _instance.transform.position.y - y * _shapeTilePrefab.transform.localScale.y,
                        -1f);

                    _drawedTiles[GameShapesData.BodySize * y + x] = tile;
                }
            }
        }
    }

    private void ClearView()
    {
        for (int i = 0; i < _drawedTiles.Length; ++i)
        {
            if (_drawedTiles[i] != null)
            {
                GameObject.Destroy(_drawedTiles[i]);

                _drawedTiles[i] = null;
            }
        }
    }
}

public enum GameShapeType
{
    I_BLOCK,
    O_BLOCK,
}

public enum GameShapeOrientation
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
}