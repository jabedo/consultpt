using System;
using System.Collections.Generic;
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
        


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment _env;

        private string GetConString()
        {
            return Configuration["ConnectionStrings:UserDBContext"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();

            services.AddLogging(builder =>
            {
                builder.AddConsole()
                    .AddDebug()
                    .AddFilter<ConsoleLoggerProvider>(category: null, level: LogLevel.Debug)
                    .AddFilter<DebugLoggerProvider>(category: null, level: LogLevel.Debug);
            });

            if (_env.IsDevelopment())
            {
                services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
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
            app.UseAuthorization();


            #region UseWebSocketsOptionsAO
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            webSocketOptions.AllowedOrigins.Add("http://localhost:8080");
            webSocketOptions.AllowedOrigins.Add("https://localhost:8080");
            webSocketOptions.AllowedOrigins.Add("http://localhost:8082");
            webSocketOptions.AllowedOrigins.Add("https://localhost:8082");

            app.UseWebSockets(webSocketOptions);
#endregion

            #region AcceptWebSocket
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                 
                    }
                }
                else
                {
                    await next();
                }

            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<NotificationHub>("/notification-hub");
            });
        }

        private void ConfigureAuth(IServiceCollection services)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["TokenAuthentication:SecretKey"]));
            // Sets the default scheme to cookies
            services.AddAuthentication(CookieAuthScheme)
                // Now configure specific Cookie and JWT auth options
                .AddCookie(CookieAuthScheme, options =>
                {
                    // Set the cookie
                    options.Cookie.Name = "ptweb.AuthCookie";
                    // Set the samesite cookie parameter as none, otherwise CORS scenarios where the client uses a different domain wont work!
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    // Simply return 401 responses when authentication fails (as opposed to default redirecting behaviour)
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                    // In order to decide the between both schemas
                    // inspect whether there is a JWT token either in the header or query string
                    options.ForwardDefaultSelector = ctx =>
                    {
                        if (ctx.Request.Query.ContainsKey("access_token")) return JWTAuthScheme;
                        if (ctx.Request.Headers.ContainsKey("Authorization")) return JWTAuthScheme;
                        return CookieAuthScheme;
                    };
                })
                .AddJwtBearer(JWTAuthScheme, options =>
                {
                    // Configure JWT Bearer Auth to expect our security key
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //LifetimeValidator = (before, expires, token, param) =>
                        //{
                        //    return expires > DateTime.UtcNow;
                        //},
                        //ValidateAudience = false,
                        //ValidateIssuer = false,
                        //ValidateActor = false,
                        //ValidateLifetime = true,
                        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes("this one would be a really real secret")),
                        //ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["TokenAuthentication:Issuer"],
                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = Configuration["TokenAuthentication:Audience"],
                        // Validate the token expiry
                        ValidateLifetime = true,
                        // If you want to allow a certain amount of clock drift, set that here:
                        ClockSkew = TimeSpan.Zero,
                    };

                    // The JwtBearer scheme knows how to extract the token from the Authorization header
                    // but we will need to manually extract it from the query string in the case of requests to the hub
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            if (ctx.Request.Query.ContainsKey("access_token"))
                            {
                                ctx.Token = ctx.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

        }

        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
           
            if (username == "TEST" && password == "TEST123")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { }));
            }

            // Account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        #region Echo
        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        #endregion
    }


}
