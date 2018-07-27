using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RoleBot.Modules
{
	public class Roles : ModuleBase<SocketCommandContext>
	{
		private const string SelfRoleFile = "resources\\selfroles.txt";

		List<SocketRole> GetSelfRoles()
		{
			var roles = new List<SocketRole>();

			if (!File.Exists(SelfRoleFile) || File.ReadAllText(SelfRoleFile) == "")
				return roles;

			foreach (var roleId in File.ReadAllText(SelfRoleFile).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
			{
				roles.Add(Context.Guild.Roles.First(r => r.Id.ToString() == roleId));
			}

			return roles;
		}

		[Command("getroles")]
		public async Task GetRoles()
		{
			var embed = new EmbedBuilder();
			var embedText = "";

			foreach (var socketRole in Context.Guild.Roles)
			{
				if (socketRole.Name != "@everyone")
					embedText += socketRole.Name + "\n";
			}

			embed.Description = embedText;

			await Context.Channel.SendMessageAsync("", false, embed);
		}

		[Command("addselfrole")]
		public async Task AddSelfRole([Remainder] string role)
		{
			bool firstWrite = false;

			if (Context.Guild.Owner.Id != Context.User.Id)
			{
				await Context.Channel.SendMessageAsync("Only the owner can add and remove self roles.");
				return;
			}

			var selectedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == role);
			if (selectedRole == null) return;

			if (!File.Exists(SelfRoleFile))
			{
				File.WriteAllText(SelfRoleFile, "");
				firstWrite = true;
			}

			if (File.ReadAllText(SelfRoleFile).Split(new[] {','},
				StringSplitOptions.RemoveEmptyEntries).Any(r => r == selectedRole.Id.ToString()))
			{
				await Context.Channel.SendMessageAsync("Role already exists!");
				return;
			}

			File.WriteAllText(SelfRoleFile,
				!firstWrite ? $"{File.ReadAllText(SelfRoleFile)},{selectedRole.Id}" : selectedRole.Id.ToString());

			await Context.Channel.SendMessageAsync("Added!");
		}

		[Command("removeselfrole")]
		public async Task RemoveSelfRole([Remainder] string role)
		{
			if (Context.Guild.Owner.Id != Context.User.Id)
			{
				await Context.Channel.SendMessageAsync("Only the owner can add and remove self roles.");
				return;
			}

			var selectedRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == role);
			if (selectedRole == null) return;

			if (!File.Exists(SelfRoleFile))
			{
				File.WriteAllText(SelfRoleFile, "");
			}

			if (File.ReadAllText(SelfRoleFile).Split(new[] {','},
				StringSplitOptions.RemoveEmptyEntries).All(r => r != selectedRole.Id.ToString()))
			{
				await Context.Channel.SendMessageAsync("SelfRole doesn't exist!");
				return;
			}

			string newFile = File.ReadAllText(SelfRoleFile);

			if (newFile.IndexOf(selectedRole.Id.ToString(), StringComparison.Ordinal) > 2)
				newFile = newFile.Remove(newFile.IndexOf(selectedRole.Id.ToString(), StringComparison.Ordinal) - 1, 1);
			newFile = newFile.Replace(selectedRole.Id.ToString(), "");

			File.WriteAllText(SelfRoleFile, newFile);

			await Context.Channel.SendMessageAsync("Removed!");
		}

		[Command("selfroles")]
		public async Task SelfRoles()
		{
			var roles = GetSelfRoles();

			var embed = new EmbedBuilder();
			var embedText = "";

			foreach (var socketRole in roles)
			{
				if (socketRole.Name != "@everyone")
					embedText += socketRole.Name + "\n";
			}

			embed.Description = embedText;

			await Context.Channel.SendMessageAsync("", false, embed);
		}

		[Command("giverole")]
		public async Task GiveRole([Remainder] string role)
		{
			var selectedRole = GetSelfRoles().FirstOrDefault(r => r.Name == role);

			if (selectedRole == null)
				await Context.Channel.SendMessageAsync("No such SelfRole found!");

			await (Context.User as IGuildUser).AddRoleAsync(selectedRole);
		}

		[Command("takerole")]
		public async Task TakeRole([Remainder] string role)
		{
			var selectedRole = GetSelfRoles().FirstOrDefault(r => r.Name == role);

			if (selectedRole == null)
				await Context.Channel.SendMessageAsync("No such role found!");

			await (Context.User as IGuildUser).RemoveRoleAsync(selectedRole);
		}
	}
}