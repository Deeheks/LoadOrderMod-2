﻿using Extensions;

using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.Managers;

using SlickControls;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoadOrderToolTwo.UserInterface.Panels;
public partial class PC_DLCs : PanelContent
{
	public PC_DLCs()
	{
		InitializeComponent();

		LC_DLCs.CanDrawItem += LC_DLCs_CanDrawItem;

		LC_DLCs.SetItems(SteamUtil.Dlcs);

		RefreshCounts();

		SteamUtil.DLCsLoaded += SteamUtil_DLCsLoaded;
	}

	private void SteamUtil_DLCsLoaded()
	{
		if (LC_DLCs.ItemCount != SteamUtil.Dlcs.Count)
		{
			LC_DLCs.SetItems(SteamUtil.Dlcs);
		}

		LC_DLCs.Loading = false;

		this.TryInvoke(RefreshCounts);
	}

	private void RefreshCounts()
	{
		var total = SteamUtil.Dlcs.Count(x => SteamUtil.IsDlcInstalledLocally(x.Id));

		L_Counts.Text = string.Format(Locale.DlcCount, total);
	}

	private void LC_DLCs_CanDrawItem(object sender, CanDrawItemEventArgs<Domain.Steam.SteamDlc> e)
	{
		if (T_YourDlcs.Selected && !SteamUtil.IsDlcInstalledLocally(e.Item.Id))
		{
			e.DoNotDraw = true;
		}

		if (!string.IsNullOrWhiteSpace(TB_Search.Text) && !TB_Search.Text.SearchCheck(e.Item.Name))
		{
			e.DoNotDraw = true;
		}
	}

	protected override void LocaleChanged()
	{
		Text = $"{Locale.DLCs} - {ProfileManager.CurrentProfile.Name}";
		L_Duplicates.Text = Locale.DlcUpdateNotice;
	}

	private void TB_Search_TextChanged(object sender, EventArgs e)
	{
		TB_Search.ImageName = string.IsNullOrWhiteSpace(TB_Search.Text) ? "I_Search" : "I_ClearSearch";
		LC_DLCs.FilterChanged();
		RefreshCounts();
	}

	protected override void DesignChanged(FormDesign design)
	{
		base.DesignChanged(design);

		tableLayoutPanel3.BackColor = design.AccentBackColor;
		L_Counts.ForeColor = design.InfoColor;
		L_Duplicates.ForeColor = design.YellowColor.MergeColor(design.ForeColor, 90);
	}

	protected override void UIChanged()
	{
		base.UIChanged();

		B_ExInclude.Size = UI.Scale(new Size(375, 26), UI.UIScale);
		TB_Search.Margin = L_Duplicates.Margin = L_Counts.Margin = B_ExInclude.Margin = UI.Scale(new Padding(5), UI.FontScale);
		L_Duplicates.Font = L_Counts.Font = UI.Font(7.5F, FontStyle.Bold);
		TB_Search.Width = (int)(250 * UI.FontScale);
	}

	public override bool KeyPressed(ref Message msg, Keys keyData)
	{
		if (keyData is (Keys.Control | Keys.F))
		{
			TB_Search.Focus();
			TB_Search.SelectAll();

			return true;
		}

		return false;
	}

	private void B_ExInclude_RightClicked(object sender, EventArgs e)
	{
		foreach (var item in LC_DLCs.FilteredItems)
		{
			item.IsIncluded = false;
		}

		LC_DLCs.Invalidate();
	}

	private void B_ExInclude_LeftClicked(object sender, EventArgs e)
	{
		foreach (var item in LC_DLCs.FilteredItems)
		{
			item.IsIncluded = true;
		}

		LC_DLCs.Invalidate();
	}

	private void TB_Search_IconClicked(object sender, EventArgs e)
	{
		TB_Search.Text = string.Empty;
	}

	private void T_YourDlcs_TabSelected(object sender, EventArgs e)
	{
		LC_DLCs.FilterChanged();
		RefreshCounts();
	}
}
