using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using CustomerCampaignService.Application.Contracts.Customers;
using CustomerCampaignService.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CustomerCampaignService.Infrastructure.Services;

public sealed class SoapCustomerLookupService : ICustomerLookupService
{
    private const string SoapAction = "http://tempuri.org/FindPerson";

    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public SoapCustomerLookupService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _endpoint = configuration["Soap:Endpoint"]
            ?? throw new InvalidOperationException("Missing configuration: Soap:Endpoint");
    }

    public async Task<CustomerLookupResult?> FindPersonAsync(string externalCustomerRef,CancellationToken cancellationToken)
    {
        var soapEnvelope = BuildSoapEnvelope(externalCustomerRef);
        using var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.Clear();
        content.Headers.ContentType = new MediaTypeHeaderValue("text/xml") { CharSet = "utf-8" };
        content.Headers.Add("SOAPAction", "\"http://tempuri.org/FindPerson\"");

        using var response = await _httpClient.PostAsync(_endpoint, content, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var xml = await response.Content.ReadAsStringAsync(cancellationToken);
            var parsed = ParseResponse(xml, externalCustomerRef);
            if (parsed is not null)
            {
                return parsed;
            }
        }

        var fallbackUrl = $"{_endpoint}?soap_method=FindPerson&id={Uri.EscapeDataString(externalCustomerRef)}";
        using var fallbackResponse = await _httpClient.GetAsync(fallbackUrl, cancellationToken);
        if (!fallbackResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var fallbackXml = await fallbackResponse.Content.ReadAsStringAsync(cancellationToken);
        return ParseResponse(fallbackXml, externalCustomerRef);
    }

    private static string BuildSoapEnvelope(string externalCustomerRef)
    {
        return $@"<?xml version=""1.0"" encoding=""utf-8""?>
        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                       xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                       xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
          <soap:Body>
            <FindPerson xmlns=""http://tempuri.org/"">
              <id>{System.Security.SecurityElement.Escape(externalCustomerRef)}</id>
            </FindPerson>
          </soap:Body>
        </soap:Envelope>";
    }

    private static CustomerLookupResult? ParseResponse(string xml, string externalCustomerRef)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml);
        }
        catch
        {
            return null;
        }

        var resultElement = doc.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("FindPersonResult", StringComparison.OrdinalIgnoreCase));

        if (resultElement is null)
        {
            return null;
        }

        var nameElement = resultElement.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("Name", StringComparison.OrdinalIgnoreCase));

        var rawName = nameElement?.Value?.Trim();
        if (string.IsNullOrWhiteSpace(rawName) || rawName.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var (firstName, lastName) = SplitName(rawName);
        return new CustomerLookupResult(externalCustomerRef, firstName, lastName);
    }

    private static (string FirstName, string LastName) SplitName(string rawName)
    {
        var parts = rawName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return (string.Empty, string.Empty);
        }

        if (parts.Length == 1)
        {
            return (parts[0], string.Empty);
        }

        var lastName = parts[^1];
        var firstName = string.Join(' ', parts[..^1]);
        return (firstName, lastName);
    }
}
