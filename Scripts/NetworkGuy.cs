using Mirror;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGuy : NetworkBehaviour
{

    public enum GameRole
    {
        Hunter,
        Hider,
        None
    }

    [SyncVar]
    public GameRole role = GameRole.None;

    private void Start()
    {
        if (isLocalPlayer)
        {

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
        }
    }

    
    [Command(requiresAuthority = false)]
    public void CmdSetRole(GameRole role)
    {
        this.role = role;
    }
    

}
