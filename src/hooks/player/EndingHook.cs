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
    private static List<EntityID> id = new List<EntityID>();
    public static void ApplyEnding()
	{
		On.RoomCamera.ChangeRoom += IsEndingRoom;
        //On.MoreSlugcats.MSCRoomSpecificScript.AddRoomSpecificScript += MSCRoomSpecificScript_AddRoomSpecificScript;
    }
    private static void IsEndingRoom(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room room, int camPos)
	{
		orig(self, room, camPos);
		if (room.abstractRoom.name == "MS_aeriestartld" && !ending)
		{
			ending = true;
			room.AddObject(new MS_AERIESTARTld(room));
		}
        if (room.abstractRoom.name == "OE_FINAL03" && ending)
        {
            ending = true;
            room.AddObject(new OE_FINAL03_Ending(room));
        }
        if (room.abstractRoom.name == "SB_G03" && ending)
        {
            ending = true;
            room.AddObject(new SB_G03_Ending(room));
        }
        if (room.abstractRoom.name == "SL_ROOF03ld" && ending)
        {
            ending = true;
            room.AddObject(new SL_ROOF03ld_Ending(room));
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

				/*this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "OE_FINAL03");
				//this.fadeOut.Destroy();
				this.foundPlayer.controller = null;
				this.cutsceneEnded = true;*/

                RainWorldGame.BeatGameMode(room.game, false);

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
		bool jumped = false;
		private int jmpCount = 0;
		float jumpPos = 1420.0f;
		bool npcsSpawned = false;
        private int timer2;
        private static List<AbstractCreature> npcs;
        int closestNpcDist = 0;
        public Player.InputPackage GetInput()
		{
			if (!jumped)
			{
				if (this.player.firstChunk.pos.x > jumpPos)
				{
					return new Player.InputPackage(true, Options.ControlSetup.Preset.None, (this.player.firstChunk.pos.x > jumpPos) ? -1 : 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
				}
				else if (jmpCount < 150)
				{
					jmpCount++;
					if ((jmpCount % 30) == 0)
					{
						return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
					}
					else
					{
						return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, 0, true, false, false, false, false);
					}

				}
				else if (jmpCount < 400)
				{
					jmpCount++;
                    jumped = true;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
                }
				else
				{
                    jmpCount++;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 1, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
				}
            }
			else
			{
                jmpCount++;
                if (closestNpcDist < 10)
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 1, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
                else
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
            }
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
            npcs = new List<AbstractCreature>();
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
					Vector2 npcvector = new Vector2(1000 + i, 310);
					id.Add(this.room.game.GetNewID());
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, this.room.ToWorldCoordinate(npcvector), id[i]);
					if (!this.room.world.game.rainWorld.setup.forcePup)
					{
						if (i < 9)
						{
							(abstractCreature.state as PlayerState).forceFullGrown = true;
						}
					}
                    abstractCreature.RealizeInRoom();
					npcs.Add(abstractCreature);
				}
                npcsSpawned = true;
                this.fadeIn.freezeFade = false;
            }
			if (npcsSpawned)
			{
                if (jumped)
                {
                    closestNpcDist = Math.Abs(player.abstractCreature.pos.x - npcs[0].pos.x);
                    foreach (AbstractCreature npc in npcs)
                    {
                        npc.abstractAI.RealAI.SetDestination(player.abstractCreature.pos);
                        int newNpcDist = Math.Abs(player.abstractCreature.pos.x - npc.pos.x);
                        if (closestNpcDist > newNpcDist)
                        {
                            closestNpcDist = newNpcDist;
                        }
                    }
                }
                else
                {
                    foreach (AbstractCreature npc in npcs)
                    {
                        //npc.abstractAI.RealAI.SetDestination(Custom.MakeWorldCoordinate(new IntVector2(1000 + i, 310), this.room.abstractRoom.index));
                        npc.abstractAI.RealAI.SetDestination(npc.pos);
                    }
                }
			}
			if (!playerSpawned || !npcsSpawned)
			{
				this.fadeIn.freezeFade = true;
			}
            if (playerSpawned && npcsSpawned && !this.setController)
            {
                RainWorld.lockGameTimer = true;
                this.setController = true;
                this.player.controller = new OE_FINAL03_Ending.EndingController(this);
            }
			if (jmpCount > 500 && this.fadeOut == null)
			{
                this.fadeOut = new FadeOut(this.room, Color.black, 120f, false);
                this.room.AddObject((UpdatableAndDeletable)this.fadeOut);
            }
			if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.warpExecuted)
			{
				this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "SB_G03");
				this.warpExecuted = true;
			}
		}
	}
    public class SB_G03_Ending : UpdatableAndDeletable
    {
        bool playerSpawned = false;
		bool npcsSpawned = false;
        private int timer;
        private Player player;
        private FadeOut fadeIn;
        private FadeOut fadeOut;
        public bool cameraInit;
        public bool warpExecuted;
        private bool setController;
        int closestNpcDist = 0;
        int moveTimer = 0;
        private List<AbstractCreature> npcs;
        bool startedRunning = false;
        public Player.InputPackage GetInput()
        {
            if (moveTimer == 0)
            {
                if (closestNpcDist < 10)
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 1, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
                else
                {
                    timer++;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
            }
            else
            {
                timer++;
                if (timer > 50)
                {
                    timer = 0;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
                else
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
            }
        }
        public class EndingController : Player.PlayerController
        {
            // Token: 0x0600470A RID: 18186 RVA: 0x004BEDDB File Offset: 0x004BCFDB
            public EndingController(SB_G03_Ending owner)
            {
                this.owner = owner;
            }

            // Token: 0x0600470B RID: 18187 RVA: 0x004BEDEA File Offset: 0x004BCFEA
            public override Player.InputPackage GetInput()
            {
                return this.owner.GetInput();
            }

            // Token: 0x04004C6E RID: 19566
            private SB_G03_Ending owner;
        }
        public SB_G03_Ending(Room room)
        {
            this.room = room;
            playerSpawned = false;
            this.npcs = new List<AbstractCreature>();
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
                Vector2 vector = new Vector2(4000.0f, 403.75f);
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
            if (timer == 50)
            {
                for (int i = 0; i < 11; i++)
                {
                    WorldCoordinate cord = player.abstractCreature.pos;
                    cord.x = cord.x - 20 + i;
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, cord, id[i]);
                    if (!this.room.world.game.rainWorld.setup.forcePup)
                    {
                        if (i < 9)
                        {
                            (abstractCreature.state as PlayerState).forceFullGrown = true;
                        }
                    }
                    abstractCreature.RealizeInRoom();
                    npcs.Add(abstractCreature);
                }
                npcsSpawned = true;
                this.fadeIn.freezeFade = false;
            }
            if (npcsSpawned)
			{
                closestNpcDist = Math.Abs(player.abstractCreature.pos.x - npcs[0].pos.x);
                foreach (AbstractCreature npc in npcs)
                {
                    int newNpcDist = Math.Abs(player.abstractCreature.pos.x - npc.pos.x);
                    if (closestNpcDist > newNpcDist)
                    {
                        closestNpcDist = newNpcDist;
                    }
                    npc.abstractAI.RealAI.SetDestination(player.abstractCreature.pos);
                }
                startedRunning = true;
                this.fadeIn.freezeFade = false;
            }
            if (!playerSpawned || !npcsSpawned || !startedRunning)
            {
                this.fadeIn.freezeFade = true;
            }
            if (playerSpawned && npcsSpawned && !this.setController)
            {
                RainWorld.lockGameTimer = true;
                this.setController = true;
                this.player.controller = new SB_G03_Ending.EndingController(this);
            }
            if (timer > 700 && this.fadeOut == null)
            {
                this.fadeOut = new FadeOut(this.room, Color.black, 120f, false);
                this.room.AddObject((UpdatableAndDeletable)this.fadeOut);
            }
            if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !this.warpExecuted)
            {
                this.room.game.overWorld.InitiateSpecialWarp_SingleRoom(null, "SL_ROOF03ld");
                this.warpExecuted = true;
            }
        }
    }
    public class SL_ROOF03ld_Ending : UpdatableAndDeletable
    {
        bool playerSpawned = false;
        bool npcsSpawned = false;
        private int timer;
        private Player player;
        private FadeOut fadeIn;
        private FadeOut fadeOut;
        public bool cameraInit;
        public bool warpExecuted;
        private bool setController;
        int closestNpcDist = 0;
        private List<AbstractCreature> npcs;
        bool startedRunning = false;
        int moveTimer = 0;
        bool startOutro = false;
        public Player.InputPackage GetInput()
        {
            if (moveTimer == 0)
            {
                if (closestNpcDist < 10)
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 1, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
                else
                {
                    timer++;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
            }
            else
            {
                timer++;
                if (timer > 50)
                {
                    timer = 0;
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
                else
                {
                    return new Player.InputPackage(true, Options.ControlSetup.Preset.None, 0, ((!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f) ? 1 : 0, (!this.player.standing || this.player.bodyMode == Player.BodyModeIndex.Crawl) && UnityEngine.Random.value < 0.5f, false, false, false, false);
                }
            }
        }
        public class EndingController : Player.PlayerController
        {
            // Token: 0x0600470A RID: 18186 RVA: 0x004BEDDB File Offset: 0x004BCFDB
            public EndingController(SL_ROOF03ld_Ending owner)
            {
                this.owner = owner;
            }

            // Token: 0x0600470B RID: 18187 RVA: 0x004BEDEA File Offset: 0x004BCFEA
            public override Player.InputPackage GetInput()
            {
                return this.owner.GetInput();
            }

            // Token: 0x04004C6E RID: 19566
            private SL_ROOF03ld_Ending owner;
        }
        public SL_ROOF03ld_Ending(Room room)
        {
            this.room = room;
            playerSpawned = false;
            this.npcs = new List<AbstractCreature>();
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
                Vector2 vector = new Vector2(570.0f, 1063.75f);
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
            if (timer == 50)
            {
                for (int i = 0; i < 11; i++)
                {
                    WorldCoordinate cord = player.abstractCreature.pos;
                    cord.x = cord.x - 20 + i;
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, cord, id[i]);
                    if (!this.room.world.game.rainWorld.setup.forcePup)
                    {
                        if (i < 9)
                        {
                            (abstractCreature.state as PlayerState).forceFullGrown = true;
                        }
                    }
                    abstractCreature.RealizeInRoom();
                    npcs.Add(abstractCreature);
                }
                npcsSpawned = true;
                this.fadeIn.freezeFade = false;
            }
            if (npcsSpawned)
            {
                closestNpcDist = Math.Abs(player.abstractCreature.pos.x - npcs[0].pos.x);
                foreach (AbstractCreature npc in npcs)
                {
                    int newNpcDist = Math.Abs(player.abstractCreature.pos.x - npc.pos.x);
                    if (closestNpcDist > newNpcDist)
                    {
                        closestNpcDist = newNpcDist;
                    }
                    npc.abstractAI.RealAI.SetDestination(player.abstractCreature.pos);
                }
                startedRunning = true;
                this.fadeIn.freezeFade = false;
            }
            if (!playerSpawned || !npcsSpawned || !startedRunning)
            {
                this.fadeIn.freezeFade = true;
            }
            if (playerSpawned && npcsSpawned && !this.setController)
            {
                RainWorld.lockGameTimer = false;
                this.setController = true;
                this.player.controller = new SL_ROOF03ld_Ending.EndingController(this);
            }
            if (timer > 700 && this.fadeOut == null)
            {
                this.fadeOut = new FadeOut(this.room, Color.black, 120f, false);
                this.room.AddObject((UpdatableAndDeletable)this.fadeOut);
            }
            if (this.fadeOut != null && this.fadeOut.IsDoneFading() && !startOutro)
            {
                /*if (this.room.game.manager.upcomingProcess != null)
                {
                    return;
                }
                if (this.room.game.manager.musicPlayer != null)
                {
                    this.room.game.manager.musicPlayer.FadeOutAllSongs(20f);
                }
                this.room.game.manager.rainWorld.progression.SaveWorldStateAndProgression(false);
                /*if (!ModManager.MMF)
                {
                    this.room.game.ExitGame(false, false);
                }
                
                this.room.game.manager.statsAfterCredits = true;
                this.room.game.manager.nextSlideshow = Enums.Scenes.Leader_AltOutro;
                this.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);*/
                this.room.game.manager.nextSlideshow = Enums.Scenes.Leader_AltOutro;
                this.room.game.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
                startOutro = true;
            }
        }
    }
}
