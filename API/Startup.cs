using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using StudentHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;

namespace StudentHub
{
	public class Startup
	{
		private IConfiguration Configuration { get; }

		private readonly string corsPolicy = "AllowOrigins";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
			});

			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
			});

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
							.GetBytes(Configuration.GetConnectionString("Token"))),
						ValidateAudience = false,
						ValidateIssuer = false
					};
				});

			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentHub API", Version = "v1" });
				options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
				{
					Description = "Bearer header authorization scheme",
					In = ParameterLocation.Header,
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey
				});
				options.OperationFilter<SecurityRequirementsOperationFilter>();
			});

			services.AddCors(options =>
			{
				options.AddPolicy(name: corsPolicy,
								  builder =>
								  {
									  builder.AllowAnyHeader()
											 .AllowAnyOrigin()
											 .AllowAnyMethod();
								  });
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentHub API V1");
			});

			app.UseStaticFiles();
			app.UseRouting();
			app.UseCors(corsPolicy);

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
