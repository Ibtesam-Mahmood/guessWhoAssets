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

    protected GameObject target;

    private List<GameObject> ai = new List<GameObject>(0);
    private List<NetworkGuy> players = new List<NetworkGuy>(0);

    public GameObject killButton;

    public override void Awake()
    {
        killButton = GameObject.FindGameObjectWithTag("Chooser");
    }

    public List<NetworkGuy> Players 
    {
        get {
            return players;
        }
    }
    public List<GameObject> AI
    {
        get
        {
            return ai;
        }
    }

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

        //Assign role
        if (numPlayers == 0)
        {

            hunter = player;

            //Hunter
            guy.role = NetworkGuy.GameRole.Hunter;
        }
        else
        {
            //Hider
            guy.role = NetworkGuy.GameRole.Hider;
        }


        NetworkServer.AddPlayerForConnection(conn, player);


    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        players.Remove(conn.identity.GetComponent<NetworkGuy>());

        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);

        if (numPlayers == 0)
        {
            toReadyState();
        }
        else if(players[0].role != NetworkGuy.GameRole.Hunter)
        {
            Debug.Log(players[0].role);
            players[0].role = NetworkGuy.GameRole.Hunter;
        }
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
            GameObject temp = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GuyAI"), transforms[i].position, transforms[i].rotation);
            NetworkServer.Spawn(temp);
            ai.Add(temp);
        }
    }

    private void disposeAI()
    {
        // destroy all AI
        while (ai.Count > 0)
        {
            GameObject temp = ai[0];
            ai.RemoveAt(0);
            Destroy(temp);
        }
    }
}
