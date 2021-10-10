﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class GoLHUD : Panel
	{

		public GoLHUD()
		{

			StyleSheet.Load( "ui/HUD.scss" );

			var sidebar = Add.Panel( "sidebar" );
			var tools = Add.Panel( "tools" );
			CellGrid.GridPanel = Add.Panel( "grid" );

			var title = sidebar.Add.Panel( "title" );
			title.Add.Label( "Game of Life" );

			var patterns = sidebar.Add.Panel( "patterns" );
			var patternsTitle = patterns.Add.Panel( "patternstitle" );
			patternsTitle.Add.Label( "Patterns" );
			var patternsContainer = patterns.Add.Panel( "patterncontainer" );

			for( int i = 0; i < 100; i++)
			{

				patternsContainer.AddChild<PatternEntry>();


			}

			// [PLAY] button
			var play = tools.Add.Button( "", "buttons" );
			CellGrid.PlayLabel = play.Add.Label( "▸", "play" );
			play.AddEventListener( "onclick", () =>
			{

				CellGrid.Play( !CellGrid.Playing, true );
				PlaySound( "click2" );

			} );
			

			// [NEXT] button
			var next = tools.Add.Button( "", "buttons" );
			next.Add.Label( "⇥", "next" );
			next.AddEventListener( "onclick", () =>
			{

				CellGrid.Next( true );
				PlaySound( "click2" );

			} );

			// [CLEAR] button
			var clear = tools.Add.Button( "", "buttons" );
			clear.Add.Label( "⨯", "clear" );
			clear.AddEventListener( "onclick", () =>
			{

				CellGrid.ClearGrid( true );
				PlaySound( "click2" );

			} );

			// [LOOP] button
			var loop = tools.Add.Button( "", "buttons" );
			loop.Add.Label( "⟳", "loop" );
			CellGrid.LoopCross = loop.Add.Label( "✕", "cross" );
			CellGrid.LoopCross.Style.Opacity = 0;
			loop.AddEventListener( "onclick", () =>
			{

				CellGrid.Loop( !CellGrid.Looping, true );
				PlaySound( "click2" );

			} );

			// [SPEED] button
			var speed = tools.Add.Button( "", "buttons" );
			CellGrid.SpeedLabel = speed.Add.Label( "⨯1", "speed" );
			speed.AddEventListener( "onclick", () =>
			{

				CellGrid.SetSpeed( (CellGrid.Speed + 1 ) % CellGrid.ValidSpeeds.Count , true );
				PlaySound( "click2" );

			} );

			CellGrid.GridPanel.Style.PixelSnap = 0;

			CellGrid.GridPanel.AddChild<CellPanel>();
			var chat = sidebar.Add.Panel( "chat" );
			chat.AddChild<ChatBox>();

		}

		public override void OnButtonEvent( ButtonEvent e )
		{

			if ( CellGrid.Playing ) { return; }

			if ( e.Button == "mouseleft" && e.Pressed == true )
			{

				int x = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.x / CellGrid.GridPanel.Box.Rect.width * 50.3f );
				int y = (int)MathX.Floor( CellGrid.GridPanel.MousePosition.y / CellGrid.GridPanel.Box.Rect.height * 50.3f );

				if( x >= 0 && x <= CellGrid.GridSize.x && y >= 0 && y <= CellGrid.GridSize.y )
				{

					CellGrid.UpdateCell( x, y, !CellGrid.Cell( x, y ).Alive, true );

					PlaySound( "click1" );

				}

			}

		}

	}

	public class CellPanel : Panel
	{

		public CellPanel()
		{

			var panel = Add.Panel( "cell" );

			panel.Style.PixelSnap = 0;

			CellGrid.CellPanel = panel;

			var shadowList = new ShadowList();

			for ( int x = 0; x < CellGrid.GridSize.x; x++ )
			{

				for ( int y = 0; y < CellGrid.GridSize.y; y++ )
				{

					var shadow = new Shadow { OffsetX = x * 19.24f + 3, OffsetY = y * 19.24f + 3, Color = Color.Black };

					CellGrid.Cell( x, y ).Shadow = shadow;

					shadowList.Add( shadow );

				}

			}

			panel.Style.BoxShadow = shadowList;

		}

	}

	public class PatternEntry : Panel
	{

		string[] names = new string[] { "Glider", "Glider Cannon", "Rose", "Bomb", "Block", "Wiggler", "Mark" };

		public PatternEntry()
		{

			var panel = Add.Panel( "pattern" );
			var entryName = panel.Add.Panel( "name" );
			entryName.Add.Label( names[new Random().Int( 0, 6 )] );

		}

	}


	public partial class HUD : Sandbox.HudEntity<RootPanel>
	{

		public HUD()
		{
			if ( !IsClient ) return;

			RootPanel.StyleSheet.Load( "ui/HUD.scss" );

			
			RootPanel.AddChild<GoLHUD>();


		}

	}

}
