﻿using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditMessageController : ControllerBase
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        public EditMessageController(ILogger<EditMessageController> logger,
            ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return Ok(new BaseResponse
                {
                    Message = "Edit message request completed successfully!"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch(AggregateNotFoundException age)
            {
                _logger.Log(LogLevel.Warning, age, "Could not retrieve aggregate, client passed an incorrect post id tergeting the aggregate !");
                return BadRequest(new BaseResponse
                {
                    Message = age.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit the message of a new post";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new NewPostResponse
                    {
                        Id = id,
                        Message = SAFE_ERROR_MESSAGE
                    });
            }
           
        }
    }
}
