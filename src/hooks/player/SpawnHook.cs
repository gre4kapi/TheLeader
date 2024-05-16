using System;
using UnityEngine;
using BepInEx.Logging;
using System.Collections.Generic;
using MoreSlugcats;
using System.IO;
using System.Linq;

namespace TheLeader;
public partial class Hooks
{
    public static void ApplySpawnHook()
    {
        On.RoomSpecificScript.AddRoomSpecificScript += StartRoom_Script;
    }

    private static void StartRoom_Script(On.RoomSpecificScript.orig_AddRoomSpecificScript orig, Room room)
    {
        orig(room);

        if (room.game.IsLeader() && room.game.world.rainCycle.CycleProgression == 0 && room.game.GetStorySession.saveState.cycleNumber == 0)
        {
            File.WriteAllText(AssetManager.ResolveFilePath("data/pebblesMet.txt"), "false");
            Debug.Log("SETTING pebblesMet TO false");

            if (room.abstractRoom.name == "OE_FINAL03")
            {
                room.AddObject(new OE_FINAL03(room));
                var message = "ADDED LEADER SPAWN SCRIPT";
                Debug.Log(message);
            }
        }

    }
    public class OE_FINAL03 : UpdatableAndDeletable
    {
        private int timer;
        private List<AbstractCreature> npcs;

        public OE_FINAL03(Room room) 
        {

            this.room = room;
            this.npcs = new List<AbstractCreature>();
            //Vector2 vector = new Vector2(0.0f, 0.0f);
            //System.Random rnd = new System.Random();
            //room.game.FirstAlivePlayer.realizedCreature.bodyChunks[0].HardSetPosition(vector + new Vector2(9f, 0f));
            //room.game.FirstAlivePlayer.realizedCreature.bodyChunks[1].HardSetPosition(vector + new Vector2(-5f, 0f));
            //AbstractCreature slug = new AbstractCreature(room.world, StaticWorld.GetCreatureTemplate("Slugcat"), null, room.game.FirstAlivePlayer.pos, new EntityID(-1, rnd.Next(2, 999)));
            //room.abstractRoom.creatures.Add(slug);
            //var message = playerPos;
            //Debug.Log(message);
            //room.PlaySound(SoundID.Mushroom_Trip_LOOP, room.game.FirstAlivePlayer.realizedCreature.mainBodyChunk, true, 1f, 1f);
            //room.abstractRoom.creatures.Add(what to put here?);
        }
        public override void Update(bool eu)
        {
            var game = (RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame);
            base.Update(eu);
            timer++;
            if (timer == 10)
            {
                for (int i = 0; i < 11; i++)
                {
                    Vector2 vector = new Vector2(UnityEngine.Random.Range(480f, 3450f), UnityEngine.Random.Range(230f, 300f));
                    AbstractCreature abstractCreature = new AbstractCreature(this.room.world, StaticWorld.GetCreatureTemplate(MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, this.room.ToWorldCoordinate(vector), this.room.game.GetNewID());
                    if (!this.room.world.game.rainWorld.setup.forcePup)
                    {
                        (abstractCreature.state as PlayerState).forceFullGrown = true;
                    }
                    //AbstractPhysicalObject pearl = new AbstractPhysicalObject(this.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, this.room.game.FirstAlivePlayer.pos, this.room.game.GetNewID());
                    //this.room.abstractRoom.AddEntity(abstractCreature);
                    abstractCreature.RealizeInRoom();
                    this.npcs.Add(abstractCreature);
                }
            }
            if (timer == 10)
            {
                Vector2 vector = new Vector2(350.0f, 310.0f);
                room.game.FirstAlivePlayer.realizedCreature.bodyChunks[0].HardSetPosition(vector + new Vector2(9f, 0f));
                room.game.FirstAlivePlayer.realizedCreature.bodyChunks[1].HardSetPosition(vector + new Vector2(-5f, 0f));
                var message = "bebra: "+playerPos;
                Debug.Log(message);
            }
            if (timer == 300)
            {
                //System.Random rnd = new System.Random();
                //AbstractCreature slug2 = new AbstractCreature(room.world, StaticWorld.GetCreatureTemplate("Slugcat"), room.game.FirstAlivePlayer.realizedCreature, room.game.FirstAlivePlayer.pos, new EntityID(-1, rnd.Next(2, 999)));
                //slug2.RealizeInRoom();
            }
        }
    }

}