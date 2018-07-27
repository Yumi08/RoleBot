using System.Threading.Tasks;
using Discord.Commands;

namespace RoleBot.Modules
{
	public class Test : ModuleBase<SocketCommandContext>
	{
		[Command("test")]
		public async Task Test_()
		{
			await Context.Channel.SendMessageAsync("I live!");
		}
	}
}