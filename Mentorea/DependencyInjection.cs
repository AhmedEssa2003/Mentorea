using FluentValidation.AspNetCore;
using Hangfire;
using MapsterMapper;
using Mentorea.Authentication;
using Mentorea.Errors;
using Mentorea.Health;
using Mentorea.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace Mentorea
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerService();
            services.AddControllerAndCorsService();
            services.AddFluentValidationService();
            services.AddMapsterService();
            services.AddServices(configuration);
            services.AddDbContextService(configuration);
            services.AddHangfireSevices(configuration);
            services.AddJWTService(configuration);
            services.AddSignalR();
            services.AddRateLimiterService();
            services.AddDistributedMemoryCache();


            return services;
        }
        private static IServiceCollection AddSwaggerService(this IServiceCollection services)
        {

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
        private static IServiceCollection AddControllerAndCorsService(this IServiceCollection services)
        {

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
            services.AddControllers();
            return services;
        }
        private static IServiceCollection AddFluentValidationService(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
        private static IServiceCollection AddMapsterService(this IServiceCollection services)
        {
            var MapConfig = TypeAdapterConfig.GlobalSettings;
            MapConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(MapConfig));
            return services;
        }
        private static IServiceCollection AddHangfireSevices(this IServiceCollection services, IConfiguration Configuration)
        {
            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"))
            );

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            return services;
        }
        private static IServiceCollection AddRateLimiterService(this IServiceCollection services)
        {

            services.AddRateLimiter(option =>
            {
                option.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                option.AddConcurrencyLimiter("default", builder =>
                {
                    builder.QueueLimit = 100;
                    builder.PermitLimit = 300;
                    builder.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
            return services;
        }
        private static IServiceCollection AddDbContextService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MentoreaDbContext>(optionsBuilder => optionsBuilder
             .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<MentoreaDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });


            return services;
        }
        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailSender, MailService>();
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IMentorAvailabilityService, MentorAvailabilityService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ISpecializationService, SpecializationService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IDistributedCacheService, DistributedCacheService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ICardService, CardService>();


            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddHttpContextAccessor();
            services.AddHttpClient<IFcmService, FcmService>();
            services.AddExceptionHandler<GlobbalExceptionHandler>();
            services.AddProblemDetails();
            services.AddHealthChecks()
                .AddSqlServer(name: "Database", connectionString: configuration.GetConnectionString("DefaultConnection")!)
                .AddHangfire(option => option.MinimumAvailableServers = 1)
                .AddCheck<MailProviderHealthCheck>(name: "Mail Provider");

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            return services;
        }
        private static IServiceCollection AddJWTService(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.SaveToken = true;

                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,

                    ValidateAudience = true,

                    ValidateIssuer = true,

                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(configuration.GetValue<string>("Jwt:Key")!)),

                    ValidIssuer = configuration.GetValue<string>("Jwt:Issuer")!,

                    ValidAudience = configuration.GetValue<string>("Jwt:Audience")!,
                };
                option.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chathub", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            var requireAuthPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            services.AddAuthorizationBuilder().SetDefaultPolicy(requireAuthPolicy);

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

            return services;

            
        }

    }
}
