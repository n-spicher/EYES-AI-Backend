using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Pdf_Processor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore.Models;
using Pdf_Processor.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Pdf_Processor.Models;

namespace Pdf_Processor
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
            var environment = Configuration.GetValue<string>("Environment");

            var connectionString = "";
            if (environment == "development")
            {
                connectionString = Configuration.GetConnectionString("pdfprocessor");
            }
            else
            {
                connectionString = Configuration.GetConnectionString("pdfprocessor");
            }
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connectionString, opt =>
              {
                  opt.EnableRetryOnFailure();
              })
            );

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.SignIn.RequireConfirmedAccount = false;
                config.User.RequireUniqueEmail = true;
                config.Tokens.AuthenticatorIssuer = "JWT";

                //for Email confirmation
                config.SignIn.RequireConfirmedEmail = true;
                //for email confirmation token valid time
                config.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
                //for reset Password token valid time
                config.Tokens.PasswordResetTokenProvider = "resetpassword";

            }).AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation")
            .AddTokenProvider<ResetPasswordTokenProvider<ApplicationUser>>("resetpassword")
            .AddEntityFrameworkStores<ApplicationDbContext>();


            //********jwt***********
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
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(ProjectConstants.AuthKey)),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });

            services.AddControllers()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }).AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddCors(options =>
            {
                string[] corsList = Configuration.GetSection("AllowedOrigins").Get<string[]>();
                options.AddDefaultPolicy(
                builder =>
                {
                    //builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    builder.WithOrigins(corsList).AllowCredentials().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pdf_Processor", Version = "v1" });
            });

            services.AddPdfProcessorServicesSettings();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/exception");

            //if (env.IsDevelopment())
            //{
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pdf_Processor v1"));
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            

            app.UseCors();

            app.UseSession();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
