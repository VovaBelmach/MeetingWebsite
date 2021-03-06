﻿using System;
using System.Text;
using System.Threading.Tasks;
using MeetingWebsite.Api.Hub;
using MeetingWebsite.BLL.Services;
using MeetingWebsite.DAL.EF;
using MeetingWebsite.DAL.Interfaces;
using MeetingWebsite.DAL.Repositories;
using MeetingWebsite.Models.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace MeetingWebsite.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddDbContext<MeetingDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            }
            );

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MeetingDbContext>()
                .AddDefaultTokenProviders();

            services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                var resolver = options.SerializerSettings.ContractResolver;
                if (resolver != null)
                    ((DefaultContractResolver)resolver).NamingStrategy = null;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JwT_Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chat")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSignalR();

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
            //})
            //    .AddGoogle("Google", options =>
            //    {
            //        options.CallbackPath = new PathString("/signin-google");
            //        options.ClientId = "526768688788-7b660hm931dfann35p93pn34cle8h2r6.apps.googleusercontent.com";
            //        options.ClientSecret = "KRVsEMbicD2sfWFLxHri6rjg";
            //    });

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IBlacklistService, BlacklistService>();
            services.AddScoped<IFriendService, FriendService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IDialogService, DialogService>();
            services.AddScoped<IPurposeService, PurposeService>();
            services.AddScoped<IUserPurposeService, UserPurposeService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IUserLanguagesService, UserLanguagesService>();
            services.AddScoped<IBadHabitsService, BadHabitsService>();
            services.AddScoped<IUserBadHabitsService, UserBadHabitsService>();
            services.AddScoped<IInterestsService, InterestsService>();
            services.AddScoped<IUserInterestsService, UserInterestsService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<IFinancialSituationService, FinancialSituationService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<INationalityService, NationalityService>();
            services.AddScoped<IZodiacSignsService, ZodiacSignsService>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("MyAllowSpecificOrigins");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
            app.UseStaticFiles();


            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/chat"); });
        }
    }
}
