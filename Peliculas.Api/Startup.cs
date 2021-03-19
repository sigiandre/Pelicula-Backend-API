using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Peliculas.Api.Data;
using Peliculas.Api.helpers;
using Peliculas.Api.PeliculasMapper;
using Peliculas.Api.Repository;
using Peliculas.Api.Repository.IRepository;

namespace Peliculas.Api
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
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            /*Agregar Dependencia de Token*/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAutoMapper(typeof(PeliculasMappers));

            //De aqui en adelante configuracion de documentacion de nuestra API
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiPeliculasCategorias", new OpenApiInfo()
                {
                    Title = "API Categorias Peliculas",
                    Version = "1",
                    Description = "Backend Peliculas",
                    Contact = new OpenApiContact()
                    {
                        Email = "andrew_programador@hotmail.com",
                        Name = " Sigi Andre Diaz Quiroz",
                        Url = new Uri("http://andrewprogramador.wixsite.com/cibernet")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Cibernet License",
                        Url = new Uri("https://github.com/sigiandre/Pelicula-Backend-API")
                    }
                });

                options.SwaggerDoc("ApiPeliculas", new OpenApiInfo()
                {
                    Title = "API Peliculas",
                    Version = "1",
                    Description = "Backend Peliculas",
                    Contact = new OpenApiContact()
                    {
                        Email = "andrew_programador@hotmail.com",
                        Name = " Sigi Andre Diaz Quiroz",
                        Url = new Uri("http://andrewprogramador.wixsite.com/cibernet")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Cibernet License",
                        Url = new Uri("https://github.com/sigiandre/Pelicula-Backend-API")
                    }
                });

                options.SwaggerDoc("ApiPeliculasUsuarios", new OpenApiInfo()
                {
                    Title = "API Usuarios Peliculas",
                    Version = "1",
                    Description = "Backend Peliculas",
                    Contact = new OpenApiContact()
                    {
                        Email = "andrew_programador@hotmail.com",
                        Name = " Sigi Andre Diaz Quiroz",
                        Url = new Uri("http://andrewprogramador.wixsite.com/cibernet")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Cibernet License",
                        Url = new Uri("https://github.com/sigiandre/Pelicula-Backend-API")
                    }
                });

                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                options.IncludeXmlComments(rutaApiComentarios);

                //Primero definir el esquema de seguridad
                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticacion JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });

            services.AddControllers();

            //Damos soporte para CORS
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            app.UseHttpsRedirection();

            //Linea para documentacion API
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/ApiPeliculasCategorias/swagger.json", "API Categorias Peliculas");
                options.SwaggerEndpoint("/swagger/ApiPeliculas/swagger.json", "API Peliculas");
                options.SwaggerEndpoint("/swagger/ApiPeliculasUsuarios/swagger.json", "API Usuarios Peliculas");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            /*Estos dos son para la autenticacion y autorizacion*/
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Damos soporte para CORS
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }
    }
}
