﻿using Extensions;

using LoadOrderToolTwo.Domain;
using LoadOrderToolTwo.Utilities;
using LoadOrderToolTwo.Utilities.Managers;

using SlickControls;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LoadOrderToolTwo.UserInterface.Lists;
internal class OtherProfilePackage : SlickStackedListControl<Profile>
{
	public IEnumerable<Profile> FilteredItems => SafeGetItems().Select(x => x.Item);

	public Package Package { get; }

	public OtherProfilePackage(Package package)
	{
		HighlightOnHover = true;
		SeparateWithLines = true;
		Package = package;
		SetItems(ProfileManager.Profiles.Skip(1));

		ProfileManager.ProfileUpdated += ProfileManager_ProfileUpdated;
		ProfileManager.ProfileChanged += ProfileManager_ProfileChanged;
	}

	private void ProfileManager_ProfileChanged(Profile obj)
	{
		Invalidate();
	}

	private void ProfileManager_ProfileUpdated()
	{
		Invalidate();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ProfileManager.ProfileChanged -= ProfileManager_ProfileChanged;
			ProfileManager.ProfileUpdated -= ProfileManager_ProfileUpdated;
		}

		base.Dispose(disposing);
	}

	protected override void UIChanged()
	{
		ItemHeight = CentralManager.SessionSettings.UserSettings.LargeItemOnHover ? 48 : 28;

		base.UIChanged();

		Padding = UI.Scale(new Padding(3, 1, 3, 1), UI.FontScale);
	}

	protected override IEnumerable<DrawableItem<Profile>> OrderItems(IEnumerable<DrawableItem<Profile>> items)
	{
		return items.OrderByDescending(x => x.Item.LastEditDate);
	}

	protected override bool IsItemActionHovered(DrawableItem<Profile> item, Point location)
	{
		var rects = GetActionRectangles(item.Bounds);

		if (rects.IncludedRect.Contains(location))
		{
			setTip(string.Format(Locale.IncludeExcludeOtherProfile, Package, item.Item), rects.IncludedRect);
			return true;
		}
		else if (rects.LoadRect.Contains(location))
		{
			setTip(Locale.LoadProfile, rects.LoadRect);
			return true;
		}

		void setTip(string text, Rectangle rectangle) => SlickTip.SetTo(this, text, offset: new Point(rectangle.X, item.Bounds.Y));

		return false;
	}

	protected override void OnItemMouseClick(DrawableItem<Profile> item, MouseEventArgs e)
	{
		base.OnItemMouseClick(item, e);

		var rects = GetActionRectangles(item.Bounds);

		if (rects.IncludedRect.Contains(e.Location))
		{
			var isIncluded = ProfileManager.IsPackageIncludedInProfile(Package, item.Item);
			ProfileManager.SetIncludedFor(Package, item.Item, !isIncluded);
		}

		if (rects.LoadRect.Contains(e.Location))
		{
			ProfileManager.SetProfile(item.Item);
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

	protected override void OnPaintItem(ItemPaintEventArgs<Profile> e)
	{
		var large = CentralManager.SessionSettings.UserSettings.LargeItemOnHover;
		var rects = GetActionRectangles(e.ClipRectangle);
		var isPressed = e.HoverState.HasFlag(HoverState.Pressed);

		e.HoverState &= ~HoverState.Pressed;

		base.OnPaintItem(e);

		var isIncluded = ProfileManager.IsPackageIncludedInProfile(Package, e.Item);

		if (isIncluded)
		{
			e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(rects.IncludedRect.Contains(CursorLocation) ? 150 : 255, FormDesign.Design.GreenColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
		}
		else if (rects.IncludedRect.Contains(CursorLocation))
		{
			e.Graphics.FillRoundedRectangle(rects.IncludedRect.Gradient(Color.FromArgb(20, ForeColor), 1.5F), rects.IncludedRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
		}

		var incl = new DynamicIcon(isIncluded ? "I_Ok" : "I_Enabled");
		using var icon = large ? incl.Large : incl.Get(rects.IncludedRect.Height / 2);

		e.Graphics.DrawImage(icon.Color(rects.IncludedRect.Contains(CursorLocation) ? FormDesign.Design.ActiveColor : isIncluded ? FormDesign.Design.ActiveForeColor : ForeColor), rects.IncludedRect.CenterR(icon.Size));

		var iconColor = FormDesign.Design.IconColor;

		if (e.Item.Color != null)
		{
			iconColor = e.Item.Color.Value.GetTextColor();

			e.Graphics.FillRoundedRectangle(rects.IconRect.Gradient(e.Item.Color.Value, 1.5F), rects.IconRect.Pad(0, Padding.Vertical, 0, Padding.Vertical), 4);
		}

		using var profileIcon = e.Item.GetIcon().Default;

		e.Graphics.DrawImage(profileIcon.Color(iconColor), rects.IconRect.CenterR(profileIcon.Size));

		e.Graphics.DrawString(e.Item.Name, UI.Font(large ? 11.25F : 9F, FontStyle.Bold), new SolidBrush(e.HoverState.HasFlag(HoverState.Pressed) ? FormDesign.Design.ActiveForeColor : ForeColor), rects.TextRect, new StringFormat { Trimming = StringTrimming.EllipsisCharacter, LineAlignment = StringAlignment.Center });

		var rect = DrawLabel(e, $"{e.Item.Assets.Count} {(e.Item.Assets.Count == 1 ? Locale.AssetIncluded : Locale.AssetIncludedPlural)}", IconManager.GetSmallIcon("I_Assets"), FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.BackColor, 50), rects.TextRect, ContentAlignment.MiddleRight);
		rect = DrawLabel(e, $"{e.Item.Mods.Count} {(e.Item.Mods.Count == 1 ? Locale.ModIncluded : Locale.ModIncludedPlural)}", IconManager.GetSmallIcon("I_Mods"), FormDesign.Design.AccentColor.MergeColor(FormDesign.Design.BackColor, 50), rects.TextRect.Pad(0, 0, rect.Width + Padding.Left, 0), ContentAlignment.MiddleRight);

		if (e.Item == ProfileManager.CurrentProfile)
		{
			DrawLabel(e, Locale.CurrentProfile, IconManager.GetSmallIcon("I_Ok"), FormDesign.Design.ActiveColor, rects.TextRect.Pad(0, 0, rects.TextRect.Right - rect.X + Padding.Left, 0), ContentAlignment.MiddleRight);
		}

		SlickButton.DrawButton(e, rects.LoadRect, string.Empty, Font, IconManager.GetIcon("I_Import"), null, rects.LoadRect.Contains(CursorLocation) ? e.HoverState | (isPressed ? HoverState.Pressed : 0) : HoverState.Normal);

		if (!isIncluded)
		{
			var filledRect = e.ClipRectangle.Pad(0, -Padding.Top, 0, -Padding.Bottom);
			e.Graphics.SetClip(filledRect);
			e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(e.HoverState.HasFlag(HoverState.Hovered) ? 30 : 85, BackColor)), filledRect);
		}
	}

	private Rectangle DrawLabel(ItemPaintEventArgs<Profile> e, string? text, Bitmap? icon, Color color, Rectangle rectangle, ContentAlignment alignment)
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
		var rects = new Rectangles
		{
			IncludedRect = rectangle.Pad(1 * Padding.Left, 0, 0, 0).Align(new Size(rectangle.Height - 2, rectangle.Height - 2), ContentAlignment.MiddleLeft),
			LoadRect = rectangle.Pad(0, 0, Padding.Right, 0).Align(new Size(ItemHeight, ItemHeight), ContentAlignment.TopRight)
		};

		rects.IconRect = rectangle.Pad(rects.IncludedRect.Right + (2 * Padding.Left)).Align(rects.IncludedRect.Size, ContentAlignment.MiddleLeft);

		rects.TextRect = new Rectangle(rects.IconRect.Right + Padding.Left, rectangle.Y, rects.LoadRect.X - rects.IconRect.Right - (2 * Padding.Left), rectangle.Height);

		return rects;
	}

	struct Rectangles
	{
		internal Rectangle IncludedRect;
		internal Rectangle IconRect;
		internal Rectangle LoadRect;
		internal Rectangle TextRect;

		internal bool Contain(Point location)
		{
			return
				IncludedRect.Contains(location) ||
				LoadRect.Contains(location);
		}
	}
}
