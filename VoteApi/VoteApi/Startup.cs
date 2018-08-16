﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoteApi.Data;
using VoteApi.Hubs;

namespace VoteApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opts => opts.AddDefaultPolicy(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            //// TODO: need to be able to create a transaction on request start
            //services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("VoteApi"));

            services.AddDbContext<ApiContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString(nameof(ApiContext))));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSignalR(routes =>
            {
                routes.MapHub<VoteHub>("/voteHub");
            });
            app.UseMvc();
        }
    }
}