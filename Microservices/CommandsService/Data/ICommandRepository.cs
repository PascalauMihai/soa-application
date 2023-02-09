using System.Collections.Generic;
using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepository
    {
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlaforms();
        void CreatePlatform(Platform platform);
        void UpdatePlatform(Platform platform);
        void DeletePlatform(int id);
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);

        // Commands
        IEnumerable<Command> GetCommandsForPlatform(int platformId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
        void UpdateCommand(Command command);
        bool DeleteCommand(int platformId, int commandId);
        void DeleteAllCommandsForPlatform(int platformId);
    }
}