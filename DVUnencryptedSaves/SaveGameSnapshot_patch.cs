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
		[HarmonyPatch(typeof(SaveGameSnapshot), "LoadData")]
		[HarmonyPrefix]
		public static bool LoadData_Prefix(SaveGameSnapshot __instance)
		{
			try
			{
				string basePath = SingletonBehaviour<UserManager>.Instance.Storage.GetFilesystemPath(SingletonBehaviour<UserManager>.Instance.CurrentUser.CurrentSession.LatestSave.BasePath);
				string path = Path.ChangeExtension(basePath, ".json");
				Main._modEntry.Logger.Log("Loading from: " + path);
				if (!File.Exists(path))
				{
					Main._modEntry.Logger.Error("File not found, letting original LoadData run.");
					return true;
				}
				byte[] file = File.ReadAllBytes(path);
				string json = Encoding.UTF8.GetString(file);
				JObject jsonData = JObject.Parse(json);
				FieldInfo backingField = AccessTools.Field(typeof(SaveGameSnapshot), "<Data>k__BackingField");
				if (backingField != null)
				{
					backingField.SetValue(__instance, jsonData);
					Main._modEntry.Logger.Log("Successfully loaded unencrypted save data.");
					return false;
				}
				else
				{
					Main._modEntry.Logger.Error("ERROR: Could not find backing field for 'Data' property.");
					return true;
				}
			}
			catch (System.Exception ex)
			{
				Main._modEntry.Logger.Error("Exception in LoadData_Prefix: " + ex);
				return true;
			}
		}
	}
}
