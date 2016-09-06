using UnityEngine;
using System;
using UnityEngine.Audio;

public enum RecallMode{
    inactive, proximity, line, any
}

public enum LineMode {
    active, inactive
}

// This big ol' class is meant to keep track of stuff that scope the whole game.
public class GameManager : MonoBehaviour
{
    protected GameManager() { }

    private static GameManager instance = null;
    private static VolumeControl volumeControl = null;

    // Stats that are not saved in PlayerPrefs go here.
    // This means shit will get refreshed when you change game state.
    [NonSerialized]
    // cut state info
    public Vector2 currentCheckpoint;
    public Vector2 playerCheckpoint;
    public bool cut = false;
    public bool transitioning = false;
    public bool canCut = true;
    public float time;
    public bool canRecall = true;  
    public RecallMode RECALL_MODE;
    public LineMode   LINE_MODE;

    // cut information
    public Vector2 cutStart;
    public Vector2 cutEnd;
    public Vector2 cutOrigin;
    public float cutSpeed;
    public Vector3 direction;

    // game object info
    public GameObject cutter;
    public GameObject player;

    // Stats that are saved in PlayerPrefs
    // This shit is constant everywhere.
    private int levelProgress, achievementShit, highScore; // shit like this

    // Makes sure there's only one instance of the Game Manager.
    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return GameManager.instance;
        }
    }

    // Same with this guy. Only one of him.
    public static VolumeControl VolumeControl
    {
        get
        {
            if (volumeControl == null)
            {
                volumeControl = GameObject.FindObjectOfType<VolumeControl>();
            }
            return volumeControl;
        }
    }

    // Player's current level progress
    // Can only accept new value if it's higher than the old value
    public int LevelProgress
    {
        get
        {
            return levelProgress;
        }
        set
        {
            if (value > levelProgress)
            {
                levelProgress = value;
                PlayerPrefs.SetInt("LevelProgress", levelProgress);
            }
        }
    }


    void Awake()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<GameManager>();
            DontDestroyOnLoad(instance.gameObject);
            canCut = true;
            // playerCheckpoint = new Vector2(9.54f, 5);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // If we ever have stats or achievements,
        // functions for remembering progress will go here
    }

    // And we'd implement ^those functions right here. Nifty.
    void Start() 
    {
        instance.playerCheckpoint = player.transform.position;
    }

    // This will need tweaking depending on how we actually structure games,
    // But this guy will make sure whatever we decide on is ready for a new round.
    public void ResetGame ()
    {
        // dunno how we're doing checkpoints yet but you get what's going on
        // currentCheckpoint = 0;
        cut = false;
        canCut = true;
    }


    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

}