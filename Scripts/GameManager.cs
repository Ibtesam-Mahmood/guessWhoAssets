using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using System;

public enum GameHostState
{
    READY,
    INGAME
}

public class GameManager : NetworkManager
{
    public Transform generalSpawn;
    public List<Transform> transforms;
    public GameObject HUD;
    public Button button;

    private List<GameObject> AI = new List<GameObject>(0);
    private List<GameObject> players = new List<GameObject>(0);

    private GameHostState state;

    
    
    public override void OnStartServer()
    {
        toReadyState();

        //instantiateButton();
    }

    public override void OnServerAddPlayer(NetworkConnection conn){

        if(state == GameHostState.INGAME)
        {
            return;
        }
        
        GameObject player = Instantiate(playerPrefab, generalSpawn.position, generalSpawn.rotation);
        
        players.Add(player);

        NetworkServer.AddPlayerForConnection(conn, player);

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(numPlayers == 0)
        {
            toReadyState();
        }


        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    public void instantiateButton(){
        Button newButton = Instantiate(button) as Button;
        newButton.transform.SetParent(HUD.transform, false);
        //NetworkServer.Spawn(newButton);
        newButton.onClick.AddListener(initialization);
    }

    public void toReadyState()
    {
        state = GameHostState.READY;
        disposeAI();
        initializeAI();
        
    }

    // --- AI control functions ---
    private void initializeAI()
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            Debug.Log("Running");
            GameObject temp = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GuyAI"), transforms[i].position, transforms[i].rotation);
            NetworkServer.Spawn(temp);
            AI.Add(temp);
        }
    }

    private void disposeAI()
    {
        // destroy all AI
        while (AI.Count > 0)
        {
            GameObject temp = AI[0];
            AI.RemoveAt(0);
            Destroy(temp);
        }
    }
}
