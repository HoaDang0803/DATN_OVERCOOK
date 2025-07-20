using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    
    private void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;
        Hide();
    }
    private void NetworkManager_OnConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        if (eventData.ClientId == networkManager.LocalClientId && eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}