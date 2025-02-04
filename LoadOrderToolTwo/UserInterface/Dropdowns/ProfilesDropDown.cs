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

namespace LoadOrderToolTwo.UserInterface.Dropdowns;
internal class ProfilesDropDown : SlickSelectionDropDown<Profile>
{
	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);

		if (Live)
		{
			Items = ProfileManager.Profiles.ToArray();
		}
	}

	protected override IEnumerable<DrawableItem<Profile>> OrderItems(IEnumerable<DrawableItem<Profile>> items)
	{
		return items.OrderByDescending(x => x.Item.Temporary).ThenByDescending(x => x.Item.LastEditDate);
	}

	protected override void PaintItem(PaintEventArgs e, Rectangle rectangle, Color foreColor, HoverState hoverState, Profile item)
	{
		if (item is null)
		{ return; }

		var text = item.Name;

		if (item.Temporary)
		{
			text = Locale.Unfiltered;
		}

		using var icon = IconManager.GetIcon(item.Temporary ? "I_Slash" : item.ForGameplay ? "I_City" : item.ForAssetEditor ? "I_Tools" : "I_ProfileSettings", rectangle.Height - 2).Color(foreColor);

		e.Graphics.DrawImage(icon, rectangle.Align(icon.Size, ContentAlignment.MiddleLeft));

		e.Graphics.DrawString(text, Font, new SolidBrush(foreColor), rectangle.Pad(icon.Width + Padding.Left, 0, 0, 0).AlignToFontSize(Font, ContentAlignment.MiddleLeft, e.Graphics), new StringFormat { Trimming = StringTrimming.EllipsisCharacter });
	}
}
