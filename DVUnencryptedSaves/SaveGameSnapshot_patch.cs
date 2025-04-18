using DV.Common;
using DV.UserManagement;
using DV.UserManagement.Data;
using DV.Utils;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Text;

namespace DVUnencryptedSaves
{
	[HarmonyPatch]
	public static class SaveGameSnapshot_patch
	{
		private static FieldInfo _dataBackingField = AccessTools.Field(typeof(SaveGameSnapshot), "<Data>k__BackingField");

		[HarmonyPatch(typeof(SaveGameSnapshot), "LoadData")]
		[HarmonyPrefix]
		public static bool LoadData_Prefix(SaveGameSnapshot __instance)
		{
			if (!Main.Settings.LoadFromJSON) return true;
			if (_dataBackingField == null)
			{
				Main._modEntry.Logger.Error("Could not find backing field for 'Data' property during initialization. Cannot load from JSON.");
				return true;
			}
			try
			{
				ISaveGame saveGame = __instance as ISaveGame;
				if (saveGame == null || string.IsNullOrEmpty(saveGame.BasePath))
				{
					Main._modEntry.Logger.Warning("LoadData_Prefix: Could not get BasePath from the provided SaveGameSnapshot instance. Letting original LoadData run.");
					return true;
				}
				var um = SingletonBehaviour<UserManager>.Instance;
				if (um == null)
				{
					Main._modEntry.Logger.Warning("LoadData_Prefix: UserManager instance is null. Letting original LoadData run.");
					return true;
				}
				var storage = um.Storage;
				if (storage == null)
				{
					Main._modEntry.Logger.Warning("LoadData_Prefix: UserManager.Storage is null. Letting original LoadData run.");
					return true;
				}

				string relativeBasePath = saveGame.BasePath;
				string absoluteBasePath;
				try
				{
					absoluteBasePath = storage.GetFilesystemPath(relativeBasePath);
				}
				catch (System.Exception pathEx)
				{
					Main._modEntry.Logger.Error($"LoadData_Prefix: Error getting filesystem path for '{relativeBasePath}'. Exception: {pathEx.Message}. Letting original LoadData run.");
					return true;
				}
				if (string.IsNullOrEmpty(absoluteBasePath))
				{
					Main._modEntry.Logger.Warning($"LoadData_Prefix: GetFilesystemPath returned null or empty for '{relativeBasePath}'. Letting original LoadData run.");
					return true;
				}
				string jsonPath = Path.ChangeExtension(absoluteBasePath, ".json");
				Main._modEntry.Logger.Log("Attempting to load from JSON path: " + jsonPath);
				if (!File.Exists(jsonPath))
				{
					Main._modEntry.Logger.Log($"JSON file not found at '{jsonPath}', letting original LoadData run.");
					return true;
				}
				string jsonContent = File.ReadAllText(jsonPath, Encoding.UTF8);
				JObject jsonData = JObject.Parse(jsonContent);
				_dataBackingField.SetValue(__instance, jsonData);
				Main._modEntry.Logger.Log($"Successfully loaded unencrypted save data from '{jsonPath}'.");
				return false;
			}
			catch (System.Exception ex)
			{
				Main._modEntry.Logger.Error($"Exception in LoadData_Prefix while processing '{__instance?.GetType().Name}': {ex}");
				return true;
			}
		}
	}
}
