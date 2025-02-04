﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Domain.Utilities;
using LoadOrderToolTwo.Utilities.IO;
using LoadOrderToolTwo.Utilities.Managers;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LoadOrderToolTwo.Utilities;
public static class LogUtil
{
	static LogUtil()
	{
		try
		{
			foreach (var item in Directory.GetFiles(LocationManager.Combine(LocationManager.LotAppDataPath, "Support Logs")))
			{
				if (DateTime.Now - File.GetLastWriteTime(item) > TimeSpan.FromDays(15))
				{
					ExtensionClass.DeleteFile(item);
				}
			}
		}
		catch { }
	}

	public static string GameLogFile => LocationManager.Platform switch
	{
		Platform.MacOSX => $"/Users/{Environment.UserName}/Library/Logs/Unity/Player.log",
		Platform.Linux => $"/.config/unity3d/Colossal Order/Cities: Skylines/Player.log",
		_ => LocationManager.Combine(LocationManager.GamePath, "Cities_Data", "output_log.txt")
	};

	public static string GameDataPath => LocationManager.Platform switch
	{
		Platform.MacOSX => LocationManager.Combine(LocationManager.GamePath, "Cities.app", "Contents"),
		_ => LocationManager.Combine(LocationManager.GamePath, "Cities_Data")
	};

	public static string CreateZipFileAndSetToClipboard(string? folder = null)
	{
		var file = LocationManager.Combine(folder ?? Path.GetTempPath(), $"LogReport_{DateTime.Now:yy-MM-dd_hh-mm-tt}.zip");

		using (var fileStream = File.Create(file))
		using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create, true))
		{
			AddMainFilesToZip(zipArchive);

			foreach (var filePath in GetFilesForZip())
			{
				if (LocationManager.FileExists(filePath))
				{
					zipArchive.CreateEntryFromFile(filePath, $"Other Files\\{Path.GetFileName(filePath)}");
				}
			}
		}

		PlatformUtil.SetFileInClipboard(file);

		return file;
	}

	private static IEnumerable<string> GetFilesForZip()
	{
		yield return GetLastCrashLog();
		yield return GetLastLSMReport();
		yield return LocationManager.Combine(GameDataPath, "CompatibilityReport.html");

		foreach (var item in new DirectoryInfo(LocationManager.Combine(GameDataPath, "Logs")).GetFiles("*.log"))
		{
			if (DateTime.Now - item.LastWriteTime < TimeSpan.FromDays(1))
			{
				yield return item.FullName;
			}
		}

		foreach (var item in new DirectoryInfo(GameDataPath).GetFiles("*.log"))
		{
			if (DateTime.Now - item.LastWriteTime < TimeSpan.FromDays(1) && Path.GetFileName(GameLogFile) != item.Name)
			{
				yield return item.FullName;
			}
		}
	}

	private static void AddMainFilesToZip(ZipArchive zipArchive)
	{
		if (!LocationManager.FileExists(GameLogFile))
		{
			return;
		}

		var tempLogFile = Path.GetTempFileName();
		var tempLotLogFile = Path.GetTempFileName();

		ExtensionClass.CopyFile(GameLogFile, tempLogFile, true);
		ExtensionClass.CopyFile(Log.LogFilePath, tempLotLogFile, true);

		zipArchive.CreateEntryFromFile(tempLogFile, "log.txt");

		zipArchive.CreateEntryFromFile(tempLotLogFile, "Tool\\LOT2.log");

		var logTrace = SimplifyLog(tempLogFile, out var simpleLogText);

		AddSimpleLog(zipArchive, simpleLogText);

		AddErrors(zipArchive, logTrace);

		AddCompatibilityReport(zipArchive);

		AddProfile(zipArchive);
	}

	private static void AddCompatibilityReport(ZipArchive zipArchive)
	{
		var profileEntry = zipArchive.CreateEntry("Tool\\CompatibilityReport.json");
		using var writer = new StreamWriter(profileEntry.Open());
		var packages = CentralManager.Packages.Where(x => x.Workshop && x.IsIncluded).ToList();
		var reports = packages
			.Select(x => CompatibilityManager.GetCompatibilityReport(x))
			.Where(x => x != null)
			.GroupBy(x => x.Severity)
			.OrderByDescending(x => x.Key)
			.ToDictionary(x => x.Key.ToString(), x => x.ToList());

		writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(reports, Newtonsoft.Json.Formatting.Indented));
	}

	private static void AddProfile(ZipArchive zipArchive)
	{
		var profileEntry = zipArchive.CreateEntry("Tool\\LogProfile.json");
		using var writer = new StreamWriter(profileEntry.Open());
		var profile = new Profile("LogProfile");
		ProfileManager.GatherInformation(profile);
		profile.Temporary = true;

		writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(profile, Newtonsoft.Json.Formatting.Indented));
	}

	private static void AddErrors(ZipArchive zipArchive, List<LogTrace> logTrace)
	{
		if (logTrace.Count == 0)
		{
			return;
		}

		var errorsEntry = zipArchive.CreateEntry("log_errors.txt");
		using var writer = new StreamWriter(errorsEntry.Open());
		var errors = logTrace.Select(e => e.ToString()).ListStrings("\r\n*********************************************\r\n");

		writer.Write(errors);
	}

	private static void AddSimpleLog(ZipArchive zipArchive, string simpleLogText)
	{
		var simpleLogEntry = zipArchive.CreateEntry("log_simple.txt");
		using var writer = new StreamWriter(simpleLogEntry.Open());
		writer.Write(simpleLogText);
	}

	private static string GetLastCrashLog()
	{
		if (LocationManager.Platform is not Platform.Windows)
		{
			return string.Empty;
		}

		try
		{
			var mainGameDir = new DirectoryInfo(GameDataPath).Parent;
			var directories = mainGameDir.GetDirectories($"*-*-*");
			var latest = directories
				.Where(s => DateTime.Now - s.LastWriteTime < TimeSpan.FromDays(1))
				.OrderByDescending(s => s.CreationTime)
				.FirstOrDefault();

			if (latest != null)
			{
				return LocationManager.Combine(latest.FullName, "error.log");
			}
		}
		catch (Exception ex) { Log.Exception(ex, "Failed to load the previous crash dump log"); }

		return string.Empty;
	}

	private static string GetLastLSMReport()
	{
		try
		{
			var path = LsmUtil.GetReportFolder();

			var reports = Directory.GetFiles(path, "*Assets Report*.htm");

			return reports.OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}

		return string.Empty;
	}

	public static List<LogTrace> SimplifyLog(string log, out string simpleLog)
	{
		var lines = File.ReadAllLines(log).ToList();

		// decruft the log file
		for (var i = lines.Count - 1; i > 0; i--)
		{
			var current = lines[i];
			if (current.IndexOf("DebugBindings.gen.cpp Line: 51") != -1 ||
				current.StartsWith("Fallback handler") ||
				current.Contains("[PlatformService, Native - Internal]") ||
				current.Contains("m_SteamUGCRequestMap error") ||
				current.IndexOf("(this message is harmless)") != -1 ||
				current.IndexOf("PopsApi:") != -1 ||
				current.IndexOf("GfxDevice") != -1 ||
				current.StartsWith("Assembly ") ||
				current.StartsWith("No source files found:") ||
				current.StartsWith("d3d11: failed") ||
				current.StartsWith("(Filename:  Line: ") ||
				current.Contains("SteamHelper+DLC_BitMask") ||
				current.EndsWith(" [Packer - Internal]") ||
				current.EndsWith(" [Mods - Internal]"))
			{
				lines.RemoveAt(i);

				if (i < lines.Count && string.IsNullOrWhiteSpace(lines[i]))
				{
					lines.RemoveAt(i);
				}
			}
		}

		// clear excess blank lines

		var blank = false;

		for (var i = lines.Count - 1; i > 0; i--)
		{
			if (blank)
			{
				if (string.IsNullOrWhiteSpace(lines[i]))
				{
					lines.RemoveAt(i);
				}
				else
				{
					blank = false;
				}
			}
			else
			{
				blank = string.IsNullOrWhiteSpace(lines[i]);
			}
		}

		simpleLog = string.Join("\r\n", lines);

		// now split out errors

		LogTrace? currentTrace = null;
		var traces = new List<LogTrace>();

		for (var i = 0; i < lines.Count; i++)
		{
			var current = lines[i];

			if (!current.StartsWith("Crash!!!") && !current.TrimStart().StartsWith("at ") && !(current.TrimStart().StartsWith("--") && currentTrace is not null))
			{
				if (currentTrace is not null)
				{
					if (!currentTrace.Title.Contains("System.Environment.get_StackTrace()"))
					{
						traces.Add(currentTrace);
					}

					currentTrace = null;
				}

				if (current.Contains("[Warning]") || current.Contains("[Error]"))
				{
					traces.Add(new(lines, i + 1, false));
				}

				continue;
			}

			currentTrace ??= new(lines, i, current.StartsWith("Crash!!!"));

			currentTrace.AddTrace(current);
		}

		if (currentTrace is not null)
		{
			traces.Add(currentTrace);
		}

		return traces;
	}
}
