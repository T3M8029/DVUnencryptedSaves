using System;
using System.Reflection;
using DV.Utils;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace DVUnencryptedSaves
{
	public static class Main
	{
		public static UnityModManager.ModEntry _modEntry;
		public static Settings Settings { get; private set; }
		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Save(modEntry);
		}
		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Draw(modEntry);
		}

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Settings = Settings.Load<Settings>(modEntry);
			Harmony? harmony = null;
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;
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
