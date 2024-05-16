using Menu;
using MoreSlugcats;
using Music;
using SlugBase.SaveData;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TheLeader;

public static class ModuleManager
{
	public static SaveMiscWorld? GetMiscWorld(this RainWorldGame game) => game.IsStorySession ? GetMiscWorld(game.GetStorySession.saveState.miscWorldSaveData) : null;
	public static SaveMiscWorld GetMiscWorld(this MiscWorldSaveData data)
	{
		if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out SaveMiscWorld save))
		{
			data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
		}

		return save;
	}

	public static SaveMiscProgression GetMiscProgression(this RainWorld rainWorld) => GetMiscProgression(rainWorld.progression.miscProgressionData);
	public static SaveMiscProgression GetMiscProgression(this PlayerProgression.MiscProgressionData data)
	{
		if (!data.GetSlugBaseData().TryGet(Plugin.MOD_ID, out SaveMiscProgression save))
		{
			data.GetSlugBaseData().Set(Plugin.MOD_ID, save = new());
		}

		return save;
	}

}