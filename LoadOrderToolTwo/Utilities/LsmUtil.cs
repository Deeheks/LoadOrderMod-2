﻿using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Domain.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoadOrderToolTwo.Utilities;
internal static class LsmUtil
{
	public static bool IsValidLsmReportFile(string filePath)
	{
		if (!File.Exists(filePath) || new FileInfo(filePath).Length > 50 * 1024 * 1024)
		{
			return false;
		}

		using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		using var streamReader = new StreamReader(fileStream);
		
		while (!streamReader.EndOfStream)
		{
			var line = streamReader.ReadLine();
			
			if (Regex.IsMatch(line, "data-lomtag=\"(missingreq)|(unused)\".+?href=\"(.+?(\\d+))\">(.+?)</a>"))
			{
				return true;
			}
		}

		return false;
	}

	internal static IEnumerable<Profile.Asset> LoadMissingAssets(string obj)
	{
		var lines = File.ReadAllLines(obj);

		for (var i = 0; i < lines.Length; i++)
		{
			var match = Regex.Match(lines[i], "data-lomtag=\"missingreq\".+?href=\"(.+?(\\d+))\">(.+?)</a>");

			if (match.Success)
			{
				var steamId = match.Groups[2].Value;
				var assetName = System.Net.WebUtility.HtmlDecode(match.Groups[3].Value);

				yield return new Profile.Asset
				{
					SteamId = ulong.Parse(steamId),
					Name = assetName
				};
			}
		}
	}

	internal static IEnumerable<Profile.Asset> LoadUnusedAssets(string obj)
	{
		var lines = File.ReadAllLines(obj);

		for (var i = 0; i < lines.Length; i++)
		{
			var match = Regex.Match(lines[i], "data-lomtag=\"unused\".+?href=\"(.+?(\\d+))\">(.+?)</a>");

			if (match.Success)
			{
				var steamId = match.Groups[2].Value;
				var assetName = System.Net.WebUtility.HtmlDecode(match.Groups[3].Value);

				yield return new Profile.Asset
				{
					SteamId = ulong.Parse(steamId),
					Name = assetName
				};
			}
		}
	}
}