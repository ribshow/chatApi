using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using chatApi.Hubs;
using chatApi.Models;
using chatApi.Services;
using MongoDB.Driver;

namespace chatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        private readonly IHubContext<ChatHub> _hubContext;

        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger, IHubContext<ChatHub> hubContext, ChatService chatService)
        {
            _chatService = chatService;
            _hubContext = hubContext;
            _logger = logger;
        }
        /// <summary>
        /// Returns all messages saved
        /// </summary>
        /// <returns></returns>
        /// <response code="200">All messages were returned</response>
        // exibindo todas as mensagens do chat
        [HttpGet(Name = "chatHub")]
        public async Task<List<Chat>> Index()
        {
           var chats = await _chatService.GetAsync();

            foreach(Chat item in chats)
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
        ///     GET chat/id
        ///     {
        ///         "user": John Snow,
        ///         "nickname": johnsnowzin,
        ///         "message": Hello World!,
        ///         "date": 05/11/2024 - 22:50:44
        ///     }
        /// </remarks>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult> Details(string id)
        {
            //1var id = "671459283c7d80a0b4e78331";
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
                return Ok(new {Id = $"{chat.Id}" ,User = $"{chat.Fullname}", Nickname = $"{chat.Fullname}", Message = $"{chat.Message}" ,Date = $"Data: {saoPauloTime}" });
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
        ///     POST /chat/send
        ///     {
        ///         "user": John Snow,
        ///         "nickname": johnsnowzin,
        ///         "message": Hello World!
        ///     }
        /// </remarks>
        /// <response code="201">Return the new chat created</response>
        /// <response code="400">Error return, usually the parameter was passed as null</response>
        // Enviando uma mensagem no chat e salvando no banco de dados
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage([FromForm]string user, [FromForm]string nickname, [FromForm] string message)
        {
            // chama o método sendMessage do chatHub
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);

            DateTime date = DateTime.Now.ToLocalTime();

            // instancia um novo chat
            Chat messageChat = new(user, nickname, message);

            // salva no banco de dados a mensagem
            await _chatService.CreateAsync(messageChat);

            return CreatedAtAction(nameof(Index), new {User = $"{user}", Nickname = $"{nickname}" ,Message = $"{message}" });
        }

        /// <summary>
        /// Editing a message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedChat"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /chat/id
        ///     {
        ///         "id": 672a80515730065ba59f1eea,
        ///         "user": John Snow,
        ///         "nickname": johnsnow,
        ///         "message": New message
        ///     }
        /// </remarks>
        // atualizando uma mensagem do chat
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Chat updatedChat)
        {
            var chat = await _chatService.GetAsync(id);

            if(chat is null)
            {
                return NotFound();
            }

            // mantém o mesmo id, e nome de usuário para a mensagem
            updatedChat.Id = chat.Id;
            updatedChat.Fullname = chat.Fullname;

            await _chatService.UpdateAsync(id, updatedChat);

            return Ok(new { Message = $"{updatedChat.Message}" });
        }

        // apagando uma mensagem do chat
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var chat = await _chatService.GetAsync(id);

            if(chat is null)
            {
                return NotFound();
            }

            await _chatService.RemoveAsync(id);

            return Ok(new { Message = "Mensagem apagada com sucesso!" });
        }

        [HttpDelete("delete/all")]
        public async Task<IActionResult> DeleteAll()
        {
            await _chatService.Chats.DeleteManyAsync(c => true);

            return Ok(new { message = "Chat limpo com sucesso!" });
        }
    }
}
