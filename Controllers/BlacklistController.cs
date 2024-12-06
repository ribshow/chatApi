using chatApi.Models;
using chatApi.Services;
using chatApi.Responses;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Amazon.Runtime.SharedInterfaces;

namespace chatApi.Controllers
{
    public class BlacklistController : Controller
    {
        private readonly ContextMongoDb _database;

        private readonly ChatService _chatService;
        private readonly ChatTechService _chatTechService;
        private readonly ChatGeekService _chatGeekService;
        private readonly ChatSciService _chatSciService;

        public BlacklistController(ContextMongoDb database, ChatService chatService, ChatTechService chatTechService, ChatGeekService chatGeekService, ChatSciService chatSciService)
        {
            _database = database;
            _chatService = chatService;
            _chatTechService = chatTechService;
            _chatGeekService = chatGeekService;
            _chatSciService = chatSciService;
        }

        [HttpGet("/report/all")]
        public async Task<IActionResult> Index()
        {
            var reports = await _database.Blacklist.Find(report => true).ToListAsync();

            return Ok(new {reports});
        }

        /// <summary>
        /// Report Message from ChatHub
        /// </summary>
        /// <param name="id"></param>
        /// <param name="origin"></param>
        /// <returns>Successful report</returns>
        /// <response code="200">Successful report</response>
        /// <responde code="404">Message not found</responde>
        [HttpPost("report/chathub")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReportMessage([FromForm] string id, [FromForm]string origin)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            // Verificando a origem da mensagem e buscando ela na coleção apropriada
            if(origin == "ChatHub")
            {
                var chat = await _chatService.GetAsync(id);
                if(chat == null)
                {
                    return NotFound(new {
                        status = 404,
                        error = "Chat message not found!"
                    });
                }
                Blacklist blacklist = new(chat.Id, chat.Fullname, chat.Nickname, chat.Message, chat.date, origin);
                await _database.Blacklist.InsertOneAsync(blacklist);
            }

            // Verificando a origem da mensagem e buscando ela na coleção apropriada
            if (origin == "ChatTech")
            {
                var chat = await _chatTechService.GetAsync(id);
                if (chat == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        error = "Chat message not found!"
                    });
                }
                Blacklist blacklist = new(chat.Id, chat.Fullname, chat.Nickname, chat.Message, chat.date, origin);
                await _database.Blacklist.InsertOneAsync(blacklist);
            }

            // Verificando a origem da mensagem e buscando ela na coleção apropriada
            if (origin == "ChatGeek")
            {
                var chat = await _chatGeekService.GetAsync(id);
                if (chat == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        error = "Chat message not found!"
                    });
                }
                Blacklist blacklist = new(chat.Id, chat.Fullname, chat.Nickname, chat.Message, chat.date, origin);
                await _database.Blacklist.InsertOneAsync(blacklist);
            }

            // Verificando a origem da mensagem e buscando ela na coleção apropriada
            if (origin == "ChatSci")
            {
                var chat = await _chatSciService.GetAsync(id);
                if (chat == null)
                {
                    return NotFound(new
                    {
                        status = 404,
                        error = "Chat message not found!"
                    });
                }
                Blacklist blacklist = new(chat.Id, chat.Fullname, chat.Nickname, chat.Message, chat.date, origin);
                await _database.Blacklist.InsertOneAsync(blacklist);
            }
             


            return Ok(new Response { Message = "Successful report!" });
        }
        /// <summary>
        /// Delete a message reported
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Report deleted successfully</returns>
        /// <response code="200">Report deleted successfully</response>
        /// <response code="404">Message not found</response>
        [HttpDelete("report/{id:length(24)}")]

        public async Task<IActionResult> DeleteReport(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var report = await _database.Blacklist.Find(report => report.Id == id).FirstOrDefaultAsync();

            if(report == null)
            {
                return NotFound(new
                {
                    status = 404,
                    error = "Message not found!",
                    details = "The message ID provided is null or empty"
                });
            }

            var value = await _database.Blacklist.FindOneAndDeleteAsync(report => report.Id == id);

            return Ok(new { Message = "Report deleted successfully", report });
        }
    }
}
