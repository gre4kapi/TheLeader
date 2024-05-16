using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using MoreSlugcats;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
using static System.Reflection.BindingFlags;
using System;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;
using IL;
using System.Linq;
using UnityEngine.Diagnostics;
using On;
using System.Net.Configuration;
using RewiredConsts;
using UnityEngine.Rendering;
using RWCustom;
using System.Xml.XPath;
using TheLeader.effects;
using static DataPearl.AbstractDataPearl;
using System.Linq;
using System.IO;

namespace TheLeader;
public partial class Hooks
{
	private static ConditionalWeakTable<Oracle, StrongBox<int>> OracleCWT = new ConditionalWeakTable<Oracle, StrongBox<int>>();
    //private static ConditionalWeakTable<MoreSlugcats.SSOracleRotBehavior, OracleRMModule> OracleRMData { get; } = new();
    public static readonly GameFeature<bool> CustomConversations = GameBool("CustomConversations");
    //private static string filepath = AssetManager.ResolveFilePath("data/pebblesMet.txt");
    private static bool eyesClosed = false;
	private static bool pebblesMeeted = false;
	//private static bool started = false;
	public static Vector2 playerPos;
	public static void ApplyConvs()
	{
        new Hook(typeof(SSOracleRotBehavior).GetMethod("get_EyesClosed", Public | NonPublic | Instance), (Func<SSOracleRotBehavior, bool> orig, SSOracleRotBehavior self) => (eyesClosed && self.oracle.room.game.session is StoryGameSession session && session.saveStateNumber.value == Plugin.CAT_NAME) || orig(self));
        On.Oracle.ctor += OracleCtor;
		On.Oracle.Update += OracleUpdate;
		On.Player.Update += PlayerUpdate;
		On.SaveState.SessionEnded += SavePebbles;

		On.MoreSlugcats.SSOracleRotBehavior.Update += RotBehavior_Update;

		// On.GhostConversation.AddEvents += GhostOVerride;
		On.MoreSlugcats.SSOracleRotBehavior.RMConversation.AddEvents += PebblesOverride;
		On.SLOracleBehaviorHasMark.MoonConversation.AddEvents += MoonOverride;
		var msg = "APPLIED CONVS";
		Debug.Log(msg);
	}

    private static void SavePebbles(On.SaveState.orig_SessionEnded orig, SaveState self, RainWorldGame game, bool survived, bool newMalnourished)
    {
        orig(self, game, survived, newMalnourished);
        if (IsLeader(game) && pebblesMeeted)
        {
			File.WriteAllText(AssetManager.ResolveFilePath("data/pebblesMet.txt"), "true");
            Debug.Log("SETTING pebblesMet TO true");
        }
        else
        {
            Debug.Log("PIZDEC");
        }
    }

    private static void RotBehavior_Update(On.MoreSlugcats.SSOracleRotBehavior.orig_Update orig, SSOracleRotBehavior self, bool eu)
    {
		orig(self, eu);
		//self.InitateConversation(Enums.Pebbles_Leader_FirstMeet);
    }

    public static void OracleCtor(On.Oracle.orig_ctor orig, Oracle self, AbstractPhysicalObject abstractPhysicalObject, Room room)
	{
		orig(self, abstractPhysicalObject, room);
		OracleCWT.Add(self, new StrongBox<int>(0));
	}
	public static void OracleUpdate(On.Oracle.orig_Update orig, Oracle self, bool eu)
	{
		orig(self, eu);
		var game = (RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame);

		if (self.room.game.IsLeader())
		{
			if (self.oracleBehavior is SSOracleRotBehavior rotBehavior)
			{
				string metPebbles = File.ReadLines(AssetManager.ResolveFilePath("data/pebblesMet.txt")).First();

				if (OracleCWT.TryGetValue(self, out var Counter) && rotBehavior.conversation?.events != null)
				{
					if (metPebbles == "false")
					{
						{
							Counter.Value++;
							const int StartPain = 1244;
							const int GiveMark = 1;
							const int FixPearl = 7600;
							const int StartSecondPain = FixPearl + 700;
							//const int FixPearl = 1;
							const int MindRead = 3930;
							//const int MindRead = 1;
							if (Counter.Value >= StartPain - 35 && Counter.Value <= StartPain)
							{
								for (int i = 1; i < 3; i++)
								{
									bool lockToHorizontal = Random.Range(0f, 1f) > 0.5f;
									Spark spark = new Spark(new Vector2(lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 1225 : 1795) : Random.Range(1225, 1795), !lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 805 : 1375) : Random.Range(800, 1375)), RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.yellow, null, 10, 30);
									self.room.AddObject(spark);

									if (i == 1 && Counter.Value % 2 == 0)
									{
										lockToHorizontal = Random.Range(0f, 1f) > 0.5f;
										Vector2 startPos = new Vector2(lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 1225 : 1795) : Random.Range(1225, 1795), !lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 805 : 1375) : Random.Range(800, 1375));
										Vector2 endPos = new Vector2(!lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 1225 : 1795) : Random.Range(1225, 1795), lockToHorizontal ? (Random.Range(0f, 1f) > 0.5f ? 805 : 1375) : Random.Range(800, 1375));
										LightningBolt lightning = new LightningBolt(startPos, endPos, 0, 0.2f, 1, 0, 0f, false);
										lightning.intensity = 1f;
										lightning.color = new Color(Random.Range(0.85f, 1f), Random.Range(0f, 0.196f), Random.Range(0f, 0.456f));
										self.room.AddObject(lightning);
										self.room.PlaySound(SoundID.Zapper_Zap, lightning.from, 0.6f, 1f);
										self.room.PlaySound(SoundID.Zapper_Disrupted_LOOP, lightning.target, 0.6f, 1f);
									}
								}
								self.bodyChunks[0].vel += new Vector2(Random.Range(-3.65f, 3.65f), Random.Range(-1f, 1f));
							}
							if (Counter.Value == StartPain)
							{
								self.room.AddObject(new PebblesPanicDisplay(self));
							}
							if (Counter.Value == GiveMark)
							{
								self.room.AddObject(new PebblesGiveMark(self));
							}
							if (Counter.Value == FixPearl)
							{
								self.room.AddObject(new PebblesFixPearl(self));
							}
							if (Counter.Value == MindRead)
							{
								self.room.AddObject(new PebblesMindRead(self));
							}
							if (Counter.Value == StartSecondPain)
							{
								self.room.AddObject(new PebblesPanicDisplay(self));
							}
						}
					}
					else
					{
						Counter.Value++;
					}
				}
			}
		}
	}
	public static void PlayerUpdate(On.Player.orig_Update orig, Player player, bool eu)
	{
		orig(player, eu);
		playerPos = player.mainBodyChunk.pos;
	}
	public static void MoonOverride(On.SLOracleBehaviorHasMark.MoonConversation.orig_AddEvents orig, SLOracleBehaviorHasMark.MoonConversation self)
	{
		orig(self);
		if (CustomConversations.TryGet(self.myBehavior.oracle.room.game, out bool value) && value)
		{
			if (self.id == Conversation.ID.MoonFirstPostMarkConversation)
			{
				self.events = new List<Conversation.DialogueEvent>();
				switch (Mathf.Clamp(self.State.neuronsLeft, 0, 5)) //this gets the number of neurons left for moon.
				{
					/*case 0:
						break;
					case 1:
						self.events.Add(new Conversation.TextEvent(self, 0, "...", 0));
						return;
					case 2:
						self.events.Add(new Conversation.TextEvent(self, 0, "... 2 NEURONS LEFT!", 0));
						return;
					case 3:
						self.events.Add(new Conversation.TextEvent(self, 0, "3 NEURON CONVO!", 0));
						return;
					case 4:
						self.events.Add(new Conversation.TextEvent(self, 0, "4 NEURON CONVO!!!", 0));
						return;*/
					case 5:
						self.events = new List<Conversation.DialogueEvent>() {
						new Conversation.TextEvent(self, 0, Translate("Hello little creature."), 0),
                        new Conversation.TextEvent(self, 0, Translate("Your shape is familiar to me. I feel as if I... might have met your kind before."), 0),
                        new Conversation.TextEvent(self, 0, Translate("But my memory is so unreliable now."), 0),
                        new Conversation.TextEvent(self, 0, Translate("I see that someone has given you the gift of communication. Must have been Five Pebbles, as you don't look like you can travel very far..."), 0),
                        new Conversation.TextEvent(self, 0, Translate("He's sick, you know. Being corrupted from the inside by his own experiments.<LINE> Maybe they all are by now, who knows. We weren't designed to transcend and it drives us mad."), 0),
                        new Conversation.TextEvent(self, 0, Translate("Hm..."), 0),
                        new Conversation.TextEvent(self, 0, Translate("Such a familiar feeling."), 0)
                        };
						return;

				}
			}
		}
	}
	public static string Translate(String s)
	{
		return RWCustom.Custom.rainWorld.inGameTranslator.Translate(s);
	}
	public static void PebblesOverride(On.MoreSlugcats.SSOracleRotBehavior.RMConversation.orig_AddEvents orig, SSOracleRotBehavior.RMConversation self)
	{
		orig(self);
		var game = (RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame);
        string metPebbles = File.ReadLines(AssetManager.ResolveFilePath("data/pebblesMet.txt")).First();
        if (CustomConversations.TryGet(self.owner.oracle.room.game, out bool custom) && custom && self.owner.oracle.room.game.session is StoryGameSession session && session.saveState.saveStateNumber.value == Plugin.CAT_NAME)
		{
            if (!game.GetStorySession.saveState.deathPersistentSaveData.theMark)
			{     
                if (metPebbles == "false") //Pebbles_White is the default convo as far as i can see.
				{
					self.events = new List<Conversation.DialogueEvent>() {
						new Conversation.TextEvent(self, 1000, "...", 103),
						new Conversation.TextEvent(self, 5, Translate("Now you can understand meeeeeee-"), 20),
						new Conversation.TextEvent(self, 0, "......", 60),
						new Conversation.TextEvent(self, 5, "...", 40),
						new Conversation.TextEvent(self, 0, Translate("Giving you mark of communication wasn't good idea."), 90),
						new Conversation.TextEvent(self, 0, Translate("This caused some ... consequences."), 80),
						new Conversation.TextEvent(self, 0, Translate("This mark is the only thing I can offer you. I cannot help you. I can't even help myself."), 120),
						new Conversation.TextEvent(self, 0, Translate("I've long since lost control of most of my facilities, and my construction is rotten through.."), 130),
						new Conversation.TextEvent(self, 0, Translate("The most of my generators has also reached the end of its operating life."), 110),
						new Conversation.TextEvent(self, 0, Translate("How you've made it this far alive, I do not know..."), 90),
						new Conversation.TextEvent(self, 0, Translate("But if you value your life, leave and never return."), 80),
						new Conversation.TextEvent(self, 200, Translate("..."), 20),
						new Conversation.TextEvent(self, 0, Translate("What?"), 80),
						new Conversation.TextEvent(self, 0, Translate("..."), 20),
						new Conversation.TextEvent(self, 0, Translate("Looks like you smart enough for trying to use mark to ... transfer some data?"), 110),
						new Conversation.TextEvent(self, 0, Translate("Let me just..."), 90),
						new Conversation.TextEvent(self, 730, Translate("..."), 30),
						new Conversation.TextEvent(self, 0, Translate("If I correctly interpret the mental images you conveyed, your home was flooded due to my activity."), 130),
						new Conversation.TextEvent(self, 0, Translate("I think I can help you."), 20),
						new Conversation.TextEvent(self, 0, Translate("I once met someone of your kind. This creature carried with it an important cargo and a data pearl.<LINE>I managed to copy its contents."), 150),
						new Conversation.TextEvent(self, 0, Translate("That pearl contained a sequence that targeted the Karma Gates. Theoretically, it could open any gate by interfering its current."), 150),
                        new Conversation.TextEvent(self, 0, Translate("I'll try to write this sequence on one of my pearls. Wait a bit..."), 110),
                        new Conversation.TextEvent(self, 600, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("Sequence writing was succeeeeeeeee-"), 70),
                        new Conversation.TextEvent(self, 0, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("Take this pearl and find a new home for your tribe. Then for your own sake, never return here."), 130)
					};
					if (OracleCWT.TryGetValue(self.owner.oracle, out var Counter)) { Counter.Value = 0; }

				}
				else
				{
					//var msg = "PEBBLES MET: " + pebblesMet;
					//Debug.Log(msg);
					self.events = new List<Conversation.DialogueEvent>() {
						new Conversation.TextEvent(self, 0, Translate("You again? I have nothing for you. Leave."), 103)
					};
                    if (OracleCWT.TryGetValue(self.owner.oracle, out var Counter)) { Counter.Value = 0; }
                }

            }
			else
			{
                if (metPebbles == "false") //Pebbles_White is the default convo as far as i can see.
                {
                    self.events = new List<Conversation.DialogueEvent>() {
                        new Conversation.TextEvent(self, 1000, "...", 103),
                        new Conversation.TextEvent(self, 5, Translate("You have mark?"), 20),
                        new Conversation.TextEvent(self, 0, "......", 60),
                        new Conversation.TextEvent(self, 5, "...", 40),
                        new Conversation.TextEvent(self, 0, Translate("Giving you mark of communication wasn't good idea."), 90),
                        new Conversation.TextEvent(self, 0, Translate("This caused some ... consequences."), 80),
                        new Conversation.TextEvent(self, 0, Translate("This mark is the only thing I can offer you. I cannot help you. I can't even help myself."), 120),
                        new Conversation.TextEvent(self, 0, Translate("I've long since lost control of most of my facilities, and my construction is rotten through.."), 130),
                        new Conversation.TextEvent(self, 0, Translate("The most of my generators has also reached the end of its operating life."), 110),
                        new Conversation.TextEvent(self, 0, Translate("How you've made it this far alive, I do not know..."), 90),
                        new Conversation.TextEvent(self, 0, Translate("But if you value your life, leave and never return."), 80),
                        new Conversation.TextEvent(self, 200, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("What?"), 80),
                        new Conversation.TextEvent(self, 0, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("Looks like you smart enough for trying to use mark to ... transfer some data?"), 110),
                        new Conversation.TextEvent(self, 0, Translate("Let me just..."), 90),
                        new Conversation.TextEvent(self, 730, Translate("..."), 30),
                        new Conversation.TextEvent(self, 0, Translate("If I correctly interpret the mental images you conveyed, your home was flooded due to my activity."), 130),
                        new Conversation.TextEvent(self, 0, Translate("I think I can help you."), 20),
                        new Conversation.TextEvent(self, 0, Translate("I once met someone of your kind. This creature carried with it an important cargo and a data pearl.<LINE>I managed to copy its contents."), 150),
                        new Conversation.TextEvent(self, 0, Translate("That pearl contained a sequence that targeted the Karma Gates. Theoretically, it could open any gate by interfering its current."), 150),
                        new Conversation.TextEvent(self, 0, Translate("I'll try to write this sequence on one of my pearls. Wait a bit..."), 110),
                        new Conversation.TextEvent(self, 600, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("Sequence writing was succeeeeeeeee-"), 70),
                        new Conversation.TextEvent(self, 0, Translate("..."), 20),
                        new Conversation.TextEvent(self, 0, Translate("Take this pearl and find a new home for your tribe. Then for your own sake, never return here."), 130)
                    };
                    if (OracleCWT.TryGetValue(self.owner.oracle, out var Counter)) { Counter.Value = 0; }

                }
                else
                {
                    //var msg = "PEBBLES MET: " + pebblesMet;
                    //Debug.Log(msg);
                    self.events = new List<Conversation.DialogueEvent>() {
                        new Conversation.TextEvent(self, 0, Translate("You again? I have nothing for you. Leave."), 103)
                    };
                    if (OracleCWT.TryGetValue(self.owner.oracle, out var Counter)) { Counter.Value = 0; }
                }
            }
		}
	}
	public static void GhostOVerride(On.GhostConversation.orig_AddEvents orig, GhostConversation self)
	{
		orig(self);
		if (CustomConversations.TryGet(self.ghost.room.game, out bool value) && value) //check if this game has custom conversations
		{
			if (self.id == Conversation.ID.Ghost_CC) //check for which ghost this is
			{
				self.events = new List<Conversation.DialogueEvent>(); //remove all events already existing
				self.events.Add(new Conversation.TextEvent(self, 0, "This is a test!", 0)); //add in your own text events!
				self.events.Add(new Conversation.TextEvent(self, 0, "This should be the second text!", 0));
			}
		}
	}
	public class PebblesMindRead : UpdatableAndDeletable
	{
		Oracle self;
		private int timer;
		private int startDisplay = 1100;
        private int endDisplay = 1700;
		private int[] array = {20, 50, 100, 100, 200, 200, 300, 150 };
		private Vector2 imagePos = new Vector2(1483.1f, 1098.5f);
		private bool screenExists = false;
		private bool image2Exists = false;
		OracleProjectionScreen myScreen;
		ProjectedImage image;
        ProjectedImage image2;



        public PebblesMindRead(Oracle self)
		{
			this.self = self;
			//self.room.PlaySound(SoundID.SS_AI_Talk_1, self.firstChunk, false, 1f, 1f);
    }

		public override async void Update(bool eu)
		{
			if (!screenExists)
			{
                myScreen = new OracleProjectionScreen(self.room, null);
				screenExists = true;
            }
			var game = (RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame);
			base.Update(eu);

            timer++;
            if (timer == 1)
			{
                self.room.game.FirstAlivePlayer.realizedCreature.stun = 200;
            }
			if (timer == 1 || timer == 10 || timer == 14 || timer == 20 || timer == 35 || timer == 41 || timer == 67 || timer == 77 || timer == 90)
			{
                bool lockToHorizontal = Random.Range(0f, 1f) > 0.5f;
				for (int i = 1; i < new System.Random().Next(10, 20); i++)
				{
					Spark sparkPlayer = new Spark(playerPos, RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.white, null, 10, 30);
                    self.room.AddObject(sparkPlayer);
                }
                self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Throw_FireSpear, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk);
                    //Spark sparkOracle = new Spark(self.RandomChunk.pos, RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.white, null, 10, 30);
                    //self.room.AddObject(sparkOracle);
 
            }
			if (timer == (startDisplay - 60) || timer == (startDisplay - 90) || timer == (startDisplay - 120))
			{
                self.room.PlaySound(SoundID.SS_AI_Text, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk.pos, 1.5f, 1f);
            }
			if (timer == startDisplay)
			{
				self.room.PlaySound(SoundID.SS_AI_Text, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk.pos, 1.5f, 1f);
                image = myScreen.AddImage("Home");
				image.pos = imagePos;
				image.setAlpha = 0.9f;

            }
			/*if (timer == (startDisplay + 100))
			{
                image2 = myScreen.AddImage("chieftain");
				image2.pos = imagePos;
				image2.setAlpha = 1.0f;
            }*/
			if (timer < endDisplay && timer > startDisplay && (timer % array[new System.Random().Next(0, array.Length)]) == 0)
			{
				imagePos = new Vector2(imagePos.x + Random.Range(-20.0f, 20.0f), imagePos.y + Random.Range(-20.0f, 20.0f));
				image.pos = imagePos;
				if (timer > (startDisplay + 100))
				{
					image2.pos = imagePos;
				}
			}
			if ((timer + 50) < endDisplay && (timer % 30) == 0 && timer > startDisplay + 150)
			{
				if (image2Exists)
				{
                    self.room.PlaySound(SoundID.SS_AI_Text, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk.pos, 1.5f, 1f);
                    myScreen.RemoveImage("chieftain");
					image2Exists = false;
                }
				else
				{
                    self.room.PlaySound(SoundID.SS_AI_Text, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk.pos, 1.5f, 1f);
                    image2 = myScreen.AddImage("chieftain");
                    image2.pos = imagePos;
					image.setAlpha = 1.0f;
                    image2Exists = true;
                }
				
			}
			if (timer == endDisplay)
			{
                self.room.PlaySound(SoundID.SS_AI_Text, self.room.game.FirstAlivePlayer.realizedCreature.firstChunk.pos, 1.5f, 1f);
                myScreen.RemoveImage("Home");
				myScreen.RemoveImage("chieftain");
                Destroy();
            }
		}
	}
	public class PebblesGiveMark : UpdatableAndDeletable
	{
		Oracle self;
		private int timer;
		public PebblesGiveMark(Oracle self)
		{
			this.self = self;
            //self.room.PlaySound(SoundID.SS_AI_Talk_1, self.firstChunk, false, 1f, 1f);
		}

		public override void Update(bool eu)
		{
			var game = (RWCustom.Custom.rainWorld.processManager.currentMainLoop as RainWorldGame);
			base.Update(eu);
			timer++;
			if (timer == 50)
			{
				self.room.PlaySound(SoundID.SS_AI_Talk_4, self.firstChunk, false, 1f, 1f);
			}
			if (timer == 150)
			{
				self.room.PlaySound(SoundID.SS_AI_Talk_3, self.firstChunk, false, 1f, 1f);
			}
			if (timer == 170)
			{
				self.room.PlaySound(SoundID.SS_AI_Talk_3, self.firstChunk, false, 1f, 1f);
			}
			if (timer == 250)
			{
				self.room.PlaySound(SoundID.SS_AI_Talk_2, self.firstChunk, false, 1f, 1f);
			}
			if (timer == 600)
			{
				self.room.PlaySound(SoundID.SS_AI_Talk_1, self.firstChunk, false, 1f, 1f);
            }
			if (timer == 800)
			{
				for (int i = 1; i < 20; i++)
				{
					bool lockToHorizontal = Random.Range(0f, 1f) > 0.5f;
					Spark sparkPlayer = new Spark(playerPos, RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.white, null, 10, 30);
					//Spark sparkOracle = new Spark(self.RandomChunk.pos, RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.white, null, 10, 30);
					self.room.AddObject(sparkPlayer);
					//self.room.AddObject(sparkOracle);
				}
				game.GetStorySession.saveState.deathPersistentSaveData.theMark = true;
                self.room.game.FirstAlivePlayer.realizedCreature.stun = 100;
                self.room.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, self.firstChunk, false, 1f, 1f);
                Destroy();
            }
		}
	}
    public class PebblesFixPearl : UpdatableAndDeletable
    {
        Oracle self;
        private int timer;
        private DataPearl pearl1;
        private DataPearl pearl;
        private Vector2 wantPos = new Vector2(1530.1f, 800.5f);
        private DataPearl.AbstractDataPearl writedPearl;

        //var oracleModule = OracleRMData.GetValue(self, _ => new OracleRMModule());
        public PebblesFixPearl(Oracle self)
        {
			this.self = self;
            //self.room.PlaySound(SoundID.SS_AI_Talk_1, self.firstChunk, false, 1f, 1f);
            timer = 0;
            pearl = null;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            timer++;
			if (timer == 1)
			{
				self.oracleBehavior.lookPoint = wantPos;
 				eyesClosed = true;
				Debug.Log("Seaching for pearl to fix");
				List<PhysicalObject>[] physicalObjects = self.room.physicalObjects;
				for (int i = 0; i < physicalObjects.Length; i++)
				{
					for (int j = 0; j < physicalObjects[i].Count; j++)
					{
						PhysicalObject physicalObject = physicalObjects[i][j];
						if (physicalObject is DataPearl && (!(physicalObject is HalcyonPearl)))
						{
                            pearl1 = physicalObject as DataPearl;
                            if (pearl1.color.r == 0.7f)
							{
								pearl = physicalObject as DataPearl;
							}
						}
					}
				}
                pearl.firstChunk.HardSetPosition(wantPos);
				//myPearl = CustomRegions.Mod.CustomWorldMod.customPearls.GetEnumerator()
				//pearl.AbstractDataPearl.DataPearlType = CustomRegions.Mod.CustomWorldMod.customPearls.;

            }
            if (timer > 1 && timer < 600 && pearl != null)
            {
                if (timer % 20 == 0)
                {
                    self.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Data_Bit, pearl.firstChunk.pos, 1f, 1f + Random.value * 2f);
                    self.room.AddObject(new Explosion.ExplosionLight(pearl.firstChunk.pos, 150f, 1f, 15, Color.blue));
					for (int i = 1; i < 4; i++)
					{
						self.room.AddObject(new Spark(pearl.firstChunk.pos, RWCustom.Custom.rotateVectorDeg(Vector2.down * (i * 2.5f), Random.Range(0f, 360f)), Color.blue, null, 20, 40));
					}
                }
                pearl.firstChunk.vel *= Custom.LerpMap(pearl.firstChunk.vel.magnitude, 1f, 6f, 0.999f, 0.9f);
                pearl.firstChunk.vel += Vector2.ClampMagnitude(wantPos - pearl.firstChunk.pos, 100f) / 100f * 0.4f;
                //抵消重力
                pearl.firstChunk.vel += 0.6f * Vector2.up;
                //随机速度
                pearl.firstChunk.vel += (Random.value - 0.5f) * 0.05f * Vector2.up;
				pearl.color.r = pearl.color.r - 0.0012f;
				pearl.color.g = pearl.color.g - 0.0012f;
            }
            if (timer == 600 && pearl != null)
            {
                self.room.PlaySound(SoundID.Moon_Wake_Up_Green_Swarmer_Flash, pearl.firstChunk.pos, 0.5f, 1f);
                self.room.AddObject(new ElectricFullScreen.SparkFlash(pearl.firstChunk.pos, 50f));
            }
            if (timer == 620)
            {
				eyesClosed = false;
                PearlWritedSave.pearlWrited = true;
				pearl.RemoveFromRoom();
				WorldCoordinate writedPearlPos = pearl.AbstractPearl.pos;
				writedPearl = new DataPearl.AbstractDataPearl(self.room.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, writedPearlPos, self.room.game.GetNewID(), -1, -1, null, Enums.FixedPebblesPearl);
				writedPearl.RealizeInRoom();
                pebblesMeeted = true;
                self.room.AddObject(new WritedDataPearlEffect(pearl, self.room));
                Destroy();
            }
			
        }
    }
    public class PebblesPanicDisplay : UpdatableAndDeletable
	{
		Oracle oracle;
		private int[] timings;
		private int timer;
		public PebblesPanicDisplay(Oracle oracle)
		{
			this.oracle = oracle;
			timings = new int[] {
					140,
					190,
					210,
					240,
					320,
					30
				};
			eyesClosed = true;
			oracle.stun = 300;
			oracle.dazed = 300;
			(oracle.oracleBehavior as SSOracleRotBehavior)?.AirVoice(SoundID.SS_AI_Talk_4);
			oracle.room.PlaySound(SoundID.SL_AI_Pain_1, oracle.firstChunk, false, 0.7f, 0.3f);
			oracle.bodyChunks[1].vel.x -= 5f;
			Debug.Log(oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.DarkenLights)?.amount);
			Debug.Log(oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Darkness)?.amount);
			Debug.Log(oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Contrast)?.amount);
		}
		public override void Update(bool eu)
		{
			base.Update(eu);
			timer++;
			if (timer == 1)
			{
				if (oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.DarkenLights) == null)
				{
					oracle.room.roomSettings.effects.Add(new RoomSettings.RoomEffect(RoomSettings.RoomEffect.Type.DarkenLights, 0f, false));
				}
				if (oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Darkness) == null)
				{
					oracle.room.roomSettings.effects.Add(new RoomSettings.RoomEffect(RoomSettings.RoomEffect.Type.Darkness, 0f, false));
				}
				if (oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Contrast) == null)
				{
					oracle.room.roomSettings.effects.Add(new RoomSettings.RoomEffect(RoomSettings.RoomEffect.Type.Contrast, 0f, false));
				}
				oracle.room.PlaySound(SoundID.Broken_Anti_Gravity_Switch_Off, 0f, 1f, 1f);
			}
			if (timer < timings[0])
			{
				float t = (float)timer / timings[0];
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.DarkenLights).amount = Mathf.Lerp(0f, 0.6f, t);
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Darkness).amount = Mathf.Lerp(0f, 0.4f, t);
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Contrast).amount = Mathf.Lerp(0f, 0.3f, t);
			}
			if (timer == timings[0])
			{
				oracle.arm.isActive = false;
				oracle.setGravity(0.9f);
				oracle.stun = 9999;
				//oracle.room.PlaySound(MoreSlugcatsEnums.MSCSoundID.Moon_Panic_Attack, 0f, 0.85f, 1f);
				for (int i = 0; i < oracle.room.game.cameras.Length; i++)
				{
					if (oracle.room.game.cameras[i].room == oracle.room && !oracle.room.game.cameras[i].AboutToSwitchRoom)
					{
						oracle.room.game.cameras[i].ScreenMovement(null, Vector2.zero, 15f);
					}
				}
			}
			if (timer == (timings[1] + timings[2]) / 2)
			{
				oracle.arm.isActive = false;
				// oracle.room.PlaySound((Random.value < 0.5f) ? SoundID.SL_AI_Pain_1 : SoundID.SL_AI_Pain_2, 0f, 0.5f, 1f);
				// chatLabel = new OracleChatLabel(oracle.oracleBehavior);
				// chatLabel.pos = new Vector2(485f, 360f);
				// chatLabel.NewPhrase(99);
				oracle.setGravity(0.9f);
				oracle.stun = 9999;
				// oracle.room.AddObject(chatLabel);
			}
			if (timer > timings[1] && timer < timings[2] && timer % 5 == 0)
			{
				oracle.room.ScreenMovement(null, new Vector2(0f, 0f), 2.5f);
				// for (int j = 0; j < 6; j++)
				// {
				//     if (Random.value < 0.5f)
				//     {
				//         oracle.room.AddObject(new OraclePanicDisplay.PanicIcon(new Vector2((float)Random.Range(230, 740), (float)Random.Range(100, 620))));
				//     }
				// }
			}
			if (timer >= timings[2] && timer <= timings[3])
			{
				oracle.room.ScreenMovement(null, new Vector2(0f, 0f), 1f);
			}
			if (timer == timings[3])
			{
				// chatLabel.Destroy();
				oracle.room.PlaySound(SoundID.Broken_Anti_Gravity_Switch_On, 0f, 1f, 1f);
			}
			if (timer > timings[3])
			{
				float t2 = (float)(timer - timings[3]) / (int)(timings[0] / 2.1f);
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.DarkenLights).amount = Mathf.Lerp(0.6f, 0f, t2);
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Darkness).amount = Mathf.Lerp(0.4f, 0f, t2);
				oracle.room.roomSettings.GetEffect(RoomSettings.RoomEffect.Type.Contrast).amount = Mathf.Lerp(0.3f, 0f, t2);
			}
			if (timer == timings[3] + (int)(timings[0] / 2.1f))
			{
				oracle.setGravity(0f);
				oracle.arm.isActive = true;
				oracle.stun = 50;
			}
			if (timer > timings[3] + (int)(timings[0] / 2.1f) && timer < timings[4])
			{
				oracle.bodyChunks[0].vel += new Vector2(Random.Range(-1.7f, 1.7f), Random.Range(-1.2f, 1.2f));
				oracle.bodyChunks[0].vel *= 0.3f;
			}
			if (timer == timings[4])
			{
				//while (RoomSettings.RoomEffect.Type.DarkenLights != null && RoomSettings.RoomEffect.Type.Darkness != null && RoomSettings.RoomEffect.Type.Contrast != null)
				//{
				//	oracle.room.roomSettings.effects.RemoveAll(eff => eff.type == RoomSettings.RoomEffect.Type.DarkenLights || eff.type == RoomSettings.RoomEffect.Type.Darkness || eff.type == RoomSettings.RoomEffect.Type.Contrast);
				//}
            }
			if (timer == timings[5])
			{
				(oracle.oracleBehavior as SSOracleRotBehavior)?.AirVoice(SoundID.SS_AI_Talk_1);
				oracle.room.PlaySound(SoundID.SL_AI_Pain_2, oracle.firstChunk, false, 0.87f, 0.45f);
			}
			if (timer == (timings[4] + 100))
			{
                eyesClosed = false;
                Destroy();
            }
		}
	}
}