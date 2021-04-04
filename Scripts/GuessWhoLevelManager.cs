

using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace Application
{
    public class GuessWhoLevelManager : LevelManager
    {

        override protected void Start()
        {
            BoundsCollider = _collider;
            InstantiatePlayableCharacters();

            if (UseLevelBounds)
            {
                MMCameraEvent.Trigger(MMCameraEventTypes.SetConfiner, null, BoundsCollider);
            }

            //if (Players == null || Players.Count == 0) { return; }

            //Initialization();

            //// we handle the spawn of the character(s)
            //if (Players.Count == 1)
            //{
            //    SpawnSingleCharacter();
            //}
            //else
            //{
            //    SpawnMultipleCharacters();
            //}

            //CheckpointAssignment();

            // we trigger a fade
            MMFadeOutEvent.Trigger(IntroFadeDuration, FadeCurve, FaderID);

            // we trigger a level start event
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.LevelStart, null);
            MMGameEvent.Trigger("Load");

            MMCameraEvent.Trigger(MMCameraEventTypes.SetTargetCharacter, Players[0]);
            MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing);
            MMGameEvent.Trigger("CameraBound");
        }

    }
}
