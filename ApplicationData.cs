using UnityEngine;

public static class ApplicationData {

    private static GameModes gameMode = GameModes.Networked;

	public enum GameModes
    {
        Local,
        Networked
    }

    public static GameModes GameMode
    {
        get { return gameMode; }
        set { gameMode = value; }
    }
}
