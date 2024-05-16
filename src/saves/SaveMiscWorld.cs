using System.Collections.Generic;

namespace TheLeader;

public class SaveMiscWorld
{
    // Meta
    public bool JustBeatAltEnd { get; set; }
    public bool JustMiraSkipped { get; set; }


    // Pearlcat
    public Dictionary<int, List<string>> Inventory { get; } = new();
    public Dictionary<int, int?> ActiveObjectIndex { get; } = new();

    // Five Pebbles
    public int PebblesMeetCount { get; set; }
    public bool PebblesTookHalcyonPearl { get; set; } = false;

    // Dreams
    public string? CurrentDream { get; set; }
    public List<string> PreviousDreams { get; } = new();
}