using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameScore
{
    private Text _playerScore;
    private Text _targetScore;

    private Game _game;

    public GameScore(Game game)
    {
        _game = game;

        _playerScore = ObjectLoader.FindInScene("Player Score")?
            .GetComponent<Text>();
        _targetScore = ObjectLoader.FindInScene("Target Score")?
            .GetComponent<Text>();

        Logger.LogErrorIfNull(_playerScore);
        Logger.LogErrorIfNull(_targetScore);

        _game.OnGameStarted += OnGameStarted;
        _game.OnPlayerScoreUpdated += OnPlayerScoreUpdated;
    }

    private void OnPlayerScoreUpdated()
    {
        _playerScore.text = _game.PlayerScore.ToString();
    }

    private void OnGameStarted()
    {
        _targetScore.text = _game.TargetScore.ToString();
    }
}
