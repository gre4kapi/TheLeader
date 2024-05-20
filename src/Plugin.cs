using System;
using BepInEx;
using BepInEx.Logging;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security;
using UnityEngine;
using static TheLeader.Hooks;
using UnityEngine.Diagnostics;
using MoreSlugcats;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using Menu;

namespace TheLeader
{
    [BepInPlugin(MOD_ID, "The Leader", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        public const string MOD_ID = "gre4ka.theleader";
        public const string CAT_NAME = "Leader";
        static public SlugcatStats.Name SlugName;
        //public static DataPearl.AbstractDataPearl.DataPearlType FixedPebblesPearl;

        public new static ManualLogSource Logger { get; private set; } = null!;
        public static bool gateLock = true;
        private bool warpid = false;

        // Add hooks
        public void OnEnable()
        {
            //On.Player.Jump += Player_Jump;
            Logger = base.Logger;
            ApplyInit();

        }
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);
            /*if (self.room.game.manager.upcomingProcess != null)
            {
                return;
            }
            if (self.room.game.manager.musicPlayer != null)
            {
                self.room.game.manager.musicPlayer.FadeOutAllSongs(20f);
            }
            self.room.game.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
            //self.room.game.GoToRedsGameOver();
            if (!ModManager.MMF)
            {
                self.room.game.ExitGame(false, false);
            }
            RainWorldGame.BeatGameMode(self.room.game, true);
            self.room.game.manager.statsAfterCredits = true;
            self.room.game.manager.nextSlideshow = Enums.Scenes.Leader_Outro;
            self.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);*/
            Vector2 npcvector = new Vector2(UnityEngine.Random.Range(480f, 3450f), UnityEngine.Random.Range(230f, 300f));
            AbstractCreature abstractCreature = new AbstractCreature(self.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, self.room.ToWorldCoordinate(npcvector), self.room.game.GetNewID());
            if (!self.room.world.game.rainWorld.setup.forcePup)
            {
                (abstractCreature.state as PlayerState).forceFullGrown = true;
            }
            abstractCreature.RealizeInRoom();
            if (!warpid)
            {
                self.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "OE_FINAL03");
                warpid = true;
            }
        }

        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld)
        {
        }
    }
}