﻿using LoadOrderToolTwo.Utilities.Managers;

namespace LoadOrderToolTwo.UserInterface.Panels;

partial class PC_MainPage
{
	/// <summary> 
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary> 
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			CentralManager.ContentLoaded -= SetButtonEnabledOnLoad;
			CitiesManager.MonitorTick -= CitiesManager_MonitorTick;
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
			SlickControls.DynamicIcon dynamicIcon1 = new SlickControls.DynamicIcon();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.B_StartStop = new SlickControls.SlickButton();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.TLP_Profiles = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.profileBubble = new LoadOrderToolTwo.UserInterface.StatusBubbles.ProfileBubble();
			this.modsBubble = new LoadOrderToolTwo.UserInterface.StatusBubbles.ModsBubble();
			this.assetsBubble = new LoadOrderToolTwo.UserInterface.StatusBubbles.AssetsBubble();
			this.compatibilityReportBubble = new LoadOrderToolTwo.UserInterface.StatusBubbles.CompatibilityReportBubble();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.TLP_Profiles.SuspendLayout();
			this.SuspendLayout();
			// 
			// base_Text
			// 
			this.base_Text.Text = "Dashboard";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.B_StartStop, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 30);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(778, 403);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// B_StartStop
			// 
			this.B_StartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.B_StartStop.ColorShade = null;
			this.B_StartStop.Cursor = System.Windows.Forms.Cursors.Hand;
			this.B_StartStop.Enabled = false;
			dynamicIcon1.Name = "I_AppIcon";
			this.B_StartStop.ImageName = dynamicIcon1;
			this.B_StartStop.Location = new System.Drawing.Point(551, 368);
			this.B_StartStop.Name = "B_StartStop";
			this.B_StartStop.Padding = new System.Windows.Forms.Padding(10, 5, 0, 5);
			this.B_StartStop.Size = new System.Drawing.Size(224, 32);
			this.B_StartStop.SpaceTriggersClick = true;
			this.B_StartStop.TabIndex = 0;
			this.B_StartStop.Text = "Launch Cities: Skylines";
			this.B_StartStop.Click += new System.EventHandler(this.B_StartStop_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
			this.flowLayoutPanel1.Controls.Add(this.TLP_Profiles);
			this.flowLayoutPanel1.Controls.Add(this.modsBubble);
			this.flowLayoutPanel1.Controls.Add(this.assetsBubble);
			this.flowLayoutPanel1.Controls.Add(this.compatibilityReportBubble);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(778, 365);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// TLP_Profiles
			// 
			this.TLP_Profiles.AutoSize = true;
			this.TLP_Profiles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Profiles.ColumnCount = 1;
			this.TLP_Profiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_Profiles.Controls.Add(this.profileBubble, 0, 0);
			this.TLP_Profiles.Location = new System.Drawing.Point(0, 0);
			this.TLP_Profiles.Margin = new System.Windows.Forms.Padding(0);
			this.TLP_Profiles.Name = "TLP_Profiles";
			this.TLP_Profiles.RowCount = 1;
			this.TLP_Profiles.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Profiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Profiles.Size = new System.Drawing.Size(644, 510);
			this.TLP_Profiles.TabIndex = 13;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 373);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 30);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			this.label1.Visible = false;
			// 
			// profileBubble
			// 
			this.profileBubble.Cursor = System.Windows.Forms.Cursors.Hand;
			this.profileBubble.Location = new System.Drawing.Point(3, 3);
			this.profileBubble.Name = "profileBubble";
			this.profileBubble.Size = new System.Drawing.Size(638, 504);
			this.profileBubble.TabIndex = 0;
			this.profileBubble.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProfileBubble_MouseClick);
			// 
			// modsBubble
			// 
			this.modsBubble.Cursor = System.Windows.Forms.Cursors.Hand;
			this.modsBubble.Location = new System.Drawing.Point(3, 513);
			this.modsBubble.Name = "modsBubble";
			this.modsBubble.Size = new System.Drawing.Size(150, 47);
			this.modsBubble.TabIndex = 1;
			this.modsBubble.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ModsBubble_MouseClick);
			// 
			// assetsBubble
			// 
			this.assetsBubble.Cursor = System.Windows.Forms.Cursors.Hand;
			this.assetsBubble.Location = new System.Drawing.Point(159, 513);
			this.assetsBubble.Name = "assetsBubble";
			this.assetsBubble.Size = new System.Drawing.Size(150, 47);
			this.assetsBubble.TabIndex = 2;
			this.assetsBubble.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AssetsBubble_MouseClick);
			// 
			// compatibilityReportBubble
			// 
			this.compatibilityReportBubble.Cursor = System.Windows.Forms.Cursors.Hand;
			this.compatibilityReportBubble.Location = new System.Drawing.Point(315, 513);
			this.compatibilityReportBubble.Name = "compatibilityReportBubble";
			this.compatibilityReportBubble.Size = new System.Drawing.Size(150, 47);
			this.compatibilityReportBubble.TabIndex = 3;
			this.compatibilityReportBubble.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CompatibilityReportBubble_MouseClick);
			// 
			// PC_MainPage
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.tableLayoutPanel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(58)))), ((int)(((byte)(69)))));
			this.Name = "PC_MainPage";
			this.Padding = new System.Windows.Forms.Padding(0, 30, 5, 5);
			this.Text = "Dashboard";
			this.Controls.SetChildIndex(this.base_Text, 0);
			this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.TLP_Profiles.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	internal SlickControls.SlickButton B_StartStop;
	private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	private UserInterface.StatusBubbles.ProfileBubble profileBubble;
	private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
	private UserInterface.StatusBubbles.ModsBubble modsBubble;
	private UserInterface.StatusBubbles.AssetsBubble assetsBubble;
	private StatusBubbles.CompatibilityReportBubble compatibilityReportBubble;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.TableLayoutPanel TLP_Profiles;
}
