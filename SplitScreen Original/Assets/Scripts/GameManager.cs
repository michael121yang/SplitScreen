using UnityEngine;
using System;
using UnityEngine.Audio;



// This big ol' class is meant to keep track of stuff that scope the whole game.
public class GameManager : MonoBehaviour
{
    protected GameManager() { }

    private static GameManager instance = null;
    private static VolumeControl volumeControl = null;

    // Stats that are not saved in PlayerPrefs go here.
    // This means shit will get refreshed when you change game state.
    [NonSerialized]
    public int currentCheckpoint;
    public bool cut;
    public bool canCut;
    public float time;
    public GameObject cutter;
    public Vector3 direction;

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
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // If we ever have stats or achievements,
        // functions for remembering progress will go here
    }

    // And we'd implement ^those functions right here. Nifty.


    // This will need tweaking depending on how we actually structure games,
    // But this guy will make sure whatever we decide on is ready for a new round.
    public void ResetGame ()
    {
        // dunno how we're doing checkpoints yet but you get what's going on
        currentCheckpoint = 0;
        cut = false;
        canCut = true;
    }


    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

}