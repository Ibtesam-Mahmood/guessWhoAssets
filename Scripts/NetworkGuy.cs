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
            button = (NetworkManager.singleton as GameManager).killButton;
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

        List<GameObject> ais = (NetworkManager.singleton as GameManager).AI;

        List<dynamic> search = new List<dynamic>(0);

        foreach(NetworkGuy netGuy in players)
        {
            if(this.netId != netGuy.netId)
            {
                search.Add(netGuy);
            }
        }
        foreach(GameObject ai in ais)
        {
            search.Add(ai);
        }

        dynamic closest = null;
        float smallestDistance = distance;

        Vector3 position = this.gameObject.transform.position;
        foreach (dynamic enemy in search)
        {
            try
            {
                Vector3 enemyPos;
                if(enemy is NetworkGuy)
                {
                    enemyPos = ((NetworkGuy)enemy).gameObject.transform.position;
                }
                else
                {
                    enemyPos = ((GameObject)enemy).transform.position;
                }
                float curDistance = Vector3.Distance(enemyPos, position);
                if (curDistance <= distance)
                {
                    if (curDistance <= smallestDistance)
                    {
                        smallestDistance = curDistance;
                        closest = enemy;
                    }

                }
            }
            catch (Exception)
            {

            }
        }

        if (closest == null)
            return;

        
        if(closest is NetworkGuy)
        {
            NetworkGuy guy = (NetworkGuy)closest;
            Debug.Log("Player");
            Debug.Log("ThisID: " + this.netId);
            Debug.Log("OtherID: " + guy.netId);
            if(this.netId != guy.netId) {
                this.guessCount += 1;
                guy.guessCount = 0;
            }
        }
        else
        {
            //Bad aim
            Debug.Log("AI");
            this.guessCount -= 1;
        }

        

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