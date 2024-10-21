using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using chatApi.Hubs;
using chatApi.Models;
using chatApi.Services;

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

        [HttpGet("review", Name = "teste")]
        public async Task<ActionResult> Review()
        {
            var id = "671459283c7d80a0b4e78331";
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
                return Ok(new { Message = $"Data: {saoPauloTime}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // Enviando uma mensagem no chat e salvando no banco de dados
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromForm]string user, [FromForm]string message)
        {
            // chama o método sendMessage do chatHub
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);

            DateTime date = DateTime.Now.ToLocalTime();

            // instancia um novo chat
            Chat messageChat = new(user, message);

            // salva no banco de dados a mensagem
            await _chatService.CreateAsync(messageChat);

            return CreatedAtAction(nameof(Index), new {User = $"{user}", Message = $"{message}" });
        }

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
            updatedChat.UserName = chat.UserName;

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
    }
}
