using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public enum TypeGame { SINGLE, SERVER, HOST, CLIENT }

    [SerializeField] private Dropdown _resolution;
    [SerializeField] private Dropdown _quality;
    [SerializeField] private Toggle _checkbox;

    private Resolution[] _resolutions;
    
    private int _qualityLevel;

    private bool _isFullscreen;
    private int _indexResolution;
    private int _indexQuality;

    public static TypeGame GameType { get; set; }

    private void Start()
    {
        // Resolution Dropdown

        _resolutions = Screen.resolutions;
        _resolution.ClearOptions();

        List<string> optionsResolutions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string optionResolution = _resolutions[i].width + "x" + _resolutions[i].height;
            optionsResolutions.Add(optionResolution);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i - 2;
            }
        }

        _resolution.AddOptions(optionsResolutions);
        _resolution.value = currentResolutionIndex;
        _resolution.RefreshShownValue();

        //End Resolution Dropdown

        // Quality Dropdown

        string[] qualityNames;


        qualityNames = QualitySettings.names;

        _quality.ClearOptions();

        List<string> optionsQuality = new List<string>();

        for (int i = 0; i < qualityNames.Length; i++)
        {
            string option = qualityNames[i];
            optionsQuality.Add(option);
        }

        _quality.AddOptions(optionsQuality);
        _qualityLevel = QualitySettings.GetQualityLevel();
        _quality.value = _qualityLevel;
        _quality.RefreshShownValue();
        
        // END Quality Dropdown

    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        _isFullscreen = isFullscreen;
    }

    public void GetIndexResolution()
    {
        _indexResolution = _resolution.value;
    }

    public void GetIndexQuality()
    {
        _indexQuality = _quality.value;
    }

    public void GetCheckbox()
    {
        _isFullscreen = _checkbox.isOn;
    }

    public void SaveSettings()
    {
        SetResolution(_indexResolution);
        SetQuality(_indexQuality);
        SetFullscreen(_isFullscreen);
    }

    public void StartGame(int typeGame)
    {
        switch (typeGame)
        {
            case 0:
                GameType = TypeGame.SINGLE;
                break;
            case 1:
                GameType = TypeGame.HOST;
                break;
            case 2:
                GameType = TypeGame.SERVER;
                break;
            case 3:
                GameType = TypeGame.CLIENT;
                break;
            default:
                GameType = TypeGame.SINGLE;
                break;
        }
        SceneManager.LoadScene(1);
    
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
