using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.Hubs;
using app.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.IdentityModel.Tokens;

namespace app
{
   public class Startup
   {
      public const string CookieAuthScheme = "CookieAuthScheme";
      public const string JWTAuthScheme = "JWTAuthScheme";



      public Startup(/*ILoggerFactory loggerFactory, */IConfiguration configuration, IWebHostEnvironment env)
      {
         Configuration = configuration;
         _env = env;
         //_logger = loggerFactory.CreateLogger<Startup>();
      }

      public IConfiguration Configuration { get; }
      public IWebHostEnvironment _env;
      //private ILogger<Startup> _logger;

      private string GetConString()
      {
         return Configuration["ConnectionStrings:UserDBContext"];
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddControllers();
         services.AddCors();
       
         services.AddMvc().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

         //services.AddRazorPages()
         //    .AddRazorPagesOptions(options =>
         //    {
         //       //options.Conventions.AddPageRoute("index", "{*url}");
         //       options.Conventions.AddPageRoute("/index", "r/{roomId}");
         //       options.Conventions.AddPageRoute("/index/r/{roomId}", "r/{roomId}");
         //    });

         services.AddLogging(builder =>
         {
            builder.AddConsole()
                   .AddDebug()
                   .AddFilter<ConsoleLoggerProvider>(category: null, level: LogLevel.Debug)
                   .AddFilter<DebugLoggerProvider>(category: null, level: LogLevel.Debug);
         });

         if (_env.IsDevelopment())
         {
            //services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            ConfigureAuth(services);
         }
         else
         {
            ConfigureAuth(services);
         }



         // Tells SignalR how to get the User unique Id from the ClaimsPrincipal
         services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
         services.AddSignalR();
         services.AddDbContext<UsersDBContext>(options =>
                        options.UseSqlServer(GetConString()));
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app)
      {
         if (_env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
         }

         // Enable CORS so the Vue client can send requests
         app.UseCors(builder =>
             builder
                 .WithOrigins("http://localhost:8080")
                 .WithOrigins("http://localhost:8082")
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials()
         );

         //ToDo Make sure to redirect in production
         //app.UseHttpsRedirection();

         app.UseRouting();
         //app.UseStaticFiles();
         app.UseAuthentication();
         app.UseAuthorization();
         app.UseEndpoints(endpoints =>
         {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapHub<RoomsHub>("/roomsHub");
            endpoints.MapHub<NotificationHub>("/notification-hub");
         });
      }

      private void ConfigureAuth(IServiceCollection services)
      {


         var key = Encoding.ASCII.GetBytes(Configuration["TokenAuthentication:SecretKey"]);
         services.AddAuthentication(x =>
         {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         })
         .AddJwtBearer(x =>
         {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(key),
               ValidateIssuer = false,
               ValidateAudience = false
            };
            x.Events = new JwtBearerEvents
            {
               OnMessageReceived = ctx =>
                  {

                    var accessToken = ctx.Request.Query["access_token"];

                       // If the request is for signalling hub...
                       var path = ctx.HttpContext.Request.Path;


                    if (String.IsNullOrEmpty(path))
                    {
                       Debug.WriteLine("Path is null");
                       Console.WriteLine("Path is null");
                    }
                    else
                    {
                       Debug.WriteLine(string.Format("Path {0}", path));
                       Console.WriteLine(string.Format("Path {0}", path));
                    }

                    if (String.IsNullOrEmpty(accessToken))
                    {
                       Debug.WriteLine("Access token is null");
                       Console.WriteLine("Access token is null");
                    }
                    else
                    {
                       Debug.WriteLine(string.Format("access token {0}", accessToken));
                       Console.WriteLine(string.Format("access token {0}", accessToken));
                    }

                    if (!string.IsNullOrEmpty(accessToken) &&
                           (path.StartsWithSegments("/notification-hub")))
                    {
                          // Read the token out of the query string
                          ctx.Token = accessToken;
                    }

                    return Task.CompletedTask;
                 }

            };
         });






         //var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenAuthentication:SecretKey"]));

         //// Sets the default scheme to cookies
         //services.AddAuthentication(JWTAuthScheme)
         //    //// Now configure specific Cookie and JWT auth options
         //    //.AddCookie(CookieAuthScheme, options =>
         //    //{
         //    //    // Set the cookie
         //    //    options.Cookie.Name = "ptweb.AuthCookie";
         //    //    // Set the samesite cookie parameter as none, otherwise CORS scenarios where the client uses a different domain wont work!
         //    //    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
         //    //    // Simply return 401 responses when authentication fails (as opposed to default redirecting behaviour)
         //    //    options.Events = new CookieAuthenticationEvents
         //    //    {
         //    //        OnRedirectToLogin = redirectContext =>
         //    //        {
         //    //            redirectContext.HttpContext.Response.StatusCode = 401;
         //    //            return Task.CompletedTask;
         //    //        }
         //    //    };
         //    //    // In order to decide the between both schemas
         //    //    // inspect whether there is a JWT token either in the header or query string
         //    //    options.ForwardDefaultSelector = ctx =>
         //    //    {
         //    //        if (ctx.Request.Query.ContainsKey("access_token")) return JWTAuthScheme;
         //    //        if (ctx.Request.Headers.ContainsKey("Authorization")) return JWTAuthScheme;
         //    //        return CookieAuthScheme;
         //    //    };
         //    //})
         //    .AddJwtBearer(JWTAuthScheme, options =>
         //    {

         //        /*
         //        // Configure JWT Bearer Auth to expect our security key
         //        options.TokenValidationParameters = new TokenValidationParameters
         //        {
         //            //LifetimeValidator = (before, expires, token, param) =>
         //            //{
         //            //    return expires > DateTime.UtcNow;
         //            //},
         //            //ValidateAudience = false,
         //            //ValidateIssuer = false,
         //            //ValidateActor = false,
         //            //ValidateLifetime = true,
         //            //IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("this one would be a really real secret")),
         //            //ClockSkew = TimeSpan.Zero,
         //            ValidateIssuerSigningKey = true,
         //            IssuerSigningKey = signingKey,
         //            // Validate the JWT Issuer (iss) claim
         //            ValidateIssuer = true,
         //            ValidIssuer = Configuration["TokenAuthentication:Issuer"],
         //            // Validate the JWT Audience (aud) claim
         //            ValidateAudience = true,
         //            ValidAudience = Configuration["TokenAuthentication:Audience"],
         //            // Validate the token expiry
         //            ValidateLifetime = true,
         //            // If you want to allow a certain amount of clock drift, set that here:
         //            ClockSkew = TimeSpan.Zero,

         //        };

         //        */


         //        //options.Authority = "http://localhost:5000/";
         //        //options.Audience = "http://localhost:5001/";
         //        //options.RequireHttpsMetadata = false;


         //        //options.Events = new JwtBearerEvents
         //        //{
         //        //    OnMessageReceived = context =>
         //        //    {
         //        //        var accessToken = context.Request.Query["access_token"];

         //        //        // If the request is for our hub...
         //        //        var path = context.HttpContext.Request.Path;
         //        //        _logger.LogInformation("Hub Path: {0}", path);
         //        //        if (!string.IsNullOrEmpty(accessToken) &&
         //        //            (path.StartsWithSegments("/notification-hub")))
         //        //        {
         //        //            // Read the token out of the query string
         //        //            context.Token = accessToken;
         //        //        }
         //        //        return Task.CompletedTask;
         //        //    }
         //        //};






         //        options.Authority = "http://localhost:5000/";
         //        options.Audience = "http://localhost:5001/";
         //        options.RequireHttpsMetadata = false;

         //        // The JwtBearer scheme knows how to extract the token from the Authorization header
         //        // but we will need to manually extract it from the query string in the case of requests to the hub

         //        options.Events = new JwtBearerEvents
         //        {
         //            OnMessageReceived = ctx =>
         //            {
         //                var accessToken = ctx.Request.Query["access_token"];
         //                Debug.Write("Hub Token: {0}", accessToken);
         //                //var path = ctx.HttpContext.Request.Path;
         //                //Debug.Write("Hub Path: {0}", path);


         //                //if (!string.IsNullOrEmpty(accessToken) &&
         //                //    (path.StartsWithSegments("/notification-hub")))
         //                //{
         //                //    ctx.Token = accessToken;
         //                //}

         //                if (ctx.Request.Query.ContainsKey("access_token"))
         //                {
         //                    ctx.Token = accessToken;
         //                }
         //                return Task.CompletedTask;
         //            }
         //        };



         //    });

      }

      //private Task<ClaimsIdentity> GetIdentity(string username, string password)
      //{

      //    if (username == "TEST" && password == "TEST123")
      //    {
      //        return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { }));
      //    }

      //    // Account doesn't exist
      //    return Task.FromResult<ClaimsIdentity>(null);
      //}

   }


}
