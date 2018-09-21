using ApiAccountService.Data;
using ApiAccountService.Models;
using ApiAccountService.Repository;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Consumers;
using Contracts.Commands;
using Contracts.Models;
using MassTransit;
using MassTransit.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace ApiAccountService
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri($"http://{Environment.GetEnvironmentVariable("ES_HOST")}:{Environment.GetEnvironmentVariable("ES_PORT")}/"))
                {
                    AutoRegisterTemplate = true,
                })
            .CreateLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        public IContainer ApplicationContainer { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = $"{Environment.GetEnvironmentVariable("IS_SERVER")}";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "api";
                    options.ApiSecret = "secret";
                });

            services.AddDataProtection()
                .SetApplicationName("api-account")
                .PersistKeysToFileSystem(new DirectoryInfo(@"/var/dpkeys/"));

            services.AddIdentityWithMongoStoresUsingCustomTypes<ApplicationUser, ApplicationUserRole>($"mongodb://{Environment.GetEnvironmentVariable("MONGODB_USERNAME")}:{Environment.GetEnvironmentVariable("MONGODB_PASSWORD")}@{Environment.GetEnvironmentVariable("USER_MONGODB_HOST")}:{Environment.GetEnvironmentVariable("USER_MONGODB_PORT")}/{Environment.GetEnvironmentVariable("USER_MONGODB_DATABASE_NAME")}")
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //unique email
                options.User.RequireUniqueEmail = true;
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 5;
            });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod().WithExposedHeaders("Authorization");
                });
            });

            services.Configure<ElasticSearchSettings>(options =>
            {
                options.Host = Environment.GetEnvironmentVariable("ES_HOST");
                options.Port = Environment.GetEnvironmentVariable("ES_PORT");
                options.DatabaseName = Environment.GetEnvironmentVariable("USER_MONGODB_DATABASE_NAME");
            });

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = $"{Environment.GetEnvironmentVariable("REDIS_HOST")}:{Environment.GetEnvironmentVariable("REDIS_PORT")}";
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ApiAccountService", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "ApiAccountService.xml");
                c.IncludeXmlComments(filePath);
            });

            var builder = new ContainerBuilder();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<AccessRightRepository>().As<IAccessRightRepository>();

            builder.RegisterType<ApplicationDbContext>().WithParameter("connectionString", $"mongodb://{Environment.GetEnvironmentVariable("MONGODB_USERNAME")}:{Environment.GetEnvironmentVariable("MONGODB_PASSWORD")}@{Environment.GetEnvironmentVariable("USER_MONGODB_HOST")}:{Environment.GetEnvironmentVariable("USER_MONGODB_PORT")}")
                   .WithParameter("database", $"{Environment.GetEnvironmentVariable("USER_MONGODB_DATABASE_NAME")}");

            //mass transit endpoint
            var timeout = TimeSpan.FromSeconds(30);
            builder.RegisterType<CreateAccessRightConsumer>();
            builder.Register(c => new MessageRequestClient<ICreateAccessRight, UserPermissions>(c.Resolve<IBus>(), new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}/create_access_right"), timeout))
                .As<IRequestClient<ICreateAccessRight, UserPermissions>>()
                .SingleInstance();

            services.AddScoped<DeleteAccessRightConsumer>();
            builder.Register(c => new MessageRequestClient<IDeleteAccessRight, UserPermissions>(c.Resolve<IBus>(), new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}/delete_access_right"), timeout))
                .As<IRequestClient<IDeleteAccessRight, UserPermissions>>()
                .SingleInstance();


            builder.Register(context =>
            {
                return Bus.Factory.CreateUsingRabbitMq(sbc =>
                {
                    var host = sbc.Host(new Uri($"rabbitmq://{Environment.GetEnvironmentVariable("RABBITMQ_HOST")}/"), h =>
                    {
                        h.Username(Environment.GetEnvironmentVariable("RABBITMQ_USERNAME"));
                        h.Password(Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD"));
                    });

                    sbc.ReceiveEndpoint(host, "create_access_right", ep =>
                    {
                        ep.Consumer<CreateAccessRightConsumer>(context);
                    });

                    sbc.ReceiveEndpoint(host, "delete_access_right", ep =>
                    {
                        ep.Consumer<DeleteAccessRightConsumer>(context);
                    });
                });
            })
            .As<IBus>()
            .As<IBusControl>()
            .As<IPublishEndpoint>()
            .SingleInstance();
            builder.Populate(services);
            ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiAccountService V1");
                });
            }
            loggerFactory.AddSerilog();

            IPHostEntry local = Dns.GetHostEntry(Environment.GetEnvironmentVariable("LOADBALANCER"));
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
                KnownProxies = { local.AddressList[0] }
            });
            app.Use(async (ctx, next) =>
            {
                using (LogContext.PushProperty("IPAddress", ctx.Connection.RemoteIpAddress))
                using (LogContext.PushProperty("UserName", ctx.User?.Claims?.FirstOrDefault(_ => _.Type == "userName")?.Value))
                {
                    await next();
                }
            });

            app.UseCors("default");
            app.UseAuthentication();
            app.UseMvc();
            //resolve the bus from the container
            var bus = ApplicationContainer.Resolve<IBusControl>();
            //start the bus
            var busHandle = TaskUtil.Await(() => bus.StartAsync());

            appLifetime.ApplicationStopped.Register(() => { busHandle.Stop(); ApplicationContainer.Dispose(); });
        }
    }
}
