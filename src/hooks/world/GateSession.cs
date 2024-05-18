using System.Collections.Generic;
using UnityEngine;
using RWCustom;
using MoreSlugcats;
using System.Linq;
using System;
using JollyCoop;
using Menu;
using static MonoMod.InlineRT.MonoModRule;
using Random = UnityEngine.Random;
using Expedition;
using System.Reflection;
using System.IO;

namespace TheLeader;
public partial class Hooks
{
    public static bool openGate = false;
    public static string openGateName = "";
    public static int openCount = 0;
    public static int noGrabbedCount = 0;
    public static DataPearl writedPearl;
    public static List<GreenSparks> greenSparks;

    //进入结局cg前的准备
    private static bool isControled = false;
    //public static FSprite blackRect = new FSprite("pixel");

    //结局后的结局
    public static bool goEnding = false;

    public static void GateInit()
    {
        On.Player.ctor += Player_ctor;
        On.Player.UpdateMSC += Player_EndUpdate;
        //On.RegionState.AdaptRegionStateToWorld += RegionState_AdaptRegionStateToWorld;
    }
    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);

        openGate = false;
        openGateName = "";
        openCount = 0;
        noGrabbedCount = 0;
        isControled = false;
        goEnding = false;
        writedPearl = null;
    }

    public static void Player_EndUpdate(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);
        string nestingOpen = File.ReadLines(AssetManager.ResolveFilePath("data/nestingOpen.txt")).First();
        if (!IsLeader(self.room.game))
            return;
        if (nestingOpen == "true")
        {
            openGate = true;
            openGateName = self.room.abstractRoom.name;
        }

        if (self.room.IsGateRoom() && PearlWritedSave.pearlWrited && !openGate && self.room.regionGate.EnergyEnoughToOpen &&
            self == self.room.game.Players[0].realizedCreature as Player)//这一条是确保只对一个玩家更新
        {
            GreenSparks greenSpark = new GreenSparks(self.room, 1f);
            if (openCount == 0 && writedPearl != null && writedPearl.grabbedBy.Count == 0)//防止玩家吞吐珍珠的一瞬间也能开启业力门
            {
                noGrabbedCount++;
            }
            if (writedPearl != null && writedPearl.abstractPhysicalObject.realizedObject == null)//防止玩家吞吐珍珠的一瞬间也能开启业力门
            {
                openCount = (int)Mathf.Max(openCount - 5, 0);
                noGrabbedCount = 0;
            }
            if (writedPearl != null && writedPearl.abstractPhysicalObject.realizedObject != null && noGrabbedCount > 5 &&
                ((writedPearl.firstChunk.pos.x > 250f && writedPearl.firstChunk.pos.x < 650f) || openCount > 0))
            {
                openCount++;

                //屏幕震动
                self.room.ScreenMovement(new Vector2?(writedPearl.firstChunk.pos), new Vector2(0f, 0f), Mathf.Min(Custom.LerpMap((float)openCount, 40f, 300f, 0f, 1.5f, 1.2f), Custom.LerpMap((float)openCount, 40f, 300f, 1.5f, 0f)));
                //珍珠移动
                Vector2 wantPos = (self.firstChunk.pos.x > 480f) ? new Vector2(570f, 295f) : new Vector2(390f, 295f);
                writedPearl.firstChunk.vel *= Custom.LerpMap(writedPearl.firstChunk.vel.magnitude, 1f, 6f, 0.999f, 0.9f);
                writedPearl.firstChunk.vel += Vector2.ClampMagnitude(wantPos - writedPearl.firstChunk.pos, 100f) / 100f * 0.4f;
                //抵消重力
                writedPearl.firstChunk.vel += 1f * Vector2.up;
            }
            //找到珍珠
            List<PhysicalObject>[] physicalObjects = self.room.physicalObjects;
            for (int i = 0; i < physicalObjects.Length; i++)
            {
                for (int j = 0; j < physicalObjects[i].Count; j++)
                {
                    PhysicalObject physicalObject = physicalObjects[i][j];
                    if ((physicalObject is DataPearl) && physicalObject.abstractPhysicalObject.realizedObject != null &&
                        (physicalObject as DataPearl).AbstractPearl.dataPearlType == Enums.FixedPebblesPearl)
                    {
                        writedPearl = physicalObject as DataPearl;
                    }
                }
            }
            //在业力门范围，珍珠启动后不能再被抓住
            if (openCount > 0 && writedPearl != null && writedPearl.grabbedBy.Count != 0)
            {
                for (int i = 0; i < writedPearl.grabbedBy.Count; i++)
                {
                    if (self.grasps != null)
                    {
                        for (int j = 0; j < self.grasps.Length; j++)
                        {
                            if (self.grasps[j] != null &&
                                self.grasps[j].grabbed != null &&
                                self.grasps[j].grabbed == writedPearl)
                            {
                                self.ReleaseGrasp(j);
                            }
                        }
                    }
                }
            }
            if (openCount == 1)
            {
                greenSparks = new List<GreenSparks>();
                openGateName = self.room.abstractRoom.name;
            }
            //生成监视者
            //加绿色闪电
            if (openCount == 21)
            {
                self.room.AddObject(new ElectricFullScreen(self.room, 0.8f, 280, 180, 40));
            }
            //珍珠开门动画
            if (openCount > 21 && openCount < 220 && writedPearl != null)
            {
                if (openCount % 20 == 0)
                {
                    self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Data_Bit, writedPearl.firstChunk.pos, 1f, 1f + Random.value * 2f);
                    self.room.AddObject(new Explosion.ExplosionLight(writedPearl.firstChunk.pos, 150f, 1f, 15, Color.green));
                }
                //加绿色电火花
                if (greenSpark != null)
                {
                    self.room.AddObject(greenSpark);
                    if (greenSparks != null && greenSpark != null)
                        greenSparks.Add(greenSpark);
                }
            }
            if (openCount == 220 && writedPearl != null)
            {
                self.room.PlaySound(SoundID.Moon_Wake_Up_Green_Swarmer_Flash, writedPearl.firstChunk.pos, 0.5f, 1f);
                self.room.PlaySound(SoundID.Fire_Spear_Explode, writedPearl.firstChunk.pos, 0.5f, 1f);
                self.room.AddObject(new ElectricFullScreen.SparkFlash(writedPearl.firstChunk.pos, 50f));
                self.room.AddObject(new Spark(writedPearl.firstChunk.pos, Custom.RNV() * Random.value * 40f, new Color(0f, 1f, 0f), null, 30, 120));
                writedPearl.Destroy();
            }
            if (openCount == 300)
            {
                openCount = 0;
                openGate = true;
                File.WriteAllText(AssetManager.ResolveFilePath("data/nestingOpen.txt"), "true");
                writedPearl = null;
                for (int k = greenSparks.Count - 1; k >= 0; k--)
                {
                    GreenSparks spark = greenSparks[k];
                    greenSparks.Remove(spark);
                    spark.Destroy();
                }
                greenSparks.Clear();
            }
        }


    }
}