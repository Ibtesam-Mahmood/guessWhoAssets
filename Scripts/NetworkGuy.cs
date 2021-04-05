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
        Debug.Log("Killed");
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

/*
 * if (!isLocalPlayer || role != GameRole.Hunter)
            return;

        target = null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3 position = this.gameObject.transform.position;
        foreach (GameObject enemy in enemies)
        {
            float curDistance = Vector3.Distance(enemy.transform.position, position);
            if (curDistance <= distance)
            {
                target = enemy;
            }
        }
 */