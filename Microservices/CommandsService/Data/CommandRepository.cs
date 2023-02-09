using System;
using System.Collections.Generic;
using System.Linq;
using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            _context.Platforms.Add(platform);
        }

        public void DeleteAllCommandsForPlatform(int platformId)
        {
            var commandsToDelete = _context.Commands
                .Where(c => c.PlatformId == platformId)
                .ToList();
            
            if (commandsToDelete.Any())
            {
                _context.Commands.RemoveRange(commandsToDelete);
            }        
        }

        public bool DeleteCommand(int platformId, int commandId)
        {
             var commandToDelete = _context.Commands
                .Where(c => c.Id == commandId && c.PlatformId == platformId)
                .FirstOrDefault();
            
            if (commandToDelete != null)
            {
                _context.Commands.Remove(commandToDelete);
                return true;
            }
            return false;
        }
            
        public void DeletePlatform(int id)
        {
            var platformToDelete = _context.Platforms
                .Where(p => p.Id == id)
                .FirstOrDefault();
            
            if (platformToDelete != null)
            {
                _context.Platforms.Remove(platformToDelete);
            }
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return _context.Platforms.Any(p => p.ExternalID == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlaforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _context.Commands
                .Where(c => c.PlatformId == platformId && c.Id == commandId)
                .FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() > 0);
        }

        public void UpdateCommand(Command command)
        {
            var commandToUpdate = _context.Commands
                .Where(c => c.PlatformId == command.PlatformId && c.Id == command.Id)
                .FirstOrDefault();
            
            if (commandToUpdate == null)
            {
                throw new ArgumentNullException(nameof(commandToUpdate));
            }

            commandToUpdate.CommandLine = command.CommandLine;
            commandToUpdate.HowTo = command.HowTo;
            commandToUpdate.PlatformId = command.PlatformId;

            _context.Commands.Update(commandToUpdate);        

        }

        public void UpdatePlatform(Platform platform)
        {
             var platformToUpdate = _context.Platforms
                .Where(p => p.Id == platform.Id)
                .FirstOrDefault();

            if (platformToUpdate == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            platformToUpdate.Name = platform.Name;
            platformToUpdate.ExternalID = platform.ExternalID;
            
            _context.Platforms.Update(platformToUpdate);        
        }
    }
}