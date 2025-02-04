using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Legacy;
using LoadOrderToolTwo.Utilities.Managers;

using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace LoadOrderTool.Legacy;
public interface IProfileItem
{
	string? GetIncludedPath();
	string? GetDisplayText();
	string GetCategoryName(); // mod or asset

}

public class LoadOrderProfile
{
	const string LOCAL_APP_DATA_PATH = "%LOCALAPPDATA%";
	const string CITIES_PATH = "%CITIES%";
	const string WS_CONTENT_PATH = "%WORKSHOP%";

	static string? FromFinalPath(string? path)
	{
		return path
			?.Replace(LocationManager.AppDataPath, LOCAL_APP_DATA_PATH)
			?.Replace(LocationManager.GamePath, CITIES_PATH)
			?.Replace(LocationManager.WorkshopContentPath, WS_CONTENT_PATH);
	}

	static string? ToFinalPath(string? path)
	{
		return path
			?.Replace(LOCAL_APP_DATA_PATH, LocationManager.AppDataPath)
			?.Replace(CITIES_PATH, LocationManager.GamePath)
			?.Replace(WS_CONTENT_PATH, LocationManager.WorkshopContentPath);
	}

	public class Mod : IProfileItem
	{

		[XmlIgnore]
		public string? IncludedPathFinal;
		public string? IncludedPath
		{
			get => FromFinalPath(IncludedPathFinal);
			set => IncludedPathFinal = ToFinalPath(value);
		}

		public bool IsEnabled;
		public bool IsIncluded;
		public int LoadOrder;
		public string? DisplayText;

		public Mod() { }

		public string? GetIncludedPath()
		{
			return IncludedPathFinal;
		}

		public string? GetDisplayText()
		{
			return DisplayText;
		}

		public string GetCategoryName()
		{
			return "Mod";
		}
	}

	public class Asset : IProfileItem
	{
		[XmlIgnore]
		public string? IncludedPathFinal;

		/// <summary>
		/// only for storage. use the final path instead
		/// </summary>
		public string? IncludedPath
		{
			get => FromFinalPath(IncludedPathFinal);
			set => IncludedPathFinal = ToFinalPath(value);
		}

		public bool IsIncluded;
		public string? DisplayText;

		public Asset() { }

		public string? GetIncludedPath()
		{
			return IncludedPathFinal;
		}

		public string? GetDisplayText()
		{
			return DisplayText;
		}

		public string GetCategoryName()
		{
			return "Asset";
		}
	}

	public Mod[] Mods = new Mod[0];
	public Asset[] Assets = new Asset[0];
	public DLC[] ExcludedDLCs = new DLC[0];

	[XmlIgnore]
	public string? SkipFilePathFinal;
	public string? SkipFilePath
	{
		get => FromFinalPath(SkipFilePathFinal);
		set => SkipFilePathFinal = ToFinalPath(value);
	}

	public bool LoadEnabled = true;
	public bool LoadUsed = true;

	public Mod GetMod(string includedPath)
	{
		return Mods.FirstOrDefault(m => m.IncludedPathFinal == includedPath);
	}

	public Asset GetAsset(string includedPath)
	{
		return Assets.FirstOrDefault(m => m.IncludedPathFinal == includedPath);
	}

	public static LoadOrderProfile? Deserialize(string path)
	{
		try
		{
			return LoadOrderShared.SharedUtil.Deserialize<LoadOrderProfile>(path);
		}
		catch
		{
			return null;
		}
	}

	internal Profile ToLot2Profile(string name)
	{
		var profile = new Profile(name)
		{
			AutoSave = false
		};

		foreach (var asset in Assets)
		{
			if (asset.IsIncluded)
			{
				var rgx = Regex.Match(asset.IncludedPath, Regex.Escape(WS_CONTENT_PATH) + "[\\\\/](\\d{8,20})[\\\\/]?");
				profile.Assets.Add(new Profile.Asset
				{
					SteamId = rgx.Success ? ulong.Parse(rgx.Groups[1].Value) : 0,
					Name = asset.DisplayText,
					RelativePath = asset.IncludedPath
				});
			}
		}

		foreach (var mod in Mods)
		{
			if (mod.IsIncluded)
			{
				var rgx = Regex.Match(mod.IncludedPath, Regex.Escape(WS_CONTENT_PATH) + "[\\\\/](\\d{8,20})[\\\\/]?");
				profile.Mods.Add(new Profile.Mod
				{
					Name = mod.DisplayText,
					SteamId = rgx.Success ? ulong.Parse(rgx.Groups[1].Value) : 0,
					RelativePath = mod.IncludedPath,
					Enabled = mod.IsEnabled
				});
			}
		}

		profile.ExcludedDLCs = ExcludedDLCs.Select(x => (uint)x).ToList();

		profile.LsmSettings.LoadEnabled = LoadEnabled;
		profile.LsmSettings.LoadUsed = LoadUsed;
		profile.LsmSettings.SkipFile = SkipFilePathFinal;
		profile.LsmSettings.UseSkipFile = LocationManager.FileExists(SkipFilePathFinal);

		return profile;
	}
}
