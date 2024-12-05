namespace chatApi.Responses
{
    public class AuthResponse
    {
        public string? message { get; set; } = "User authenticated successfully";
        public string? token { get; set; }
    }
}
