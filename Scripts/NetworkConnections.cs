using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkConnections : MonoBehaviour
{
    NetworkManager manager;
    public string IP = "35.221.13.249";
    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
        manager.networkAddress = IP;
    }
    public void connectClient()
    {
        if(!NetworkClient.isConnected && !NetworkClient.ready)
        {
            manager.StartClient();
        }
        
    }

    public void disconnectClient()
    {
        if(NetworkClient.isConnected && NetworkClient.ready)
        {
            manager.StopClient();
        }
        
    }
}
