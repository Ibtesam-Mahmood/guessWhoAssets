using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGuy : NetworkBehaviour
{

    public enum GameRole
    {
        Hunter,
        Hider,
        None
    }

    [SyncVar(hook = nameof(SetRole))]
    public GameRole role = GameRole.None;

    [SyncVar(hook = nameof(SetGuess))]
    public int guessCount = 3;

    //Guess Button
    private GameObject button;

    //Distance for the guess
    public float distance = 10f;

    private void Start()
    {

        if (isLocalPlayer)
        {
            button = GameObject.FindGameObjectWithTag("Chooser");
            SetButton();
            Character character = this.GetComponent<Character>();

            MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, character);
            MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
            MMGameEvent.Trigger("CameraBound");
        }
        else
        {
            var characterAbilities = this.GetComponents<CharacterAbility>();
            foreach (CharacterAbility ability in characterAbilities)
            {
                ability.enabled = false;
            }

            this.gameObject.layer = 13;
            this.gameObject.tag = "Enemy";
        }
    }

    void SetRole(GameRole oldRole, GameRole newRole)
    {
        SetButton();
    }

    void SetGuess(int oldGuess, int newGuess)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Guesses " + newGuess);
            Health health = this.gameObject.GetComponent<Health>();
            health.SetHealth(health.MaximumHealth * newGuess / 3);
            if(newGuess == 0)
            {
                NetworkServer.Destroy(this.gameObject);
                (NetworkManager.singleton as GameManager).StopClient();
            }
        }
    }

    private void OnConnectedToServer()
    {
        SetButton();
    }


    [Command(requiresAuthority = false)]
    public void CmdSetRole(GameRole role)
    {
        this.role = role;
    }

    [Command(requiresAuthority = false)]
    public void CmdKill()
    {
        Debug.Log("Killing with " + this.netId);
        List<NetworkGuy> players = (NetworkManager.singleton as GameManager).Players;

        List<GameObject> searchedObjects = (NetworkManager.singleton as GameManager).AI;

        float minIndexAI = -1;
        float smallestDistanceAI = distance;

        Vector3 position = this.gameObject.transform.position;
        foreach (GameObject enemy in searchedObjects)
        {
            try
            {
                float curDistance = Vector3.Distance(enemy.transform.position, position);
                if (curDistance <= distance)
                {
                    if (curDistance <= smallestDistanceAI)
                    {
                        smallestDistanceAI = curDistance;
                        minIndexAI = searchedObjects.IndexOf(enemy);
                    }

                }
            }catch(Exception)
            {

            }
        }

        float minIndexPlayers = -1;
        float smallestDistancePlayers = distance;

        foreach (NetworkGuy netGuy in players)
        {
            if(this.netId != netGuy.netId)
            {
                float curDistance = Vector3.Distance(netGuy.gameObject.transform.position, position);
                if(curDistance <= distance)
                {
                    if(curDistance <= smallestDistancePlayers)
                    {
                        smallestDistancePlayers = curDistance;
                        minIndexPlayers = players.IndexOf(netGuy);
                    }
                }
            }
        }

        /*
        if (minIndexPlayers > 0 && smallestDistanceAI <= )
        {
        }
        else if(minIndexAI > 0)
        {

        }

        GameObject e = searchedObjects[minIndex];
        

        NetworkGuy guy = e.GetComponent<NetworkGuy>();

        if (guy == null)
        {


            //Bad aim
            Debug.Log("AI");
            this.guessCount -= 1;

        }
        else
        {
            Debug.Log("ThisID: " + this.netId);
            Debug.Log("OtherID: " + guy.netId);
            //killed client
            Debug.Log("Player");
            this.guessCount += 1;
            guy.guessCount = 0;
        }
        */

    }



    private void SetButton()
    {
        
        if(button == null)
        {
            return;
        }

        if(isLocalPlayer && role == GameRole.Hunter)
        {
            Debug.Log("Button Active " + button.name);
            button.SetActive(true);
            Button innerButton = button.GetComponent<Button>();
            Debug.Log(innerButton.gameObject.name);
            innerButton.onClick.RemoveAllListeners();
            innerButton.onClick.AddListener(CmdKill);
        }
        else
        {
            Debug.Log("Button Deactive");
            button.SetActive(false);
        }
    }
    

}