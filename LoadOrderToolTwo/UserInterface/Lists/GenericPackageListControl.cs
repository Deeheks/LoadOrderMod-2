﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Domain.Enums;
using LoadOrderToolTwo.Domain.Interfaces;
using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.IO;
using LoadOrderToolTwo.Utilities.Managers;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

using static CompatibilityReport.CatalogData.Enums;

namespace LoadOrderToolTwo.UserInterface.Lists;
internal class GenericPackageListControl : ItemListControl<IPackage> { }
//{
//	public IEnumerable<IGenericPackage> FilteredItems => SafeGetItems().Select(x => x.Item);
//	public int FilteredCount => SafeGetItems().Count;

//	public GenericPackageListControl()
//	{
//		HighlightOnHover = true;
//		SeparateWithLines = true;
//	}

//	protected override void UIChanged()
//	{
//		ItemHeight = CentralManager.SessionSettings.UserSettings.LargeItemOnHover?64:36;

//		base.UIChanged();

//		Padding = UI.Scale(new Padding(3, 2, 3, 2), UI.FontScale);
//	}

//	protected override IEnumerable<DrawableItem<IGenericPackage>> OrderItems(IEnumerable<DrawableItem<IGenericPackage>> items)
//	{
//		return items.OrderBy(x => x.Item.Name);
//	}

//	protected override bool IsItemActionHovered(DrawableItem<IGenericPackage> item, Point location)
//	{
//		var rects = GetActionRectangles(item.Bounds, item.Item);
//		var state = ContentUtil.GetGenericPackageState(item.Item, out var package);

//		if (rects.IncludedRect.Contains(location))
//		{
//			switch (state)
//			{
//				case GenericPackageState.Unsubscribed:
//					setTip(Locale.SubscribeToItem, rects.IncludedRect);
//					break;
//				case GenericPackageState.Disabled:
//					setTip(Locale.EnableDisable, rects.IncludedRect);
//					break;
//				case GenericPackageState.Enabled:
//				case GenericPackageState.Excluded:
//					setTip(Locale.ExcludeInclude, rects.IncludedRect);
//					break;
//			}
//		}
//		else if (rects.RemoveRect.Contains(location) && state > GenericPackageState.Unsubscribed)
//		{
//			setTip(Locale.UnsubscribePackage, rects.RemoveRect);
//		}
//		else if (item.Item.SteamId != 0)
//		{
//			if (rects.SteamRect.Contains(location))
//			{
//				setTip(Locale.ViewOnSteam, rects.SteamRect);
//			}

//			else if (rects.SteamIdRect.Contains(location))
//			{
//				setTip(Locale.CopySteamId, rects.SteamIdRect);
//			}

//			else if (rects.AuthorRect.Contains(location))
//			{
//				setTip(Locale.OpenAuthorPage, rects.AuthorRect);
//			}
//			else
//			{
//				setTip(item.Item.Name, rects.TextRect);
//			}
//		}
//		else
//		{
//			setTip(item.Item.Name, rects.TextRect);
//		}

//		void setTip(string? text, Rectangle rectangle) => SlickTip.SetTo(this, text, offset: new Point(rectangle.X, item.Bounds.Y));

//		return rects.Contain(location);
//	}

//	protected override async void OnItemMouseClick(DrawableItem<IGenericPackage> item, MouseEventArgs e)
//	{
//		base.OnItemMouseClick(item, e);

//		var rects = GetActionRectangles(item.Bounds, item.Item);
//		var state = ContentUtil.GetGenericPackageState(item.Item, out var package);

//		if (rects.IncludedRect.Contains(e.Location))
//		{
//			switch (state)
//			{
//				case GenericPackageState.Unsubscribed:
//					await CitiesManager.Subscribe(new[] { item.Item.SteamId });
//					break;
//				case GenericPackageState.Disabled:
//					package!.Mod!.IsEnabled = true;
//					break;
//				case GenericPackageState.Enabled:
//					package!.IsIncluded = false;
//					break;
//				case GenericPackageState.Excluded:
//					package!.IsIncluded = true;
//					break;
//			}
//		}

//		if (rects.RemoveRect.Contains(e.Location) && state > GenericPackageState.Unsubscribed)
//		{
//			await CitiesManager.Subscribe(new[] { item.Item.SteamId }, true);
//		}

//		if (item.Item.SteamId != 0)
//		{
//			if (rects.SteamRect.Contains(e.Location))
//			{
//				PlatformUtil.OpenUrl($"https://steamcommunity.com/workshop/filedetails/?id={item.Item.SteamId}");
//			}

//			else if (rects.SteamIdRect.Contains(e.Location))
//			{
//				Clipboard.SetText(item.Item.SteamId.ToString());
//				return;
//			}

//			else if (rects.AuthorRect.Contains(e.Location))
//			{
//				PlatformUtil.OpenUrl($"{item.Item.Author?.ProfileUrl}myworkshopfiles");
//				return;
//			}
//		}
//	}

//	protected override void OnPaint(PaintEventArgs e)
//	{
//		if (Loading)
//		{
//			base.OnPaint(e);
//		}
//		else if (!Items.Any())
//		{
//			e.Graphics.DrawString(Locale.NoItemsToBeDisplayed, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
//		}
//		else if (!SafeGetItems().Any())
//		{
//			e.Graphics.DrawString(Locale.NoPackagesMatchFilters, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
//		}
//		else
//		{
//			base.OnPaint(e);
//		}
//	}

//	protected override void OnPaintItem(ItemPaintEventArgs<IGenericPackage> e)
//	{
//		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
//		var rects = GetActionRectangles(e.ClipRectangle, e.Item);
//		var isPressed = e.HoverState.HasFlag(HoverState.Pressed);

//		if (isPressed && !rects.CenterRect.Contains(CursorLocation))
//		{
//			e.HoverState &= ~HoverState.Pressed;
//		}

//		base.OnPaintItem(e);

//		var includeHovered = rects.IncludedRect.Contains(CursorLocation);
//		var state = ContentUtil.GetGenericPackageState(e.Item);

//		switch (state)
//		{
//			case GenericPackageState.Local:
//				using (var img = IconManager.GetLargeIcon("I_PC").Color(ForeColor))
//				{
//					e.Graphics.DrawImage(img, rects.IncludedRect.CenterR(img.Size));
//				}

//				break;
//			case GenericPackageState.Unsubscribed:
//				if (includeHovered)
//				{
//					e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(20, FormDesign.Design.ForeColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
//				}

//				using (var img = IconManager.GetLargeIcon("I_Add"))
//				{
//					e.Graphics.DrawImage(img.Color(includeHovered ? FormDesign.Design.ActiveColor : ForeColor), rects.IncludedRect.CenterR(img.Size));
//				}

//				break;
//			case GenericPackageState.Disabled:
//				e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(includeHovered ? 150 : 255, FormDesign.Design.RedColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
//				using (var img = IconManager.GetLargeIcon("I_Cancel"))
//				{
//					e.Graphics.DrawImage(img.Color(includeHovered ? FormDesign.Design.ActiveColor : FormDesign.Design.ActiveForeColor), rects.IncludedRect.CenterR(img.Size));
//				}

//				break;
//			case GenericPackageState.Enabled:
//				e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(includeHovered ? 150 : 255, FormDesign.Design.GreenColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
//				using (var img = IconManager.GetLargeIcon("I_Ok"))
//				{
//					e.Graphics.DrawImage(img.Color(includeHovered ? FormDesign.Design.ActiveColor : FormDesign.Design.ActiveForeColor), rects.IncludedRect.CenterR(img.Size));
//				}

//				break;
//			case GenericPackageState.Excluded:
//				if (includeHovered)
//				{
//					e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(20, ForeColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
//				}

//				using (var img = IconManager.GetLargeIcon("I_Enabled"))
//				{
//					e.Graphics.DrawImage(img.Color(includeHovered ? FormDesign.Design.ActiveColor : ForeColor), rects.IncludedRect.CenterR(img.Size));
//				}

//				break;
//			default:
//				break;
//		}

//		var iconRectangle = rects.IconRect;
//		var textRect = rects.TextRect;

//		var iconImg = e.Item.IconImage;

//		if (iconImg is null)
//		{
//			using var authorIcon = Properties.Resources.I_DlcIcon.Color(FormDesign.Design.IconColor);

//			e.Graphics.DrawRoundedImage(authorIcon, iconRectangle, (int)(4 * UI.FontScale), FormDesign.Design.AccentBackColor);
//		}
//		else
//		{
//			e.Graphics.DrawRoundedImage(iconImg, iconRectangle, (int)(4 * UI.FontScale), FormDesign.Design.AccentBackColor);
//		}

//		e.Graphics.DrawString(e.Item.Name?.RemoveVersionText(out _), UI.Font(large ? 11.25F : 9F, FontStyle.Bold), new SolidBrush(e.HoverState.HasFlag(HoverState.Pressed) ? FormDesign.Design.ActiveForeColor : ForeColor), textRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });

//		var labelX = textRect.X;
//		var versionText = e.Item.IsMod ? e.Item.Name?.GetVersionText() : string.Empty;

//		if (!string.IsNullOrEmpty(versionText))
//		{
//			labelX = DrawLabel(e, versionText, null, FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.BackColor, 40), new Rectangle(labelX, e.ClipRectangle.Y, (int)(100 * UI.FontScale), e.ClipRectangle.Height), ContentAlignment.BottomLeft).Right;
//		}

//		if (e.Item.SteamId != 0)
//		{
//			DrawLabel(e, e.Item.Author?.Name, IconManager.GetSmallIcon("I_Developer"), rects.AuthorRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.ActiveColor, 75).MergeColor(FormDesign.Design.BackColor, 40), rects.AuthorRect, ContentAlignment.TopLeft);
//			DrawLabel(e, e.Item.SteamId.ToString(), IconManager.GetSmallIcon("I_Steam"), rects.SteamIdRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.ActiveColor, 75).MergeColor(FormDesign.Design.BackColor, 40), rects.SteamIdRect, ContentAlignment.BottomLeft);

//			var report = CompatibilityManager.GetCompatibilityReport(e.Item.SteamId);
//			if (report is not null && report.Severity != ReportSeverity.NothingToReport)
//			{
//			using var crIcon = IconManager.GetSmallIcon("I_CompatibilityReport");
//				labelX = DrawLabel(e, LocaleHelper.GetGlobalText(report.Severity == ReportSeverity.Unsubscribe ? Locale.ShouldNotBeSubscribed : $"CR_{report.Severity}"), crIcon, (report.Severity switch
//				{
//					ReportSeverity.MinorIssues => FormDesign.Design.YellowColor,
//					ReportSeverity.MajorIssues => FormDesign.Design.YellowColor.MergeColor(FormDesign.Design.RedColor),
//					ReportSeverity.Unsubscribe => FormDesign.Design.RedColor,
//					ReportSeverity.Remarks => FormDesign.Design.ButtonColor,
//					_ => FormDesign.Design.GreenColor.MergeColor(FormDesign.Design.AccentColor, 20)
//				}).MergeColor(FormDesign.Design.BackColor, 65), new Rectangle(labelX + Padding.Left, e.ClipRectangle.Y, (int)(100 * UI.FontScale), e.ClipRectangle.Height), ContentAlignment.BottomLeft).Right;
//			}

//			using var steamIcon = IconManager.GetIcon("I_Steam");
//			SlickButton.DrawButton(e, rects.SteamRect, string.Empty, Font, steamIcon, null, rects.SteamRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal);
//		}

//		if (state > GenericPackageState.Unsubscribed)
//		{
//			using var steamIcon = IconManager.GetIcon("I_RemoveSteam");
//			SlickButton.DrawButton(e, rects.RemoveRect, string.Empty, Font, steamIcon, null, rects.RemoveRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal, ColorStyle.Red);
//		}

//		if (e.Item.Tags is not null)
//		{
//			using var tagIcon = IconManager.GetSmallIcon("I_Tag");
//			foreach (var item in e.Item.Tags.OrderBy(x => x))
//			{
//				labelX = DrawLabel(e, item.Value, tagIcon, FormDesign.Design.ButtonColor, new Rectangle(labelX + Padding.Left, e.ClipRectangle.Y, (int)(100 * UI.FontScale), e.ClipRectangle.Height), ContentAlignment.BottomLeft).Right;
//			}
//		}

//		if (state is GenericPackageState.Local or GenericPackageState.Excluded)
//		{
//			var filledRect = e.ClipRectangle.Pad(0, -Padding.Top, 0, -Padding.Bottom);
//			e.Graphics.SetClip(filledRect);
//			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(e.HoverState.HasFlag(HoverState.Hovered) ? 30 : 85, BackColor)), filledRect);
//		}
//	}

//	private Rectangle DrawLabel(ItemPaintEventArgs<IGenericPackage> e, string? text, Bitmap? icon, Color color, Rectangle rectangle, ContentAlignment alignment)
//	{
//		if (text == null)
//		{
//			return Rectangle.Empty;
//		}

//		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
//		var size = e.Graphics.Measure(text, UI.Font(large ? 9F : 7.5F)).ToSize();

//		if (icon is not null)
//		{
//			size.Width += icon.Width + Padding.Left;
//		}

//		size.Width += Padding.Left;

//		rectangle = rectangle.Pad(Padding).Align(size, alignment);

//		using var backBrush = rectangle.Gradient(color);
//		using var foreBrush = new SolidBrush(color.GetTextColor());

//		e.Graphics.FillRoundedRectangle(backBrush, rectangle, (int)(3 * UI.FontScale));
//		e.Graphics.DrawString(text, UI.Font(large ? 9F : 7.5F), foreBrush, icon is null ? rectangle : rectangle.Pad(icon.Width + (Padding.Left * 2) - 2, 0, 0, 0), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

//		if (icon is not null)
//		{
//			e.Graphics.DrawImage(icon.Color(color.GetTextColor()), rectangle.Pad(Padding.Left, 0, 0, 0).Align(icon.Size, ContentAlignment.MiddleLeft));
//		}

//		return rectangle;
//	}

//	private Rectangles GetActionRectangles(Rectangle rectangle, IGenericPackage item)
//	{
//		var iconSize = rectangle.Height - Padding.Vertical;
//		var rects = new Rectangles
//		{
//			IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(ItemHeight + 1, rectangle.Height), ContentAlignment.MiddleLeft),
//			SteamRect = rectangle.Pad(0, 0, Padding.Right, 0).Align(new Size(ItemHeight, ItemHeight), ContentAlignment.TopRight)
//		};

//		rects.RemoveRect = new Rectangle(rects.SteamRect.X - rects.SteamRect.Width - Padding.Left, rects.SteamRect.Y, rects.SteamRect.Width, rects.SteamRect.Height);

//		rects.IconRect = rectangle.Pad(rects.IncludedRect.Right + (2 * Padding.Left)).Align(new Size(iconSize, iconSize), ContentAlignment.MiddleLeft);

//		if (item.SteamId != 0)
//		{
//			rects.AuthorRect = new Rectangle(rects.RemoveRect.X - (int)(100 * UI.FontScale), rectangle.Y + (rectangle.Height / 2), (int)(100 * UI.FontScale), rectangle.Height / 2);
//			rects.SteamIdRect = new Rectangle(rects.RemoveRect.X - (int)(100 * UI.FontScale), rectangle.Y, (int)(100 * UI.FontScale), rectangle.Height / 2);
//		}

//		rects.CenterRect = new Rectangle(rects.IconRect.X, rectangle.Y, rects.RemoveRect.X - (int)(item.SteamId != 0 ? 100 * UI.FontScale : 0) - rects.IconRect.X, rectangle.Height);

//		rects.TextRect = rectangle.Pad(rects.IconRect.X + rects.IconRect.Width + Padding.Left, 0, rectangle.Width - rects.CenterRect.Right, rectangle.Height / 2);

//		return rects;
//	}

//	struct Rectangles
//	{
//		internal Rectangle IncludedRect;
//		internal Rectangle IconRect;
//		internal Rectangle TextRect;
//		internal Rectangle SteamRect;
//		internal Rectangle CenterRect;
//		internal Rectangle SteamIdRect;
//		internal Rectangle AuthorRect;
//		internal Rectangle RemoveRect;

//		internal bool Contain(Point location)
//		{
//			return
//				IncludedRect.Contains(location) ||
//				SteamIdRect.Contains(location) ||
//				AuthorRect.Contains(location) ||
//				RemoveRect.Contains(location) ||
//				CenterRect.Contains(location) ||
//				SteamRect.Contains(location);
//		}
//	}
//}
