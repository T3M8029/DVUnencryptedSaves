using HarmonyLib;
using DV.UserManagement;
using DV.Common;
using System.Collections.Generic;
using DV.UserManagement.Data;
using DV.Utils;
using System.IO;
using System.Linq;

namespace DVUnencryptedSaves
{
	[HarmonyPatch]
	public static class SaveGameManager_patch
	{
		[HarmonyPatch(typeof(SaveGameManager), "Save")]
		[HarmonyPostfix]
		public static void Save_Postfix(ISaveGame __result)
		{
			User currentUser = SingletonBehaviour<UserManager>.Instance.CurrentUser;
			string path = Path.Combine(Path.GetDirectoryName(SingletonBehaviour<UserManager>.Instance.Storage.GetFilesystemPath(currentUser.CurrentSession.LatestSave.BasePath)), Path.GetFileName(__result.BasePath));
			Main._modEntry.Logger.Log("Game is saving, running modified method with input of path: " + path);
			SaveGameManager.Instance.SaveCurrentDataEncrypted(path);
		}
	}
}
