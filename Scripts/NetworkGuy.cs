using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
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
            Health health = this.gameObject.GetComponent<Health>();
            health.SetHealth(health.MaximumHealth * newGuess / 3);
            if(guessCount == 0)
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
        List<NetworkGuy> players = (NetworkManager.singleton as GameManager).Players;

        players.Remove(this);

        List<GameObject> searchedObjects = (NetworkManager.singleton as GameManager).AI;
        foreach(NetworkGuy player in players)
        {
            searchedObjects.Add(player.gameObject);
        }

        Vector3 position = this.gameObject.transform.position;
        foreach (GameObject enemy in searchedObjects)
        {
            float curDistance = Vector3.Distance(enemy.transform.position, position);
            if (curDistance <= distance)
            {
                NetworkGuy guy = enemy.GetComponent<NetworkGuy>();
                if(guy == null)
                {
                    //Bad aim
                    Debug.Log("AI");
                    this.guessCount -= 1;
                    
                }
                else
                {
                    //killed client
                    Debug.Log("Player");
                    this.guessCount += 1;
                    guy.guessCount = 0;
                }
            }
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
            innerButton.onClick.AddListener(CmdKill);
        }
        else
        {
            Debug.Log("Button Deactive");
            button.SetActive(false);
        }
    }
    

}