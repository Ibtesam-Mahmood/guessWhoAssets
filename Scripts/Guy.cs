using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class Guy : NetworkBehaviour
{

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
            foreach(CharacterAbility ability in characterAbilities){
                ability.enabled = false;
            }
        }
    }

}
