using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FinalScreen
{
    private GameObject _instance;
    private GameObject _restartButton;

    private Game _game;

    public FinalScreen(Game game)
    {
        _game = game;

        _instance = ObjectLoader.FindInScene("Final Screen");
        _restartButton = ObjectLoader.FindInScene("Restart Button");

        Logger.LogErrorIfNull(_instance);
        Logger.LogErrorIfNull(_restartButton);

        _game.GameBoard.OnGameEnded += OnGameEnded;

        EventTrigger eventTrigger = _restartButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Restart(); });

        eventTrigger.triggers.Add(entry);
        
        _instance.SetActive(false);
    }

    private void OnGameEnded()
    {
        _instance.SetActive(true);
    }

    private void Restart()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);

        SceneManager.sceneLoaded += (scene, loadMode) => Main.OnSceneLoaded();
    }
}
