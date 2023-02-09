using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;
using System.Collections.Generic;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient.ReturnAllPlatforms();

                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd, 
                    serviceScope.ServiceProvider.GetService<ICommandRepository>(), platforms);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd, 
            ICommandRepository repository, IEnumerable<Platform> platforms)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations");
                try
                {
                    
                context.Database.Migrate();
            
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--/ Could not load migrations: {ex.Message}");
                }
            }

            foreach (var plat in platforms)
            {
                if (!repository.ExternalPlatformExists(plat.ExternalID))
                {
                    repository.CreatePlatform(plat);
                }
            }
                
            context.SaveChanges();

           /* if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Sending Platforms Data...");

                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net"},
                    new Platform() {Name="SQL Server Express"},
                    new Platform() {Name="Kubernetes"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have Platforms data");
            }

            if (!context.Commands.Any())
            {

                Console.WriteLine("--> Sending Commands Data...");

                 context.Commands.AddRange(
                    new Command() {HowTo="Push a docker container to hub", CommandLine="docker push <name_container>", PlatformId=3}
                );
                                
                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("--> We already have Commands data");

            }*/
        }
    }
}