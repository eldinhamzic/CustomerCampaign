namespace CustomerCampaignService.Application.Errors;

public sealed class CCSException : Exception
{
    public CCSException(int statusCode, string title, string detail)
        : base(detail)
    {
        StatusCode = statusCode;
        Title = title;
    }

    public int StatusCode { get; }
    public string Title { get; }
}
