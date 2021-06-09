using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Game
{
    private GameBoard _gameBoard;
    public GameBoard GameBoard => _gameBoard;

    private int _playerScore = 0;
    private int _targetScore = 0;

    public int PlayerScore => _playerScore;
    public int TargetScore => _targetScore;

    public event Action OnPlayerScoreUpdated;
    public event Action OnGameStarted;

    public Game(Vector2Int size, int targetScore)
    {
        _gameBoard = new GameBoard(size);
        _targetScore = targetScore;
        
        _gameBoard.OnLinePopped += OnLinePopped;
    }

    public void Start()
    {
        _gameBoard.Start();

        OnGameStarted?.Invoke();
    }
   
    private void OnLinePopped()
    {
        _playerScore += _gameBoard.Size.x;
        OnPlayerScoreUpdated?.Invoke();

        if (_playerScore >= _targetScore)
            _gameBoard.StopGame();
    }
}
