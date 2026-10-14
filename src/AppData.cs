using System;
using System.Collections.Generic;
using System.IO;

internal class AppData
{
	public const int MaxHistory = 10;

	public List<string> Workspaces = new List<string>();

	public string BgImagePath = "";

	public static readonly string DataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClaudeCode");

	public static readonly string HistoryFile = Path.Combine(DataDir, "workspace_history.txt");

	public static readonly string SettingsFile = Path.Combine(DataDir, "settings.txt");
}
