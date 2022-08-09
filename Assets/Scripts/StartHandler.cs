using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartHandler : MonoBehaviour
{
    private string _address;
    private string _port;

    private NetworkManager _networkManager;

    private void Awake()
    {
        //Menu.GameType = Menu.TypeGame.SINGLE;

        _networkManager = GameObject.FindObjectOfType<NetworkManager>();
    }

    private void Start()
    {
        switch (Menu.GameType)
        {
            case Menu.TypeGame.SINGLE:
                StartHost();
                break;
            case Menu.TypeGame.HOST:
                StartHost();
                break;
            case Menu.TypeGame.SERVER:
                StartServer();
                break;
            case Menu.TypeGame.CLIENT:
                StartClient();
                break;
        }
        
    }

    private void StartHost()
    {
        if (!NetworkClient.active)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                _networkManager.StartHost();
            }
        }
        else
        {
            _networkManager.StopClient();
        }
    }

    private void StartServer()
    {
        if (!NetworkClient.active)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // WebGL cannot be server
            }
            else
            {
                _networkManager.StartServer();
            }
        }
        else
        {
            _networkManager.StopClient();
        }
    }

    private void StartClient()
    {
        if (!NetworkClient.active)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                _address = Menu.Address;

                _networkManager.networkAddress = _address;

                _networkManager.StartClient();
            }
        }
        else
        {
            _networkManager.StopClient();
        }
    }
}
