using MoreSlugcats;
using static DataPearl.AbstractDataPearl;
using static Menu.MenuScene;
using static Menu.SlideShow;
namespace TheLeader;

public static class Enums
{
    public static SlugcatStats.Name Leader = new(nameof(Leader), false);
    public static bool registed = false;

    //public static MoreSlugcats.SSOracleRotBehavior.RMConversation MeetLeader;

    //Conversation.ID
    public static Conversation.ID Pebbles_Leader_FirstMeet;
    public static Conversation.ID Pebbles_Leader_AfterMet;
    public static void RegisterAllValues()
    {
        if (registed) return;

       //MeetLeader = new MoreSlugcats.SSOracleRotBehavior.RMConversation("MeetLeader", true);

        //GateRequirement.RegisterValues();
        registed = true;
    }

    public static void UnregisterAllValues()
    {
        if (registed)
        {
            //GateRequirement.UnregisterValues();

            Pebbles_Leader_FirstMeet.Unregister();
            Pebbles_Leader_FirstMeet = null;

            Pebbles_Leader_AfterMet.Unregister();
            Pebbles_Leader_AfterMet = null;
            //registed = false;
        }
    }
    public static DataPearlType FixedPebblesPearl = new(nameof(FixedPebblesPearl), false);
    public static RegionGate.GateRequirement LeaderLock = new RegionGate.GateRequirement("Leader", true);
    /*public static void Unregister<T>(ExtEnum<T> extEnum) where T : ExtEnum<T>
    {
        if (extEnum != null)
        {
            extEnum.Unregister();
        }
    }
    public class GateRequirement
    {
        public static void RegisterValues()
        {
            LeaderLock = new RegionGate.GateRequirement("Leader", true);
        }
        public static void UnregisterValues()
        {
            Enums.Unregister(LeaderLock);
        }
        public static RegionGate.GateRequirement LeaderLock;
    }*/
    public static class Scenes
    {
        public static SceneID Dream_Leader_Random = new(nameof(Dream_Leader_Random), false);
    }
    public static class Dreams
    {
        public static DreamsState.DreamID Dream_Leader_Random = new(nameof(Dream_Leader_Random), true);

        public static void RegisterDreams()
        {
            SlugBase.Assets.CustomDreams.SetDreamScene(Dream_Leader_Random, Scenes.Dream_Leader_Random);
        }
    }
}