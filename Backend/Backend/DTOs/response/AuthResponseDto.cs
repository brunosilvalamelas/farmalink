namespace Backend.DTOs.response;

public class AuthResponseDto
{
    public AuthResponseDto(LoginResponseDto loginResponse, string token)
    {
        LoginResponse = loginResponse;
        Token = token;
    }

    public LoginResponseDto LoginResponse { get; set; }
    public string Token { get; set; }
}