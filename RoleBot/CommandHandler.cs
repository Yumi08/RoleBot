﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace RoleBot
{
	class CommandHandler
	{
		public static event EventHandler<MessageReceievedEventArgs> MessageReceived; 

		private DiscordSocketClient _client;
		private CommandService _service;

		public async Task InitializeAsync(DiscordSocketClient client)
		{
			_client = client;
			_service = new CommandService();
			await _service.AddModulesAsync(Assembly.GetEntryAssembly());
			_client.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage s)
		{
			var msg = (SocketUserMessage) s;
			if (msg == null) return;
			var context = new SocketCommandContext(_client, msg);
			if (context.User.IsBot) return;

			var argPos = 0;
			// NOTE: ToString() can cause issues by spitting out the type instead of the char
			if (!msg.HasStringPrefix(Config.Bot.cmdPrefix.ToString(), ref argPos))
			{
				MessageReceievedEventArgs e = new MessageReceievedEventArgs {Msg = msg};
				OnMessageReceived(e);
			}

				if ((msg.HasStringPrefix(Config.Bot.cmdPrefix.ToString(), ref argPos)
			|| msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) && !context.IsPrivate)
			{
				var result = await _service.ExecuteAsync(context, argPos);

				if (!result.IsSuccess /*&& result.Error != CommandError.UnknownCommand*/)
				{
					Console.WriteLine(result.ErrorReason);
				}
			}
		}

		public struct MessageReceievedEventArgs
		{
			public SocketUserMessage Msg { get; set; }
		}

		protected virtual void OnMessageReceived(MessageReceievedEventArgs e)
		{
			MessageReceived?.Invoke(this, e);
		}
	}
}