using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoteApi.Data;
using VoteApi.Hubs;

namespace VoteApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration, ILogger<Startup> logger)
		{
			Configuration = configuration;
			Logger = logger;
		}

		public IConfiguration Configuration { get; }
		public ILogger<Startup> Logger { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			Logger.LogDebug("Configuring services...");
			services.AddCors(opts => opts.AddDefaultPolicy(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

			services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("VoteApi"));
			services.AddCors();
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			Logger.LogDebug("Configuring ...");

			app.UseDeveloperExceptionPage();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


			app.UseFileServer();

			app.UseSignalR(routes =>
			{
				routes.MapHub<VoteHub>("/voteHub");
			});
			app.UseMvc();
		}
	}
}
