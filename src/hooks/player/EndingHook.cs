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
			var message = "ADDED ENDING SCRIPT";
			Debug.Log(message);
		}
        if (room.abstractRoom.name == "OE_FINAL03" && ending)
        {
            ending = true;
            room.AddObject(new OE_FINAL03(room));
            var message = "ADDED ENDING SCRIPT";
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
		private bool endTrigger;
		private int endTriggerTimer;
		public FadeOut fadeOut;
		private bool doneFinalSave;
        private WorldLoader worldLoader;
		private string roomName = "OE_FINAL03";
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
			
			if (this.foundPlayer.firstChunk.pos.x > 750f && !this.endTrigger)
			{
				this.endTriggerTimer++;
				float num4 = 80f;
				if ((float)this.endTriggerTimer >= num4)
				{
					this.endTrigger = true;
					this.room.game.manager.sceneSlot = this.room.game.GetStorySession.saveStateNumber;
					if (this.fadeOut == null)
					{
						this.fadeOut = new FadeOut(this.room, Color.black, 200f, false);
						this.room.AddObject(this.fadeOut);
					}
				}
			}
			if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.doneFinalSave)
			{

				if (this.room.game.manager.upcomingProcess != null)
				{
					return;
				}
				if (this.room.game.manager.musicPlayer != null)
				{
					this.room.game.manager.musicPlayer.FadeOutAllSongs(20f);
				}
				this.room.game.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
				//self.room.game.GoToRedsGameOver();
				if (!ModManager.MMF)
				{
					//this.room.game.ExitGame(false, false);
				}
				this.room.game.manager.statsAfterCredits = true;
                //this.room.game.manager.nextSlideshow = Enums.Scenes.Leader_AltOutro;
                //this.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
                RainWorldGame.BeatGameMode(this.room.game, false);
				//Debug.Log(this.room.world.singleRoomWorld);
				this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "OE_FINAL03");
				//Debug.Log(this.room.world.singleRoomWorld);
                //this.worldLoader = new WorldLoader(this.room.game, Enums.Leader, true, this.roomName, (Region)null, this.room.game.setupValues);
                //this.worldLoader.NextActivity();
                this.doneFinalSave = true;
			}

		}
		public class OE_FINAL03 : UpdatableAndDeletable, IRunDuringDialog
		{
			// Token: 0x06003F6B RID: 16235 RVA: 0x0046EFF0 File Offset: 0x0046D1F0
			public OE_FINAL03(Room room)
			{
				this.room = room;
				this.fadeIn = new FadeOut(this.room, Color.black, 200f, true);
				this.room.AddObject(this.fadeIn);
			}

			// Token: 0x06003F6C RID: 16236 RVA: 0x0046F02C File Offset: 0x0046D22C
			public override void Update(bool eu)
			{
				base.Update(eu);
				Player player = null;
				AbstractCreature firstAlivePlayer = this.room.game.FirstAlivePlayer;
				if (this.room.game.Players.Count > 0 && firstAlivePlayer != null && firstAlivePlayer.realizedCreature != null)
				{
					player = (firstAlivePlayer.realizedCreature as Player);
					for (int i = 0; i < this.room.game.Players.Count; i++)
					{
						if (this.room.game.Players[i].realizedCreature != null && this.room.game.Players[i].Room.name == this.room.abstractRoom.name)
						{
							Player player2 = this.room.game.Players[i].realizedCreature as Player;
							player2.LoseAllGrasps();
							player2.SuperHardSetPosition(new Vector2(-500f, 2000f));
							player2.Stun(40);
							for (int j = 0; j < player2.bodyChunks.Length; j++)
							{
								player2.bodyChunks[j].vel.y = Mathf.Clamp(player2.bodyChunks[j].vel.y, -5f, 5f);
							}
						}
					}
					//MSCRoomSpecificScript.HardSetStunAllObjects(this.room, new Vector2(-500f, 3000f));
				}
				if (this.fadeIn != null && this.room.game.cameras[0].currentCameraPosition != 1)
				{
					this.fadeIn.fade = 1f;
				}
				if (this.fadeIn != null && this.fadeIn.IsDoneFading() && player != null)
				{
					this.timer++;
					if (this.timer == 1)
					{
						this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Sat_Interference3, 0f, 1f, 0.95f);
						this.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Sat_Interference3, 0f, 1f, 0.98f);
					}
					if (this.timer == 120)
					{
						player.InitChatLog(ChatlogData.ChatlogID.Chatlog_LM7);
					}
					if (this.timer > 120 && !player.chatlog && this.fadeOut == null)
					{
						this.fadeOut = new FadeOut(this.room, Color.black, 120f, false);
						this.room.AddObject(this.fadeOut);
					}
				}
				if (this.room.fullyLoaded && !this.cameraInit)
				{
					this.room.game.cameras[0].followAbstractCreature = null;
					this.room.game.cameras[0].MoveCamera(1);
					this.cameraInit = true;
				}
				if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.warpExecuted)
				{
					this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "GW_E02");
					if (this.room.blizzardGraphics != null)
					{
						this.room.RemoveObject(this.room.blizzardGraphics);
						this.room.blizzardGraphics.Destroy();
						this.room.blizzardGraphics = null;
						this.room.blizzard = false;
					}
					this.warpExecuted = true;
				}
			}

			// Token: 0x04004099 RID: 16537
			private int timer;

			// Token: 0x0400409A RID: 16538
			private FadeOut fadeIn;

			// Token: 0x0400409B RID: 16539
			private FadeOut fadeOut;

			// Token: 0x0400409C RID: 16540
			public bool cameraInit;

			// Token: 0x0400409D RID: 16541
			public bool warpExecuted;
		}
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
	}
}