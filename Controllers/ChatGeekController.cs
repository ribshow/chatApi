using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using chatApi.Models;
using chatApi.Services;
using MongoDB.Driver;

namespace chatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatGeekController : ControllerBase
    {
        private readonly ChatGeekService _chatService;

        private readonly IHubContext<Hubs.ChatGeek> _hubContext;

        private readonly ILogger<ChatGeekController> _logger;

        public ChatGeekController(ILogger<ChatGeekController> logger, IHubContext<Hubs.ChatGeek> hubContext, ChatGeekService chatService)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Returns all messages saved
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /chatGeek
        /// </remarks>
        /// <response code="200">All messages were returned</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<Models.ChatGeek>> Index()
        {
            var chats = await _chatService.GetAsync();
            //var chats = await _chatService.GetAsync();
            foreach (Models.ChatGeek item in chats)
            {
                item.date = TimeZoneConfig.ConvertToSaoPauloTime(item.date);
            }

            return chats;

        }


        /// <summary>
        /// Return a specific message from chat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /chatGeek/id
        ///     {
        ///         "user": John Snow,
        ///         "nickname": johnsnowzin,
        ///         "message": Hello World!,
        ///         "date": 05/11/2024 - 22:50:44
        ///     }
        /// </remarks>
        /// <response code="200">Chat returned with successfully</response>
        /// <response code="404">Chat not found</response>
        /// <response code="500">Id format invalid</response>
        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Details(string id)
        {
            //var id = "671459283c7d80a0b4e78331";
            var chat = await _chatService.GetAsync(id);

            if (chat == null)
            {
                return NotFound("Chat não encontrado.");
            }

            if (chat.date == DateTime.MinValue)
            {
                return BadRequest("Data não definida.");
            }

            try
            {
                DateTime saoPauloTime = TimeZoneConfig.ConvertToSaoPauloTime(chat.date);
                return Ok(new { Id = $"{chat.Id}", User = $"{chat.Fullname}", Nickname = $"{chat.Fullname}", Message = $"{chat.Message}", Date = $"Data: {saoPauloTime}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Receive message from frontend e insert into the database
        /// </summary>
        /// <param name="user">John Snow</param>
        /// <param name="message">Hello World!</param>
        /// <param name="nickname">jhonsnow</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /chatGeek/send
        ///     {
        ///         "user": John Snow,
        ///         "nickname": johnsnowzin,
        ///         "message": Hello World!
        ///     }
        /// </remarks>
        /// <response code="201">Return the new chat created</response>
        /// <response code="400">Error return, usually the parameter was passed as null</response>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage([FromForm] string user, [FromForm] string nickname, [FromForm] string message)
        {
            // chama o método sendMessage do chatHub
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);

            DateTime date = DateTime.Now.ToLocalTime();

            // instancia um novo chat
            Models.ChatGeek messageChat = new(user, nickname, message);

            // salva no banco de dados a mensagem
            await _chatService.ChatGeek.InsertOneAsync(messageChat);

            return CreatedAtAction(nameof(Index), new { User = $"{user}", Nickname = $"{nickname}", Message = $"{message}" });
        }

        /// <summary>
        /// Editing a message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedChat"></param>
        /// <returns>Old message and the new message</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /chatGeek/id
        ///     {
        ///         "id": 672a80515730065ba59f1eea,
        ///         "user": John Snow,
        ///         "nickname": johnsnow,
        ///         "message": New message
        ///     }
        /// </remarks>
        /// <response code="200">Chat updated with successfully</response>
        /// <response code="404">Chat not found</response>
        /// <response code="500">ID format invalid</response>
        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, Models.ChatGeek updatedChat)
        {
            var chat = await _chatService.GetAsync(id);
            var oldChat = chat;

            if (chat is null)
            {
                return NotFound();
            }

            // mantém o mesmo id, e nome de usuário para a mensagem
            updatedChat.Id = chat.Id;
            updatedChat.Fullname = chat.Fullname;
            updatedChat.date = chat.date;

            await _chatService.UpdateAsync(id, updatedChat);

            return Ok(new { OldMessage = $"{oldChat?.Message}", Message = $"{updatedChat.Message}" });
        }

        /// <summary>
        /// Delete only message from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Chat deleted with successfully</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /chatGeek/id
        ///     {
        ///         "id": 672a80515730065ba59f1eea
        ///     }
        /// </remarks>
        /// <response code="200">Chat deleted with successfully</response>
        /// <response code="404">Chat not found</response>
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var chat = await _chatService.GetAsync(id);

            if (chat is null)
            {
                return NotFound();
            }

            await _chatService.RemoveAsync(id);

            return Ok(new { Message = "Mensagem apagada com sucesso!" });
        }

        /// <summary>
        /// Delete all of messages from database
        /// </summary>
        /// <returns>Chat cleaned with successfully</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     DELETE /chatGeek/delete/all
        /// </remarks>
        /// <response code="202">Chat cleaned with successfully</response>
        [HttpDelete("delete/all")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> DeleteAll()
        {
            await _chatService.Chats.DeleteManyAsync(chat => true);

            return Accepted(new { message = "Chat limpo com sucesso!" });
        }
    }
}
