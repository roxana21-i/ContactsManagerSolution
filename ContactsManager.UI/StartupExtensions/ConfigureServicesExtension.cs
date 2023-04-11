using ContactsManager.Core.Domain.IdentityEntities;
using CRUD_Application.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace CRUD_Application
{
	public static class ConfigureServicesExtension
	{
		public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllersWithViews(options =>
			{
				//options.Filters.Add<ResponseHeaderActionFilter>(5); //can't add parameters here
				var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
				options.Filters.Add(new ResponseHeaderActionFilter(logger)
				{
					Key = "X-My-Global-Key",
					Value = "My-Global-Value",
					Order = 2
				});
				options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
			});

			services.AddScoped<ICountriesRepository, CountriesRepository>();
			services.AddScoped<IPersonsRepository, PersonsRepository>();
			services.AddScoped<ICountriesAdderService, CountriesAdderService>();
			services.AddScoped<ICountriesGetterService, CountriesGetterService>();
			services.AddScoped<ICountriesUploaderService, CountriesUploaderService>();
			//services.AddScoped<IPersonsGetterService, PersonsGetterService_ExcelFieldsUpdate>();
			services.AddScoped<IPersonsGetterService, PersonsGetterServiceChild>();
			services.AddScoped<PersonsGetterService, PersonsGetterService>();
			services.AddScoped<IPersonsAdderService, PersonsAdderService>();
			services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
			services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
			services.AddScoped<IPersonsSorterService, PersonsSorterService>();
			//so you can use [ServiceFilter]
			services.AddTransient<PersonsListActionFilter>();
			services.AddTransient<ResponseHeaderActionFilter>();

			//add DbContext
			services.AddDbContext<ApplicationDbContext>(options =>
			//options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
			{
					options.UseMySql(configuration["ConnectionStrings:AZURE_MYSQL_CONNECTIONSTRING"],
							ServerVersion.AutoDetect(
							  configuration["ConnectionStrings:AZURE_MYSQL_CONNECTIONSTRING"]
							)
						  );
				}
			);

			//enable Identity
			services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
			{
				options.Password.RequiredLength = 5; //default is 6
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireDigit = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = true;
				options.Password.RequiredUniqueChars = 3;
			})
				.AddEntityFrameworkStores<ApplicationDbContext>() //aplication level
				.AddDefaultTokenProviders()
				//repository level
				.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
				.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

			services.AddAuthorization(options =>
			{
				//for all the action methods
				options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

				options.AddPolicy("NotAuthenticated", policy =>
				{
					policy.RequireAssertion(context =>
					{
						return !context.User.Identity.IsAuthenticated;
					});
				});
			});

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/Account/Login";
			});

			services.AddHttpLogging(options =>
			{
				options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties |
					Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
			});

			return services;
		}
	}
}
