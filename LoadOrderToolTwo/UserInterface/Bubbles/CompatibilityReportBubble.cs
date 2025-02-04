﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.Managers;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ReportSeverity = CompatibilityReport.CatalogData.Enums.ReportSeverity;

namespace LoadOrderToolTwo.UserInterface.StatusBubbles;
internal class CompatibilityReportBubble : StatusBubbleBase
{
	public CompatibilityReportBubble()
	{ }

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		if (!Live)
		{
			return;
		}

		ImageName = "I_CompatibilityReport";
		Text = Locale.CompatibilityReport;

		if (!CentralManager.IsContentLoaded)
		{
			Loading = true;

			CentralManager.ContentLoaded += CentralManager_InfoUpdated;
		}

		CentralManager.AssetInformationUpdated += Invalidate;
		ProfileManager.ProfileChanged += ProfileManager_ProfileChanged;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);

		CentralManager.AssetInformationUpdated -= Invalidate;
		CentralManager.ContentLoaded -= CentralManager_InfoUpdated;
		ProfileManager.ProfileChanged -= ProfileManager_ProfileChanged;
	}

	private void ProfileManager_ProfileChanged(Profile obj)
	{
		Invalidate();
	}

	private void CentralManager_InfoUpdated()
	{
		if (Loading)
		{
			Loading = false;
		}
		else
		{
			Invalidate();
		}
	}

	protected override void CustomDraw(PaintEventArgs e, ref int targetHeight)
	{
		if (!CentralManager.IsContentLoaded)
		{
			DrawText(e, ref targetHeight, Locale.Loading, FormDesign.Design.InfoColor);
			return;
		}

		if (!CompatibilityManager.CatalogAvailable)
		{
			DrawText(e, ref targetHeight, Locale.CrNotAvailable, FormDesign.Design.RedColor);
			return;
		}

		var groups = CentralManager.Mods.Where(x => x.IsIncluded).GroupBy(x => x.Package.CompatibilityReport?.Severity);

		DrawValue(e, ref targetHeight, groups.Sum(x => !(x.Key > ReportSeverity.Remarks) ? x.Count() : 0).ToString(), Locale.ModsNoIssues);

		foreach (var group in groups.OrderBy(x => x.Key))
		{
			if (!(group.Key > ReportSeverity.Remarks))
			{
				continue;
			}

			DrawValue(e, ref targetHeight, group.Count().ToString(), group.Key switch
			{
				ReportSeverity.MinorIssues => Locale.ModsWithMinorIssues,
				ReportSeverity.MajorIssues => Locale.ModsWithMajorIssues,
				ReportSeverity.Unsubscribe => Locale.ModsShouldUnsub,
				_ => ""
			}, group.Key switch
			{
				ReportSeverity.MinorIssues => FormDesign.Design.YellowColor,
				ReportSeverity.MajorIssues => FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.RedColor),
				ReportSeverity.Unsubscribe => FormDesign.Design.RedColor,
				_ => Color.Empty
			});
		}
	}
}
