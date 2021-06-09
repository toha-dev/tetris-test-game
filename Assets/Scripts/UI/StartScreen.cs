using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScreen
{
    private GameObject _instance;
    private GameObject _startButton;

    private Game _game;

    public StartScreen(Game game)
    {
        _game = game;

        _instance = ObjectLoader.FindInScene("Start Screen");
        _startButton = ObjectLoader.FindInScene("Start Button");

        Logger.LogErrorIfNull(_instance);
        Logger.LogErrorIfNull(_startButton);
        
        EventTrigger eventTrigger = _startButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Start(); });

        eventTrigger.triggers.Add(entry);
    }
    
    public void Hide()
    {
        _instance.SetActive(false);
    }

    private void Start()
    {
        Hide();

        _game.Start();
    }
}
