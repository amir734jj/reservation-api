using System;
using System.Text;
using AgileObjects.AgileMapper;
using Api.Attributes;
using Api.Configs;
using Dal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using WebMarkupMin.AspNetCore2;
using static Api.Utilities.ConnectionStringUtility;

namespace Api
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        // ReSharper disable once NotAccessedField.Local
        private readonly IHostingEnvironment _env;

        private Container _container;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMiniProfiler(opt =>
            {
                opt.ShouldProfile = _ => true;
                opt.ShowControls = true;
                opt.StackMaxLength = short.MaxValue;
                opt.PopupStartHidden = false;
                opt.PopupShowTrivial = true;
                opt.PopupShowTimeWithChildren = true;
            });

            services.AddLogging();

            // If environment is localhost, then enable CORS policy, otherwise no cross-origin access
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddRouting(options => { options.LowercaseUrls = true; });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(50);
                options.Cookie.HttpOnly = true;
            });

            services.AddWebMarkupMin(opt =>
                {
                    opt.AllowMinificationInDevelopmentEnvironment = true;
                    opt.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification()
                .AddHttpCompression();

            services.AddDbContext<EntityDbContext>(opt =>
            {
                switch (_env.EnvironmentName)
                {
                    // If Development then use Sqlite
                    case "Development":
                        opt.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:SQLite"));
                        break;
                    case "Production":
                        // Database Url
                        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                        // Create postgres specific connection string
                        var connectionString = ConnectionStringUrlToResource(databaseUrl);

                        // Initialize postgres
                        opt.UseNpgsql(connectionString);
                        break;
                    default:
                        throw new Exception("Invalid Environment!");
                }
            });

            // Configure Entity Framework Identity for Auth
            services.AddIdentity<User, IdentityRole>(x => { x.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddDefaultTokenProviders();

            var jwtSetting = new JwtSettings();

            var jwtConfigSection = _configuration.GetSection("JwtSettings");

            // Populate the JwtSettings object
            jwtConfigSection.Bind(jwtSetting);

            services.Configure<JwtSettings>(jwtConfigSection);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(x =>
            {
                x.Cookie.MaxAge = TimeSpan.FromMinutes(60);
            });

            // Add framework services
            services
                .AddMvc(opt =>
                {
                    opt.Filters.Add<ExceptionFilterAttributeImpl>();

                    if (_env.IsDevelopment())
                    {
                        opt.Filters.Add<AllowAnonymousFilter>();
                    }
                })
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Info
                {
                    Title = "reservation-api", Version = "v1",
                    Description = "Reservation API"
                });

                // In swagger, use the name of Enum instead of their integer value
                opt.DescribeAllEnumsAsStrings();
            });

            services.AddSingleton(Mapper.CreateNew());

            _container = new Container(opt =>
            {
                // Also exposes Lamar specific registrations
                // and functionality
                opt.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("Dal");
                    _.Assembly("Logic");
                    _.WithDefaultConventions();
                });

                opt.Populate(services);

                opt.For<IConfigurationRoot>().Use(_ => _configuration);

                // Transient means fresh instance for each request
                opt.For<EntityDbContext>().Transient();
            });

            // Validate IoC container
            if (_env.IsDevelopment())
            {
                _container.AssertConfigurationIsValid();
            }

            return _container.GetInstance<IServiceProvider>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // ...existing configuration...
            app.UseMiniProfiler();

            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            // Use wwwroot folder as default static path
            app.UseDefaultFiles();

            // Serve static files
            app.UseStaticFiles();

            // Not necessary for this workshop but useful when running behind kubernetes
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
                // This is particularly important so that HttpContet.Request.Scheme will be correct behind a SSL terminating proxy
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseCookiePolicy();

            app.UseSession();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(opt => { opt.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
        }
    }
}