using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoteApi.Data;

namespace VoteApi.Test
{
	public class InMemoryApplicationFactory<TStartup> : WebApplicationFactory<VoteApi.Startup>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{

				var serviceProvider = new ServiceCollection()
					.AddEntityFrameworkInMemoryDatabase()
					.BuildServiceProvider();

				services.AddDbContext<ApiContext>(opts =>
				{
					opts.UseInMemoryDatabase("InMemoryDatabaseForTesting");
					opts.UseInternalServiceProvider(serviceProvider);
				});
			});
		}
	}
}
