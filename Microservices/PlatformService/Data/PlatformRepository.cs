using System;
using System.Collections.Generic;
using System.Linq;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _appContext;

        public PlatformRepository(AppDbContext appContext)
        {
            _appContext = appContext;    
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
            
            _appContext.Platforms.Add(platform);
        }

        public bool DeletePlatform(int id)
        {
            var platform = _appContext.Platforms.FirstOrDefault(p => p.Id == id);
            Console.WriteLine($"--> Deleting {platform}");
            if (platform != null)
            {
                _appContext.Platforms.Remove(platform);
                return true;
            }
            return false;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _appContext.Platforms.ToList();
        }

        public Platform GetPlatformById(int id)
        {
            return _appContext.Platforms.FirstOrDefault(platform => platform.Id == id);
        }

        public bool SaveChanges()
        {
            return (_appContext.SaveChanges() >= 0);
        }

        public Platform UpdatePlatform(Platform platform)
        {
            var platformToUpdate = _appContext.Platforms.FirstOrDefault(p => p.Id == platform.Id);
            if (platformToUpdate == null)
            {
                throw new ArgumentNullException(nameof(platformToUpdate));
            }

            platformToUpdate.Name = platform.Name;
            platformToUpdate.Publisher = platform.Publisher;
            platformToUpdate.Cost = platform.Cost;
            
            _appContext.Platforms.Update(platformToUpdate);

            return platformToUpdate;
        }
    }
}