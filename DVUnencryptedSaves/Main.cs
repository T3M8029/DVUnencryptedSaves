using System;
using System.Reflection;
using HarmonyLib;
using UnityModManagerNet;

namespace DVUnencryptedSaves
{
	public class Main
	{
		public static UnityModManager.ModEntry _modEntry;
		
		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Harmony? harmony = null;
			_modEntry = modEntry;
			try
			{
				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());

			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				harmony?.UnpatchAll(modEntry.Info.Id);
				return false;
			}
			return true;
		}
	}
}
