using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using MinAPI.Model.Entity;
using MinAPI.Auth.Interfaces;
using MinAPI.Model.Dto;
using MinAPI.Model.DataModel.Interface;
using MinAPI.Model.DataModel;
using MinAPI.Auth;

namespace MinAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<HotelDb>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
            builder.Services.AddScoped<IHotelRepository, HotelRepository>();
            builder.Services.AddSingleton<ITokenService>(new TokenService());
            builder.Services.AddSingleton<IUserRepository>(new UserRepository());
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<HotelDb>();
                db.Database.EnsureCreated();
            }

            app.MapGet("/login", [AllowAnonymous] async (HttpContext context,
                ITokenService tokenService, IUserRepository userRepository) =>
            {
                UserModel userModel = new()
                {
                    UserName = context.Request.Query["username"],
                    Password = context.Request.Query["password"]
                };

                var userDto = userRepository.GetUser(userModel);
                if (userDto == null) return Results.Unauthorized();
                var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
                    builder.Configuration["Jwt:Issuer"], userDto);
                return Results.Ok(token);
            });

            app.MapGet("/hotels/{pageSize}/{pageNumber}", [Authorize] async (int pageSize, int pageNumber, IHotelRepository repository) =>
                Results.Ok(await repository.GetHotelsAsync(pageSize,pageNumber)))
                .Produces<List<Hotel>>(StatusCodes.Status200OK)
                .WithName("GetHotelsByPageAndCount")
                .WithTags("Getters");
            app.MapGet("/hotels", [Authorize] async (IHotelRepository repository) =>
               Results.Ok(await repository.GetHotelsAsync()))
               .Produces<List<Hotel>>(StatusCodes.Status200OK)
               .WithName("GetHotels")
               .WithTags("Getters");
            app.MapGet("/hotels/name/{query}", [Authorize]
            async (string query, IHotelRepository repository) =>
                    await repository.GetHotelWithRooms(query) is IEnumerable<RoomDto> hotels
                        ? Results.Ok(hotels)
                        : Results.NotFound(Array.Empty<Hotel>())
            ).Produces<List<Hotel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetHotelRooms")
            .WithTags("Getters")
            .ExcludeFromDescription();
            app.MapGet("/hotels/{id}/", [Authorize] async (int id, IHotelRepository repository) =>
                await repository.GetHotelAsync(id) is Hotel hotel
                ? Results.Ok(hotel)
                : Results.NotFound())
                .Produces<List<Hotel>>(StatusCodes.Status200OK)
                .WithName("GetHotelsById")
                .WithTags("Getters");


            app.MapPost("/hotels", [Authorize] async ([FromBody] Hotel hotel ,IHotelRepository repository) =>
            {
                await repository.InsertHotelAsync(hotel);
                await repository.SaveAsync();
                return Results.Created($"/hotels/{hotel.Id}", hotel);
            }).Accepts<Hotel>("application/json")
                .Produces<List<Hotel>>(StatusCodes.Status201Created)
                .WithName("CreateHotel")
                .WithTags("Creators");

            app.MapPut("/hotels", [Authorize] async ([FromBody] HotelDto hotel, IHotelRepository repository) =>
            {
                await repository.UpdateHotelAsync(hotel);
                await repository.SaveAsync();
                return Results.NoContent();
            }).Accepts<Hotel>("application/json")
            .WithName("UpdateHotel")
            .WithTags("Updaters");


            app.MapDelete("hotels/{id}", [Authorize] async (int id, IHotelRepository repository) =>
            {
                await repository.DeleteHotelAsync(id);
                await repository.SaveAsync();
                return Results.NoContent();
            })
                .WithName("DeleteHotel")
                .WithTags("Deleters");
            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
