using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using System;

public class GameManager : NetworkManager
{
    public Transform generalSpawn;
    public List<Transform> transforms;
    public GameObject HUD;
    public Button button;

    private List<GameObject> AI = new List<GameObject>(0);
    private List<GameObject> players = new List<GameObject>(0);
    
    public override void OnStartServer()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            Debug.Log("Running");
            GameObject temp = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GuyAI"), transforms[i].position, transforms[i].rotation);
            NetworkServer.Spawn(temp);
            AI.Add(temp);
        }

        instantiateButton();
    }

    public override void OnServerAddPlayer(NetworkConnection conn){
        
        GameObject player = Instantiate(playerPrefab, generalSpawn.position, generalSpawn.rotation);
        
        players.Add(player);

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

    public void instantiateButton(){
        Button newButton = Instantiate(button) as Button;
        newButton.transform.SetParent(HUD.transform, false);
        newButton.onClick.AddListener(initialization);
    }

    public void initialization(){
        
    }
}
