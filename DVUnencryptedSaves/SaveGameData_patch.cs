using HarmonyLib;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DVUnencryptedSaves
{
	[HarmonyPatch]
	public static class SaveGameData_patch
	{
		[HarmonyPatch(typeof(SaveGameData), "SaveToFile")]
		[HarmonyPostfix]
		public static void SaveToFile_Postfix(SaveGameData data, string path)
		{
			if (Main.Settings.SaveJSON)
			{
				string jsonString = data.GetJsonString();
				JObject jsonObj = JObject.Parse(jsonString);
				string formattedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
				path = Path.ChangeExtension(path, ".json");
				Main._modEntry.Logger.Log("Output file path is: " + path);
				//Main._modEntry.Logger.Log(formattedJson);
				File.WriteAllText(path, formattedJson);
			}
		}
	}
}
