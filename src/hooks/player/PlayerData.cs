namespace TheLeader;
public static partial class Hooks
{
    public static bool IsLeader(this RainWorldGame? game) => game?.StoryCharacter == Enums.Leader;
    public static void ApplyPlayerHooks()
    {
        On.Player.GraspsCanBeCrafted += Player_GraspsCanBeCrafted;
    }
    private static bool Player_GraspsCanBeCrafted(On.Player.orig_GraspsCanBeCrafted orig, Player self)
    {
        if (self.slugcatStats.name == Enums.Leader && (self.CraftingResults() != null))
        {
            return true;
        }
        return orig(self);
    }
}