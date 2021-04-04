using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameManager : NetworkManager
{
    public List<Transform> transforms;

    private List<GameObject> AI = new List<GameObject>(0);
    
    public override void OnStartServer()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            Debug.Log("Running");
            GameObject temp = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GuyAI"), transforms[i].position, transforms[i].rotation);
            NetworkServer.Spawn(temp);
            AI.Add(temp);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn){

        System.Random r = new System.Random();
        int randomIndex = r.Next(0, transforms.Count);

        GameObject player = Instantiate(playerPrefab, transforms[randomIndex].position, transforms[randomIndex].rotation);

        NetworkServer.AddPlayerForConnection(conn, player);

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(numPlayers == 0)
        {
            // destroy ball
            while(AI.Count > 0)
            {
                GameObject temp = AI[0];
                AI.RemoveAt(0);
                Destroy(temp);
            }
        }


        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }
}
