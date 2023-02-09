using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDTO>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");

            var platformItems = _platformRepository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems));
        }

        [HttpGet]
        [Route("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDTO> GetPlatformById(int id)
        {
            var platformItem = _platformRepository.GetPlatformById(id);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDTO>(platformItem));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("{id}", Name = "UpdatePlatformById")]
        public ActionResult<PlatformReadDTO> UpdatePlatformById(int id, PlatformCreateDTO platformCreateDTO)
        {
            var platformItem = _platformRepository.GetPlatformById(id);
            var platformModel = _mapper.Map<Platform>(platformCreateDTO);
            platformModel.Id = id;

            if (platformItem != null && platformModel != null)
            {
                var platformUpdated = _platformRepository.UpdatePlatform(platformModel);
                _platformRepository.SaveChanges();
                
                var platformReadDto = _mapper.Map<PlatformReadDTO>(platformUpdated);

                // Send Async Message
                try
                {
                    var platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(platformReadDto);
                    platformPublishedDTO.Event = "Platform_Updated";

                    _messageBusClient.PublishNewPlatform(platformPublishedDTO);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
                }

                return CreatedAtRoute(nameof(UpdatePlatformById), new {Id = platformReadDto.Id}, platformReadDto);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id}", Name = "DeletePlatformById")]
        public ActionResult<PlatformReadDTO> DeletePlatformById(int id)
        {
            var platform = _platformRepository.GetPlatformById(id);
            var deleted = _platformRepository.DeletePlatform(id);
            _platformRepository.SaveChanges();

            if (deleted && platform != null)
            {
                var platformReadDTO = _mapper.Map<PlatformReadDTO>(platform);
                // Send Async Message
                try
                {
                    var platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(platformReadDTO);
                    platformPublishedDTO.Event = "Platform_Deleted";

                    _messageBusClient.PublishNewPlatform(platformPublishedDTO);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
                }

                return Ok();
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDTO>> CreatePlatform(PlatformCreateDTO platformCreateDTO)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDTO);
            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDTO>(platformModel);
            
            // Send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // Send Async Message
            try
            {
                var platformPublishedDTO = _mapper.Map<PlatformPublishedDTO>(platformReadDto);
                platformPublishedDTO.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishedDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);
        }
    }
}