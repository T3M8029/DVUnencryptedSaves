using UnityModManagerNet;

namespace DVUnencryptedSaves
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		[Draw("Load from json - needed off when loading into an old save for the first time with this mod, otherwise keep on, if you want your changes to reflect in-game")]
		public bool LoadFromJSON = false;
		public override void Save(UnityModManager.ModEntry entry)
		{
			Save(this, entry);
		}
		public void OnChange() { }
	}
}
