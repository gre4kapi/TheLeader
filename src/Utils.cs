using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;
using RWCustom;
using static Conversation;
using static SSOracleBehavior;
using Random = UnityEngine.Random;
using static AbstractPhysicalObject;
using MoreSlugcats;
using TheLeader;

namespace TheLeader;

public static class Utils
{
    public static RainWorld RainWorld => Custom.rainWorld;
    public static SaveMiscProgression GetMiscProgression() => RainWorld.GetMiscProgression();
    public static void TryDream(this StoryGameSession storyGame, DreamsState.DreamID dreamId, bool isRecurringDream = false)
    {
        var miscWorld = storyGame.saveState.miscWorldSaveData.GetMiscWorld();

        if (miscWorld == null) return;

        var strId = dreamId.value;

        if (miscWorld.PreviousDreams.Contains(strId) && !isRecurringDream) return;

        miscWorld.CurrentDream = strId;
        SlugBase.Assets.CustomDreams.QueueDream(storyGame, dreamId);
    }


    public static void GiveTrueEnding(this SaveState saveState)
    {
        if (saveState.saveStateNumber != Enums.Leader) return;

        var miscProg = GetMiscProgression();
        var miscWorld = saveState.miscWorldSaveData.GetMiscWorld();

        if (miscWorld == null) return;


        miscProg.HasTrueEnding = true;
        miscProg.IsPearlpupSick = false;

        miscWorld.PebblesMeetCount = 0;

        //SlugBase.Assets.CustomScene.SetSelectMenuScene(saveState, Enums.Scenes.Slugcat_Pearlcat);

        // So the tutorial scripts can be added again
        foreach (var regionState in saveState.regionStates)
        {
            regionState?.roomsVisited?.RemoveAll(x => x?.StartsWith("T1_") == true);
        }
    }
}