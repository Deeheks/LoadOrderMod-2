﻿using Extensions;

using LoadOrderToolTwo.Domain.Steam;
using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.IO;
using LoadOrderToolTwo.Utilities.Managers;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoadOrderToolTwo.UserInterface.Lists;
internal class DlcListControl : SlickStackedListControl<SteamDlc>
{
	public IEnumerable<SteamDlc> FilteredItems => SafeGetItems().Select(x => x.Item);

	public DlcListControl()
	{
		HighlightOnHover = true;
		SeparateWithLines = true;

		Loading = SteamUtil.Dlcs.Count == 0;
	}

	protected override void UIChanged()
	{
		ItemHeight = CentralManager.SessionSettings.UserSettings.LargeItemOnHover ? 76 : 46;

		base.UIChanged();

		Padding = UI.Scale(new Padding(3, 2, 3, 2), UI.FontScale);
	}

	protected override IEnumerable<DrawableItem<SteamDlc>> OrderItems(IEnumerable<DrawableItem<SteamDlc>> items)
	{
		return items.OrderByDescending(x => x.Item.ReleaseDate);
	}

	protected override bool IsItemActionHovered(DrawableItem<SteamDlc> item, Point location)
	{
		var rects = GetActionRectangles(item.Bounds);

		if (rects.IncludedRect.Contains(location) && SteamUtil.IsDlcInstalledLocally(item.Item.Id))
		{
			setTip(Locale.ExcludeInclude, rects.IncludedRect);
			return true;
		}
		else if (rects.SteamRect.Contains(location))
		{
			setTip(Locale.ViewOnSteam, rects.SteamRect);
			return true;
		}
		else
		{
			setTip(System.Net.WebUtility.HtmlDecode(item.Item.Description), rects.TextRect);
		}

		void setTip(string text, Rectangle rectangle) => SlickTip.SetTo(this, text, offset: new Point(rectangle.X, item.Bounds.Y));

		return false;
	}

	protected override void OnItemMouseClick(DrawableItem<SteamDlc> item, MouseEventArgs e)
	{
		base.OnItemMouseClick(item, e);

		var rects = GetActionRectangles(item.Bounds);

		if (rects.IncludedRect.Contains(e.Location) && SteamUtil.IsDlcInstalledLocally(item.Item.Id))
		{
			item.Item.IsIncluded = !item.Item.IsIncluded;
		}

		if (rects.SteamRect.Contains(e.Location))
		{
			PlatformUtil.OpenUrl($"https://store.steampowered.com/app/{item.Item.Id}");
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (Loading)
		{
			base.OnPaint(e);
		}
		else if (!Items.Any())
		{
			e.Graphics.DrawString(Locale.NoDlcsNoInternet, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
		}
		else if (!SafeGetItems().Any())
		{
			e.Graphics.DrawString(Locale.NoDlcsOpenGame, UI.Font(9.75F, FontStyle.Italic), new SolidBrush(ForeColor), ClientRectangle, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
		}
		else
		{
			base.OnPaint(e);
		}
	}

	protected override void OnPaintItem(ItemPaintEventArgs<SteamDlc> e)
	{
		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
		var rects = GetActionRectangles(e.ClipRectangle);
		var isPressed = e.HoverState.HasFlag(HoverState.Pressed);

		e.HoverState &= ~HoverState.Pressed;

		base.OnPaintItem(e);

		var owned = SteamUtil.IsDlcInstalledLocally(e.Item.Id);
		var isIncluded = owned && e.Item.IsIncluded;

		if (owned)
		{
			if (isIncluded)
			{
				e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(rects.IncludedRect.Contains(CursorLocation) ? 150 : 255, FormDesign.Design.GreenColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
			}
			else if (rects.IncludedRect.Contains(CursorLocation))
			{
				e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(20, ForeColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
			}
		}

		var dIcon = new DynamicIcon(!owned ? "I_Slash" : isIncluded ? "I_Ok" : "I_Enabled");
		using (var icon = (large ? dIcon.Large : dIcon.Get(rects.IncludedRect.Height / 2)))
			e.Graphics.DrawImage(icon.Color(owned && rects.IncludedRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : isIncluded ? FormDesign.Design.ActiveForeColor : ForeColor), rects.IncludedRect.CenterR(icon.Size));

		var iconRectangle = rects.IconRect;
		var textRect = rects.TextRect;

		var iconImg = e.Item.Thumbnail;

		if (iconImg is null)
		{
			using var authorIcon = Properties.Resources.I_DlcIcon.Color(FormDesign.Design.IconColor);

			e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

			e.Graphics.FillRoundedRectangle(new SolidBrush(FormDesign.Design.IconColor), iconRectangle, (int)(4 * UI.FontScale));
			e.Graphics.FillRectangle(new SolidBrush(BackColor), iconRectangle.CenterR(iconRectangle.Height - 4, iconRectangle.Height - 4));
			e.Graphics.DrawImage(authorIcon, iconRectangle.CenterR(iconRectangle.Height - 2, iconRectangle.Height - 2));
		}
		else
		{
			e.Graphics.DrawRoundedImage(iconImg, iconRectangle, (int)(4 * UI.FontScale), FormDesign.Design.AccentBackColor);
		}

		e.Graphics.DrawString(e.Item.Name, UI.Font(large ? 11.25F : 9F, FontStyle.Bold), new SolidBrush(e.HoverState.HasFlag(HoverState.Pressed) ? FormDesign.Design.ActiveForeColor : ForeColor), textRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter });
		e.Graphics.DrawString(System.Net.WebUtility.HtmlDecode(e.Item.Description), UI.Font(large ? 9.75F : 7F), new SolidBrush(Color.FromArgb(150, e.HoverState.HasFlag(HoverState.Pressed) ? FormDesign.Design.ActiveForeColor : ForeColor)), textRect.Pad(0, (int)e.Graphics.Measure(e.Item.Name, UI.Font(large ? 11.25F : 9F, FontStyle.Bold)).Height, 0, 0), new StringFormat { Trimming = StringTrimming.EllipsisCharacter, LineAlignment = StringAlignment.Far });

		if (e.Item.ReleaseDate != DateTime.MinValue)
		{
			DrawLabel(e, CentralManager.SessionSettings.UserSettings.ShowDatesRelatively ? e.Item.ReleaseDate.ToLocalTime().ToRelatedString(true, false) : e.Item.ReleaseDate.ToString("D"), IconManager.GetSmallIcon("I_UpdateTime"), FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.BackColor, 50), rects.TextRect, ContentAlignment.TopRight);
		}

		using (var steamIcon = IconManager.GetIcon("I_Steam", rects.SteamRect.Height / 2))
			SlickButton.DrawButton(e, rects.SteamRect, string.Empty, Font, steamIcon, null, rects.SteamRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal);

		if (!isIncluded)
		{
			var filledRect = e.ClipRectangle.Pad(0, -Padding.Top, 0, -Padding.Bottom);
			e.Graphics.SetClip(filledRect);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(e.HoverState.HasFlag(HoverState.Hovered) ? 30 : 85, BackColor)), filledRect);
		}
	}

	private Rectangle DrawLabel(ItemPaintEventArgs<SteamDlc> e, string? text, Bitmap? icon, Color color, Rectangle rectangle, ContentAlignment alignment)
	{
		if (text == null)
		{
			return Rectangle.Empty;
		}

		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
		var size = e.Graphics.Measure(text, UI.Font(large ? 9F : 7.5F)).ToSize();

		if (icon is not null)
		{
			size.Width += icon.Width + Padding.Left;
		}

		size.Width += Padding.Left;

		rectangle = rectangle.Pad(Padding).Align(size, alignment);

		using var backBrush = rectangle.Gradient(color);
		using var foreBrush = new SolidBrush(color.GetTextColor());

		e.Graphics.FillRoundedRectangle(backBrush, rectangle, (int)(3 * UI.FontScale));
		e.Graphics.DrawString(text, UI.Font(large ? 9F : 7.5F), foreBrush, icon is null ? rectangle : rectangle.Pad(icon.Width + (Padding.Left * 2) - 2, 0, 0, 0), new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

		if (icon is not null)
		{
			e.Graphics.DrawImage(icon.Color(color.GetTextColor()), rectangle.Pad(Padding.Left, 0, 0, 0).Align(icon.Size, ContentAlignment.MiddleLeft));
		}

		return rectangle;
	}

	private Rectangles GetActionRectangles(Rectangle rectangle)
	{
		var includeItemHeight = CentralManager.SessionSettings.UserSettings.LargeItemOnHover ? (ItemHeight / 2) : ItemHeight;
		var iconSize = rectangle.Height - Padding.Vertical;
		var rects = new Rectangles
		{
			IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(includeItemHeight - 2, rectangle.Height - 2), ContentAlignment.MiddleLeft),
			SteamRect = rectangle.Pad(0, 0, Padding.Right, 0).Align(new Size(includeItemHeight, ItemHeight), ContentAlignment.TopRight)
		};

		rects.IconRect = rectangle.Pad(rects.IncludedRect.Right + (2 * Padding.Left)).Align(new Size(iconSize * 460 / 215, iconSize), ContentAlignment.MiddleLeft);

		rects.CenterRect = new Rectangle(rects.IconRect.X, rectangle.Y, rects.SteamRect.X - rects.IconRect.X, rectangle.Height);

		rects.TextRect = rectangle.Pad(rects.IconRect.X + rects.IconRect.Width + Padding.Left, 0, rectangle.Width - rects.CenterRect.Right, 0);

		return rects;
	}

	struct Rectangles
	{
		internal Rectangle IncludedRect;
		internal Rectangle IconRect;
		internal Rectangle TextRect;
		internal Rectangle SteamRect;
		internal Rectangle CenterRect;

		internal bool Contain(Point location)
		{
			return
				IncludedRect.Contains(location) ||
				SteamRect.Contains(location);
		}
	}
}
