using MoreSlugcats;
using UnityEngine;
namespace TheLeader;

public partial class Hooks
{
    private static bool notFlooded = true;

    public static void ApplyWorldHooks()
    {
        On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements;
        On.MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding.Update += OE_GourmandEnding_Update;
        On.GateKarmaGlyph.ctor += GateKarmaGlyph_ctor;
        On.RegionGate.ctor += RegionGate_Ctor;
        //On.Room.Update += Room_SetWaterLevel;
        //On.RainWorldGame.
    }
    private static void Room_SetWaterLevel(On.Room.orig_Update orig, Room room)
    {
        orig(room);
        if (room.abstractRoom.name == "OE_FINAL03" && notFlooded)
        {
            room.waterObject.fWaterLevel += 100;
            notFlooded = false;

        }
 
    }
    private static void OE_GourmandEnding_Update(On.MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding.orig_Update orig, MSCRoomSpecificScript.OE_GourmandEnding self, bool eu)
    {
        return;
        orig(self, eu);
    }
    private static bool RegionGate_customOEGateRequirements(On.RegionGate.orig_customOEGateRequirements orig, RegionGate self)
    {
        var result = orig(self);
        if (self.room.game.IsLeader())
        {
            return true;
        }
        return result;
    }
    private static void GateKarmaGlyph_ctor(On.GateKarmaGlyph.orig_ctor orig, GateKarmaGlyph self, bool side, RegionGate gate, RegionGate.GateRequirement requirement)
    {
        orig(self, side, gate, requirement);

        if (!gate.IsGateOpenForLeader()) return;

        self.requirement = RegionGate.GateRequirement.OneKarma;
    }
    private static void RegionGate_Ctor(On.RegionGate.orig_ctor orig, RegionGate self, Room room)
    {
        orig(self, room);
        if (!self.IsGateOpenForLeader()) return;
        self.karmaRequirements[0] = RegionGate.GateRequirement.OneKarma;
        self.karmaRequirements[1] = RegionGate.GateRequirement.OneKarma;
    }
    public static bool IsGateOpenForLeader(this RegionGate gate)
    {
        var roomName = gate.room?.roomSettings?.name;

        if (gate.room == null || roomName == null)
            return false;

        if (!gate.room.game.IsLeader())
            return false;

        // Metropolis gate
        /*if (roomName == "GATE_UW_LC")
            return true;*/

        /*if (roomName == "GATE_SL_MS")
            return true;*/

        if (roomName == "GATE_SB_OE")
            return true;


        return false;
    }
}