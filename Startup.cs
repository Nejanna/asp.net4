using lab4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace lab4
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("configMyfile.json")
               .Build();

            var config2 = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("config2User.json")
               .Build();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/Library", async context =>
                {
                    await context.Response.WriteAsync("Welcome!");
                });

                endpoints.MapGet("/Library/Books", async context =>
                {
                    var books = config.GetSection("Books").Get<Dictionary<string, Book>>();

                    StringBuilder response = new StringBuilder();
                    foreach (var book in books.Values)
                    {
                        response.AppendLine(book.Title + "  " + book.Author + "  " + book.Year);
                    }
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(response.ToString());
                });

                endpoints.MapGet("/Library/Profile/{id?}", async context =>
                {
                    var id = context.Request.RouteValues["id"]?.ToString();
                    // отримання значення з урл адреси

                    var users = config2.GetSection("Users").Get<Dictionary<string, User>>();
                    var users2 = config2.GetSection("myuser").Get<Dictionary<string, User>>();
                    // Перевірка, чи введено дійсний id (від 0 до 5)
                    if (int.TryParse(id, out var userId) && userId > 0 && userId <= 5)
                    {
                        // Отримання інформації про користувача за вказаним id
                        if (users.ContainsKey(userId.ToString()))
                        {
                            var user = users[userId.ToString()];
                            await context.Response.WriteAsync($"Name: {user.Name}, Gender: {user.Gender}, Email: {user.Email}");
                        }
                        else
                        {
                            await context.Response.WriteAsync("Користувача не знайдено.");
                        }
                    }
                    if (string.IsNullOrEmpty(id))
                    {
                        // Якщо id порожній, вивести інформацію про самого користувача
                        var myuser = config2.GetSection("myuser").Get<User>();
                        await context.Response.WriteAsync($"Name: {myuser.Name}, Gender: {myuser.Gender}, Email: {myuser.Email}");
                    }
                });
            });
        }
    }
}




