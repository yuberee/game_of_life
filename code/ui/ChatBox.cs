﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace GameOfLife
{

	public class LogEntry
	{

		public string User { get; set; }
		public string Message { get; set; }
		public ChatEntry Entry { get; set; }
		public int Multiplier { get; set; }

		public LogEntry( string _User , string _Message, ChatEntry _Entry = null, int _Multiplier = 0)
		{

			User = _User;
			Message = _Message;
			Entry = _Entry;
			Multiplier = _Multiplier;
				
		}

	}

	public partial class ChatEntry : Panel
	{

		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Label Multiplier { get; internal set; }


		public ChatEntry()
		{

			NameLabel = Add.Label( "Name", "name" );
			Message = Add.Label( "Message", "message" );
			Multiplier = Add.Label( "", "multiplier" );

		}

	}

	public partial class ChatBox : Panel
	{

		static ChatBox Current;

		public static Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		public ChatBox()
		{
			Current = this;

			StyleSheet.Load( "ui/HUD.scss" );

			Canvas = Add.Panel( "chat_canvas" );
			Canvas.PreferScrollToBottom = true;

			Input = Add.TextEntry( "Press [ENTER] to type" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			Sandbox.Hooks.Chat.OnOpenChat += Open;
		}


		void Open()
		{

			AddClass( "open" );
			Input.Focus();

			Input.Text = "";

		}

		void Close()
		{

			RemoveClass( "open" );
			Input.Blur();

			Input.Text = "Press [ENTER] to type";

		}

		void Submit()
		{

			var msg = Input.Text.Trim();

			Close();

			if ( string.IsNullOrWhiteSpace( msg ) ) return;

			Say( msg );

		}


		[ClientRpc]
		public static void AddChatEntry( string name, string message )
		{

			if ( !Global.IsListenServer )
			{

				Log.Info( $"{name}: {message}" );

			}

			if ( GameOfLife.ChatMessages.Count > 0 )
			{

				var lastMessage = GameOfLife.ChatMessages[ GameOfLife.ChatMessages.Count - 1];

				if ( lastMessage.Message == message )
				{

					lastMessage.Multiplier++;
					lastMessage.Entry.Multiplier.Text = $" x{lastMessage.Multiplier}";

					return;

				}

			}

			var entry = Canvas.AddChild<ChatEntry>();
			entry.Message.Text = message;
			entry.NameLabel.Text = name + ":";

			GameOfLife.ChatMessages.Add( new LogEntry( name, message, entry) );

			entry.SetClass( "noname", string.IsNullOrEmpty( name ) );

		}

		[ServerCmd]
		public static void SayInfo( string message )
		{

			Log.Info( message );
			AddChatEntry( To.Everyone, null, message );

			GameOfLife.ChatMessages.Add( new LogEntry( null, message ) );

		}

		[ServerCmd( "say" )]
		public static void Say( string message )
		{

			Assert.NotNull( ConsoleSystem.Caller );

			if ( message.Contains( '\n' ) || message.Contains( '\r' ) ) return;

			Log.Info( $"{ConsoleSystem.Caller.Name}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message );

			GameOfLife.ChatMessages.Add( new LogEntry( ConsoleSystem.Caller.Name, message ) );

		}

	}

}