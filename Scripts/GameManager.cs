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

    private Dictionary<int, GameObject> characterCounts = new Dictionary<int, GameObject>(0);

    protected GameObject target;

    private List<GameObject> AI = new List<GameObject>(0);
    private List<NetworkGuy> players = new List<NetworkGuy>(0);

    private GameHostState state;

    private GameObject hunter;
    private Button _button;
    bool serverInitialized = false;

    public override void OnStartServer()
    {
        toReadyState();

        serverInitialized = true;
    }

    public override void OnServerAddPlayer(NetworkConnection conn){

        if(state == GameHostState.INGAME)
        {
            return;
        }
        
        GameObject player = Instantiate(playerPrefab, generalSpawn.position, generalSpawn.rotation);

        NetworkGuy guy = player.GetComponent<NetworkGuy>();

        players.Add(guy);

        NetworkServer.AddPlayerForConnection(conn, player);

        //Assign role
        if (numPlayers == 1)
        {

            hunter = player;

            //Hunter
            guy.CmdSetRole(NetworkGuy.GameRole.Hunter);
        }
        else
        {
            //Hider
            guy.CmdSetRole(NetworkGuy.GameRole.Hider);
        }

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(numPlayers == 0)
        {
            toReadyState();
        }

        players.Remove(conn.identity.GetComponent<NetworkGuy>());

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

    /*
    public void instantiateKillButton(){
        _button = button;
        Button newButton = Instantiate(_button) as Button;
        newButton.transform.SetParent(InventoryCanvas.transform, false);
    }*/

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
