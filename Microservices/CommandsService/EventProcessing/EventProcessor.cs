using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory,
            IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                case EventType.PlatformUpdated:
                    UpdatePlatform(message);
                    break;
                case EventType.PlatformDeleted:
                    DeletePlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage);

            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                case "Platform_Updated":
                    Console.WriteLine("--> Platform Updated Event detected");
                    return EventType.PlatformUpdated;
                case "Platform_Deleted":
                    Console.WriteLine("--> Platform Deleted Event detected");
                    return EventType.PlatformDeleted;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDTO);

                    if (!repository.ExternalPlatformExists(platform.ExternalID))
                    {
                        repository.CreatePlatform(platform);
                        repository.SaveChanges();
                        Console.WriteLine("--> Platform added!");

                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");
                }
            }
        }
        private void UpdatePlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDTO);

                    if (repository.ExternalPlatformExists(platform.ExternalID))
                    {
                        repository.UpdatePlatform(platform);
                        repository.SaveChanges();
                        Console.WriteLine("--> Platform updated!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform cannot be updated: it does not exist...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not update Platform to DB: {ex.Message}");
                }
            }
        }

         private void DeletePlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDTO);

                    if (repository.ExternalPlatformExists(platform.ExternalID))
                    {
                        repository.DeleteAllCommandsForPlatform(platform.Id);
                        repository.DeletePlatform(platform.Id);
                        repository.SaveChanges();
                        Console.WriteLine("--> Platform deleted!");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform cannot be deleted: it does not exist...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not delete Platform to DB: {ex.Message}");
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        PlatformUpdated,
        PlatformDeleted,
        Undetermined
    }
}