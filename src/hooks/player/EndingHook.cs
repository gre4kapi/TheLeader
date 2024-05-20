using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using MoreSlugcats;
using RWCustom;
using UnityEngine;
using static TheLeader.Hooks.MS_AERIESTARTld;

namespace TheLeader;
public partial class Hooks
{

	public static void ApplyEnding()
	{
		On.RoomCamera.ChangeRoom += IsEndingRoom;
        //On.MoreSlugcats.MSCRoomSpecificScript.AddRoomSpecificScript += MSCRoomSpecificScript_AddRoomSpecificScript;
    }

    private static void MSCRoomSpecificScript_AddRoomSpecificScript(On.MoreSlugcats.MSCRoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
		orig(room);
        if (room.abstractRoom.name == "MS_aeriestartld" && !ending)
        {
            ending = true;
            room.AddObject(new MS_AERIESTARTld(room));
            var message = "ADDED ENDING SCRIPT";
            Debug.Log(message);
        }
    }

    private static void IsEndingRoom(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room room, int camPos)
	{
		orig(self, room, camPos);
		if (room.abstractRoom.name == "MS_aeriestartld" && !ending)
		{
			ending = true;
			room.AddObject(new MS_AERIESTARTld(room));
			var message = "ADDED BITTER AERIE ENDING SCRIPT";
			Debug.Log(message);
		}
        if (room.abstractRoom.name == "OE_FINAL03" && ending)
        {
            ending = true;
            room.AddObject(new OE_FINAL03_Ending(room));
            var message = "ADDED OE_FINAL03 ENDING SCRIPT";
            Debug.Log(message);
        }
        else
        {
            Debug.Log(room.abstractRoom.name);
        }
    }
	public class MS_AERIESTARTld : UpdatableAndDeletable
	{
		private Player foundPlayer;
		private bool setController;
		private bool cutsceneEnded = false;
		public FadeOut fadeOut;
        public Player.InputPackage GetInput()
        {
            return new Player.InputPackage(true, Options.ControlSetup.Preset.None, (this.foundPlayer.firstChunk.pos.x < 2000.0f) ? 1 : 0, ((!this.foundPlayer.standing || this.foundPlayer.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.foundPlayer.standing || this.foundPlayer.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
        }
		public class EndingController : Player.PlayerController
		{
			// Token: 0x0600470A RID: 18186 RVA: 0x004BEDDB File Offset: 0x004BCFDB
			public EndingController(MS_AERIESTARTld owner)
			{
				this.owner = owner;
			}

			// Token: 0x0600470B RID: 18187 RVA: 0x004BEDEA File Offset: 0x004BCFEA
			public override Player.InputPackage GetInput()
			{
				return this.owner.GetInput();
			}

			// Token: 0x04004C6E RID: 19566
			private MS_AERIESTARTld owner;
		}
            public MS_AERIESTARTld(Room room)
		{
			this.room = room;
		}
		public override void Update(bool eu)
		{
			base.Update(eu);
			if (!ModManager.CoopAvailable)
			{
				if (this.foundPlayer == null && this.room.game.Players.Count > 0 && this.room.game.Players[0].realizedCreature != null && this.room.game.Players[0].realizedCreature.room == this.room)
				{
					this.foundPlayer = (this.room.game.Players[0].realizedCreature as Player);
				}
				if (this.foundPlayer == null || this.foundPlayer.inShortcut || this.room.game.Players[0].realizedCreature.room != this.room)
				{
					return;
				}
			}
			else
			{
				if (this.foundPlayer == null && this.room.PlayersInRoom.Count > 0 && this.room.PlayersInRoom[0] != null && this.room.PlayersInRoom[0].room == this.room)
				{
					this.foundPlayer = this.room.PlayersInRoom[0];
				}
				if (this.foundPlayer == null || this.foundPlayer.inShortcut || this.foundPlayer.room != this.room)
				{
					return;
				}
				if (ModManager.CoopAvailable)
				{
					this.room.game.cameras[0].EnterCutsceneMode(this.foundPlayer.abstractCreature, RoomCamera.CameraCutsceneType.EndingOE);
				}
			}
			float num = 2000f;
			if (this.foundPlayer.firstChunk.pos.x < num && !this.setController)
			{
				Debug.Log("START CUTSCENE");
				RainWorld.lockGameTimer = true;
				this.setController = true;
				this.foundPlayer.controller = new MS_AERIESTARTld.EndingController(this);
			}

			if (this.foundPlayer.firstChunk.pos.x > 750f && !cutsceneEnded)
			{
				if (this.fadeOut == null)
				{
					this.fadeOut = new FadeOut(this.room, Color.black, 200f, false);
					this.room.AddObject(this.fadeOut);
				}
			}
			if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.cutsceneEnded)
			{

				//this.room.game.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
				//self.room.game.GoToRedsGameOver();
				if (!ModManager.MMF)
				{
					//this.room.game.ExitGame(false, false);
				}
				//this.room.game.manager.statsAfterCredits = true;
				//this.room.game.manager.nextSlideshow = Enums.Scenes.Leader_AltOutro;
				//this.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
				//RainWorldGame.BeatGameMode(this.room.game, false);
				//Debug.Log(this.room.world.singleRoomWorld);
				this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "OE_FINAL03");
				//this.fadeOut.Destroy();
				this.foundPlayer.controller = null;
				this.cutsceneEnded = true;
				//Debug.Log(this.room.world.singleRoomWorld);
				//this.worldLoader = new WorldLoader(this.room.game, Enums.Leader, true, this.roomName, (Region)null, this.room.game.setupValues);
				//this.worldLoader.NextActivity();
			}

		}
	}
	public class OE_FINAL03_Ending : UpdatableAndDeletable
	{
		bool playerSpawned = false;
        private int timer;
		private Player player;
        private FadeOut fadeIn;
        private FadeOut fadeOut;
        public bool cameraInit;
        public bool warpExecuted;
		private bool setController;
        private List<AbstractCreature> npcs;
        public Player.InputPackage GetInput()
        {
            return new Player.InputPackage(true, Options.ControlSetup.Preset.None, (this.player.firstChunk.pos.x > 1800.0f) ? -1 : 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
        }
        public class EndingController : Player.PlayerController
        {
            // Token: 0x0600470A RID: 18186 RVA: 0x004BEDDB File Offset: 0x004BCFDB
            public EndingController(OE_FINAL03_Ending owner)
            {
                this.owner = owner;
            }

            // Token: 0x0600470B RID: 18187 RVA: 0x004BEDEA File Offset: 0x004BCFEA
            public override Player.InputPackage GetInput()
            {
                return this.owner.GetInput();
            }

            // Token: 0x04004C6E RID: 19566
            private OE_FINAL03_Ending owner;
        }
        public OE_FINAL03_Ending(Room room)
		{
            this.npcs = new List<AbstractCreature>();
            this.room = room;
			playerSpawned = false;
            this.fadeIn = new FadeOut(this.room, Color.black, 200f, true);
			this.room.AddObject(this.fadeIn);
        }
		public override void Update(bool eu)
			{
			base.Update(eu);
			player = this.room.game.FirstAlivePlayer.realizedCreature as Player;
			timer++;
			if (!playerSpawned && this.room.fullyLoaded)
			{
                Vector2 vector = new Vector2(5580.0f, 240.0f);
                room.game.FirstAlivePlayer.realizedCreature.bodyChunks[0].HardSetPosition(vector + new Vector2(9f, 0f));
                room.game.FirstAlivePlayer.realizedCreature.bodyChunks[1].HardSetPosition(vector + new Vector2(-5f, 0f));
                AbstractPhysicalObject spear = new AbstractSpear(this.room.world, null, player.abstractCreature.pos, this.room.game.GetNewID(), false);
                AbstractPhysicalObject spear2 = spear;
                spear.RealizeInRoom();
                spear2.RealizeInRoom();
                player.LoseAllGrasps();
                player.SlugcatGrab(spear.realizedObject, 1);
                playerSpawned = true;
                this.fadeIn.freezeFade = false;

            }
			if (timer == 100)
			{
                for (int i = 0; i < 11; i++)
                {
                    Vector2 npcvector = new Vector2(UnityEngine.Random.Range(350f, 360f), UnityEngine.Random.Range(310f, 320f));
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, this.room.ToWorldCoordinate(npcvector), this.room.game.GetNewID());
                    if (!this.room.world.game.rainWorld.setup.forcePup)
                    {
						if (i < 9)
						{
							(abstractCreature.state as PlayerState).forceFullGrown = true;
						}
                    }
                    abstractCreature.RealizeInRoom();
                    this.npcs.Add(abstractCreature);
					abstractCreature.abstractAI.destination = player.abstractCreature.pos;
					abstractCreature.abstractAI.followCreature = player.abstractCreature;
					//abstractCreature.abstractAI.
                }
            }
			if (!playerSpawned)
			{
				this.fadeIn.freezeFade = true;
			}
			if (playerSpawned)
            if (playerSpawned && !this.setController)
            {
                Debug.Log("START CUTSCENE");
                RainWorld.lockGameTimer = true;
                this.setController = true;
                this.player.controller = new OE_FINAL03_Ending.EndingController(this);
            }
            var msg = "Vector2: " + this.room.game.FirstAlivePlayer.pos.x + " and " + this.room.game.FirstAlivePlayer.pos.y + " WC: " + player.abstractCreature.pos.x + " and " + player.abstractCreature.pos.y;
			Debug.Log(msg);
			if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.warpExecuted)
			{
				this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "GW_E02");
				this.warpExecuted = true;
			}
		}
	}
}
