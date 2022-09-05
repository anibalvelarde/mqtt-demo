﻿using ConsoleGUI;
using ConsoleGUI.Controls;
using ConsoleGUI.Data;
using ConsoleGUI.Input;
using ConsoleGUI.Space;
using System;
using System.Threading;

namespace mqtt.demo
{
	class Program
	{
		static void Main()
		{
			var clock = new TextBlock();

			var canvas = new Canvas();
			var textBox = new TextBox();
			var mainConsole = new LogPanel();
			var secondaryConsole = new LogPanel();

			var leaderboard = new DataGrid<Site>
			{
				Columns = new[]
				{
					new DataGrid<Site>.ColumnDefinition("Site", 12, p => p.Name, foreground: p => p.Name == "Rosemount" ? (Color?)new Color(100, 100, 220) : null, textAlignment: TextAlignment.Right),
					new DataGrid<Site>.ColumnDefinition("Address", 22, p => p.Address),
					new DataGrid<Site>.ColumnDefinition("Created On", 11, p => p.CreaedDate.ToShortDateString(), textAlignment: TextAlignment.Center),
					new DataGrid<Site>.ColumnDefinition("Equipment Count", 15, p => p.Machines.Count().ToString(), background: p => p.Machines.Any(m => m.IsRunning) ? (Color?)new Color(0, 220, 0) : null, textAlignment: TextAlignment.Right)
				},
				Data = new[]
				{
					new Site("Telford", "123 someplace", 38, "TLF-10"),
					new Site("Rosemount", "555 Overthere", 40, "RM-100"),
					new Site("Plymouth", "8675309 Plc", 3, "PY-123"),
					new Site("Brklyn Park", "P.Sherman Wallaby Way", 5, "BP-001"),
				},
				Style = DataGridStyle.AllBorders
			};

			var tabPanel = new TabPanel();

			tabPanel.AddTab("Sites", new Box
			{
				HorizontalContentPlacement = Box.HorizontalPlacement.Center,
				VerticalContentPlacement = Box.VerticalPlacement.Center,
				Content = new Background
				{
					Color = new Color(45, 74, 85),
					Content = new Border
					{
						BorderStyle = BorderStyle.Single,
						Content = leaderboard
					}
				}
			});

			tabPanel.AddTab("About", new Box
			{
				HorizontalContentPlacement = Box.HorizontalPlacement.Center,
				VerticalContentPlacement = Box.VerticalPlacement.Center,
				Content = new Boundary
				{
					MaxWidth = 20,
					Content = new VerticalStackPanel
					{
						Children = new IControl[]
						{
							new WrapPanel
							{
								Content = new TextBlock { Text = "This more complex console UI facade for an MQTT client using ConsoleGUI package, by Tomasz Rewak." }
							},
							new HorizontalSeparator(),
							new TextBlock {Text ="by AEV", Color = new Color(200, 200, 200)}
						}
					}
				}
			});

			var dockPanel = new DockPanel
			{
				Placement = DockPanel.DockedControlPlacement.Top,
				DockedControl = new DockPanel
				{
					Placement = DockPanel.DockedControlPlacement.Right,
					DockedControl = new Background
					{
						Color = new Color(100, 100, 100),
						Content = new Boundary
						{
							MinWidth = 20,
							Content = new Box
							{
								Content = clock,
								HorizontalContentPlacement = Box.HorizontalPlacement.Center
							}
						}
					},
					FillingControl = new Background
					{
						Color = ConsoleColor.DarkRed,
						Content = new Box
						{
							Content = new TextBlock { Text = "MQTT Console UI" },
							HorizontalContentPlacement = Box.HorizontalPlacement.Center
						}
					}
				},
				FillingControl = new DockPanel
				{
					Placement = DockPanel.DockedControlPlacement.Bottom,
					DockedControl = new Boundary
					{
						MinHeight = 1,
						MaxHeight = 1,
						Content = new Background
						{
							Color = new Color(0, 100, 0),
							Content = new HorizontalStackPanel
							{
								Children = new IControl[] {
									new TextBlock { Text = " 10 ↑ " },
									new VerticalSeparator(),
									new TextBlock { Text = " 5 ↓ " }
								}
							}
						}
					},
					FillingControl = new Overlay
					{
						BottomContent = new Background
						{
							Color = new Color(25, 54, 65),
							Content = new DockPanel
							{
								Placement = DockPanel.DockedControlPlacement.Right,
								DockedControl = new Background
								{
									Color = new Color(30, 40, 50),
									Content = new Border
									{
										BorderPlacement = BorderPlacement.Left,
										BorderStyle = BorderStyle.Double.WithColor(new Color(50, 60, 70)),
										Content = new Boundary
										{
											MinWidth = 40,
											MaxWidth = 40,
											Content = new DockPanel
											{
												Placement = DockPanel.DockedControlPlacement.Bottom,
												DockedControl = new Boundary
												{
													MaxHeight = 1,
													Content = new HorizontalStackPanel
													{
														Children = new IControl[]
														{
															new Style
															{
																Foreground = new Color(150, 150, 200),
																Content = new TextBlock { Text = @"D:\Software\> " }
															},
															textBox
														}
													}
												},
												FillingControl = new Box
												{
													VerticalContentPlacement = Box.VerticalPlacement.Bottom,
													HorizontalContentPlacement = Box.HorizontalPlacement.Stretch,
													Content = mainConsole
												}
											}
										}
									}
								},
								FillingControl = new DockPanel
								{
									Placement = DockPanel.DockedControlPlacement.Right,
									DockedControl = new Background
									{
										Color = new Color(20, 30, 40),
										Content = new Border
										{
											BorderPlacement = BorderPlacement.Left,
											BorderStyle = BorderStyle.Double.WithColor(new Color(50, 60, 70)),
											Content = new Boundary
											{
												MinWidth = 40,
												MaxWidth = 40,
												Content = new Box
												{
													VerticalContentPlacement = Box.VerticalPlacement.Bottom,
													HorizontalContentPlacement = Box.HorizontalPlacement.Stretch,
													Content = secondaryConsole
												}
											}
										}
									},
									FillingControl = tabPanel
								}
							}
						},
						TopContent = new Box
						{
							HorizontalContentPlacement = Box.HorizontalPlacement.Center,
							VerticalContentPlacement = Box.VerticalPlacement.Center,
							Content = new Boundary
							{
								Width = 41,
								Height = 12,
								Content = canvas
							}
						}
					}
				}
			};

			var scrollPanel = new VerticalScrollPanel
			{
				Content = new SimpleDecorator
				{
					Content = new VerticalStackPanel
					{
						Children = new IControl[]
						{
							new WrapPanel { Content = new TextBlock{Text = "Here is a short example of text wrapping" } },
							new HorizontalSeparator(),
							new WrapPanel { Content = new TextBlock{Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." } },
						}
					}
				}
			};

			//canvas.Add(new Background
			//{
			//	Color = new Color(10, 10, 10),
			//	Content = new VerticalStackPanel
			//	{
			//		Children = new IControl[]
			//		{
			//			new Border
			//			{
			//				BorderPlacement = BorderPlacement.Left | BorderPlacement.Top | BorderPlacement.Right,
			//				BorderStyle = BorderStyle.Double.WithColor(new Color(80, 80, 120)),
			//				Content = new Box
			//				{
			//					HorizontalContentPlacement = Box.HorizontalPlacement.Center,
			//					Content = new TextBlock { Text = "Popup 1", Color = new Color(200, 200, 100) }
			//				}
			//			},
			//			new Border
			//			{
			//				BorderPlacement = BorderPlacement.All,
			//				BorderStyle = BorderStyle.Double.WithTopLeft(new Character('╠')).WithTopRight(new Character('╣')).WithColor(new Color(80, 80, 120)),
			//				Content = scrollPanel
			//			}
			//		}
			//	}
			//}, new Rect(11, 0, 30, 10));

			//canvas.Add(new Background
			//{
			//	Color = new Color(10, 40, 10),
			//	Content = new Border
			//	{
			//		Content = new Box
			//		{
			//			HorizontalContentPlacement = Box.HorizontalPlacement.Center,
			//			VerticalContentPlacement = Box.VerticalPlacement.Center,
			//			Content = new TextBlock { Text = "Popup 2" }
			//		}
			//	}
			//}, new Rect(0, 7, 17, 5));

			ConsoleManager.Setup();
			ConsoleManager.Resize(new Size(150, 40));
			ConsoleManager.Content = dockPanel;

			var input = new IInputListener[]
			{
				scrollPanel,
				tabPanel,
				new InputController(textBox, mainConsole),
				textBox
			};

			for (int i = 0; ; i++)
			{
				Thread.Sleep(10);

				clock.Text = DateTime.Now.ToLongTimeString();
				if (i % 200 == 0) secondaryConsole.Add($"Ping {i / 200 + 1}");

				ConsoleManager.ReadInput(input);
				ConsoleManager.AdjustBufferSize();
			}
		}
	}
}