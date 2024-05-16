
using System.Collections.Generic;
using UnityEngine;

namespace TheLeader;

public class SaveMiscProgression
{
    // Meta
    public bool IsNewLeaderSave { get; set; } = true;

    // Story

    public bool IsPearlpupSick { get; set; }
    public bool HasOEEnding { get; set; }
    public bool JustAscended { get; set; }
    public bool Ascended { get; set; }
    public bool HasTrueEnding { get; set; }


    public void ResetSave()
    {

        IsNewLeaderSave = true;

        HasOEEnding = false;

        JustAscended = false;
        Ascended = false;
        HasTrueEnding = false;
    }
}