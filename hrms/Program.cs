using CloudinaryDotNet;
using hrms.CustomException;
using hrms.Data;
using hrms.Repository;
using hrms.Repository.impl;
using hrms.Service;
using hrms.Service.impl;
using hrms.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.
    Configuration.
    GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("\"Connection string 'DefaultConnection' not found.\"");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilterAttribute>();
});

builder.Services.Configure<ApiBehaviorOptions>(option =>
{
    option.InvalidModelStateResponseFactory = context =>
    {
        var error = context.ModelState
        .Where(e => e.Value.Errors.Count > 0)
        .Select(e => new
        {
            field = e.Key,
            error = e.Value.Errors.Select(x => x.ErrorMessage)
        });
        return new BadRequestObjectResult(new
        {
            error = "Validation Failed",
            details = error
        });
    };
});
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("WeeklyJob");
    q.AddJob<WeeklyJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("WeeklyJob-trigger")
        .StartNow()
        //.WithCronSchedule("0 0 0 * * ?")
    //.WithCronSchedule("0 0 0 ? * MON")
    );
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITravelRepository, TravelRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IDepartmentRepository,DepartmentRepository>();
builder.Services.AddScoped<INotificationRepository,NotificationRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobReviewerRepository, JobReviewerRepository>();
builder.Services.AddScoped<IJobReferralRepository, JobReferralRepository>();
builder.Services.AddScoped<IJobShareRepository, JobShareRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameBookingRepository, GameBookingRepository>();
builder.Services.AddScoped<IUserGameRepository, UserGameRepository>();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITravelService, TravelService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IExpenseService,ExpenseService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobReviewerService, JobReviewerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameSlotService, GameSlotService>();
builder.Services.AddScoped<ISlotBookingService, SlotBookingService>();

builder.Services.AddHostedService<SlotPriorotyCornJob>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret
    );
    return new Cloudinary(account);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt_token"];
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"])
        )
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("MyAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
