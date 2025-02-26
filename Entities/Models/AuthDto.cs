public class AuthLoginDto
{
    public required string Password { get; set; }

    public required string Username { get; set; }
}

public class AuthResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class AuthRefreshTokenRequestDto {
    public required int ID { get; set; }
    public required string RefreshToken{ get; set; }
}