namespace Clew.BLL
{
    public record TokenDto
    (
        string AccessToken,
        int DurationInMinutes,
        string TokenType = "Bearer"
    );
}
