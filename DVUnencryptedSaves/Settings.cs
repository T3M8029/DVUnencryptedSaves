using UnityModManagerNet;

namespace DVUnencryptedSaves
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		[Draw("Enable saving as JSON")]
		public bool SaveJSON = true;

		[Draw("Load from JSON - needed if you want your changes to reflect in-game")]
		public bool LoadFromJSON = true;
		public override void Save(UnityModManager.ModEntry entry)
		{
			Save(this, entry);
		}
		public void OnChange() { }
	}
}
