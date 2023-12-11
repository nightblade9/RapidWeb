namespace Stocks.Web.Models;

class RegisterModel
{
    public int Id { get; set; }
    public string EmailAddress { get; set; } = default!;
    public string PlainTextPassword { get; set; } = default!;
    public string PlainTextPasswordAgain { get; set; } = default!;
}