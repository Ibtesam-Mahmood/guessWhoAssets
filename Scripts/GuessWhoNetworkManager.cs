using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GuessWhoNetworkManager : NetworkManager
{
    public Transform start;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);

        ////Checks if local player
        //if (player.GetComponent<NetworkIdentity>().isLocalPlayer)
        //{
        //    Destroy(player);
        //    player = GameObject.FindGameObjectWithTag("Player");
        //}

        NetworkServer.AddPlayerForConnection(conn, player);

    }
}
