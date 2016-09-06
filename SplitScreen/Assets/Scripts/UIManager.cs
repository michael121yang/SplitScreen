using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


// The UIManager controls which UI elements are
// visible at any given time.
public class UIManager : MonoBehaviour
{
	// The various UI groups. These remain serialized
	// so that they can be attached via the inspector.
	// I'm assuming we'll have these four eventually.
	// Especially the last one. *war flashbacks*
	public GameObject
		IngameGUI,
		Settings,
		PauseMenu,
		LoadingScreen;
	
	private static UIManager instance;
	
	public static UIManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<UIManager>();
			}
			return instance;
		}
	}
	
	
	void Start ()
	{
	
		// this is probaly where we initialize shit for when we have different levels?
		// I don't know! new and fun things!!
		
	}
	
	
	public void LoadScene(string name)
	{
			GameManager.Instance.ResetGame();
			SceneManager.LoadScene(name);
		
	}
	
	/*
	public void Continue ()
	{
		Results.SetActive(false);
		IngameGUI.SetActive(true);
	}*/

	/*
	// For when we have a settings menu
	// And want to show it off
	public void ShowSettings (bool show = true)
	{
		if (IngameGUI != null)
		{
			PauseMenu.SetActive(!show);
		}
		Settings.SetActive(show);
	}
	*/

	/*
	// For when we have a pause menu
	// and want it to be alone on screen
	public void ShowPauseMenu (bool show = true)
	{
		PauseMenu.SetActive(show);
		IngameGUI.SetActive(!show);
		// pile currenly active GUI elements onto the last open list
		lastOpenUI.Clear();
		lastOpenUI.Add(PauseMenu);
		paused = show;
	}
	*/

	// WE ARE GETTING A LOADING SCREEN. IT WILL BE GREAT.
	// Planning on having it live in the main menu as a hidden UI element
	// And then call this when we wanna change scenes from Main Menu
	public void ShowLoadingScreen (bool show = true)
	{
		LoadingScreen.SetActive(show);
	}

	// Yup
	/*
	public void LevelSelect (int mode)
	{
		GameManager.Instance.TimedMode = mode;
	}
	*/
	
	//for cute wee sliders
	public void SFXSlider (float volume)
	{
		GameManager.VolumeControl.SetSFXVolume(volume, false);
	}
	
	
	public void MusicSlider (float volume)
	{
		GameManager.VolumeControl.SetMusicVolume(volume, false);
	}
	
	// for muting SFX
	public void SFXToggle (bool on)
	{
		GameManager.VolumeControl.SetSFXVolume(!on);
	}
	
	// for muting music
	public void MusicToggle (bool on)
	{
		GameManager.VolumeControl.SetMusicVolume(!on);
	}
	

	// Start over everything.
	// We can add more shit here for debugging purposes.
	// Gonna just call this from some button on main menu probably.
	public void ResetData ()
	{
		PlayerPrefs.DeleteAll();
	}

	/*
	// iterate over the list of lastOpenUI and set their active property to show
	// <param name="show"></param>
	void lastOpenUISetActive(bool show)
	{
		foreach (GameObject go in lastOpenUI)
		{
			go.SetActive(show);
		}
	}*/
}
