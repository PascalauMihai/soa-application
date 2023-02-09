using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.DTOs;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDTO>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commands = _repository.GetCommandsForPlatform(platformId);
        
            return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDTO> GetCommandForPlatform(int platformId, int commandId)
        {
             Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _repository.GetCommand(platformId, commandId);
            
            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDTO>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDTO> CreateCommandForPlatform(int platformId, CommandCreateDTO commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDto);

            _repository.CreateCommand(platformId, command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDTO>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }

        [HttpPost("{commandId}", Name = "UpdateCommandForPlatform")]
        public ActionResult<CommandReadDTO> UpdateCommandForPlatform(int platformId, int commandId, CommandCreateDTO commandDto)
        {
            Console.WriteLine($"--> Hit UpdateCommandForPlatform: {platformId}");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(commandDto);
            command.Id = commandId;
            command.PlatformId = platformId;

            _repository.UpdateCommand(command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDTO>(command);

            return CreatedAtRoute(nameof(UpdateCommandForPlatform), 
                new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }


        [HttpDelete("{commandId}", Name = "DeleteCommand")]
        public ActionResult<CommandReadDTO> DeleteCommand(int platformId, int commandId)
        {
             Console.WriteLine($"--> Hit DeleteCommand: {platformId} / {commandId}");
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var deleted = _repository.DeleteCommand(platformId, commandId);
            _repository.SaveChanges();

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}