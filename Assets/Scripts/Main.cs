using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public static class Main
{
    private static bool _isGameRunning = true;
    private static List<ITickable> _tickable;

    private static bool _isFirstSceneLoad = true;

    private struct Delays
    {
        public float GameTick;
        public float FixedUpdate;

        public Delays(float gameTick, float fixedUpdate)
        {
            GameTick = gameTick;
            FixedUpdate = fixedUpdate;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static async void OnGameLaunched()
    {
        float gameTickDelay = EditorPrefs.GetFloat("GameTickDelay", 0.15f);
        float fixedUpdateDelay = EditorPrefs.GetFloat("FixedUpdateDelay", 0.05f);
        
        Delays delays = new Delays(gameTickDelay, fixedUpdateDelay);

        OnSceneLoaded();

        Task<int> gameTickGenerator = GameTickGenerator(delays);

        Application.quitting += () => _isGameRunning = false;
        await gameTickGenerator;
    }

    public static void OnSceneLoaded()
    {
        ObjectLoader.ClearCachedSceneObjects();
        ObjectLoader.CacheAllSceneObjects();

        Vector2Int boardSize = new Vector2Int(
            EditorPrefs.GetInt("BoardSizeX", 10),
            EditorPrefs.GetInt("BoardSizeY", 18));

        int targetScore = EditorPrefs.GetInt("TargetScore", 85);

        Game game = new Game(boardSize, targetScore);

        GameScore gameScore = new GameScore(game);
        StartScreen startScreen = new StartScreen(game);
        FinalScreen finalScreen = new FinalScreen(game);
        
        if (!_isFirstSceneLoad)
        {
            startScreen.Hide();

            game.Start();
        }

        _tickable = new List<ITickable>()
        {
            game.GameBoard,
        };

        _isFirstSceneLoad = false;
    }
    
    private static async Task<int> GameTickGenerator(Delays delays)
    {
        float fixedUpdatesPerGameTick = delays.GameTick / delays.FixedUpdate;
        
        while (_isGameRunning)
        {
            for (int i = 0; i < fixedUpdatesPerGameTick - 1; ++i)
            {
                foreach (var tickable in _tickable)
                    tickable.FixedUpdate();

                await Task.Delay((int)(delays.FixedUpdate * 1000f));
                if (!_isGameRunning)
                    return 0;
            }

            foreach (var tickable in _tickable)
            {
                tickable.GameTick();
                tickable.FixedUpdate();
            }

            await Task.Delay((int)(delays.FixedUpdate * 1000f));
            if (!_isGameRunning)
                return 0;
        }

        return 0;
    }
}
