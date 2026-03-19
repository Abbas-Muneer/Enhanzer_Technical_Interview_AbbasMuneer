using System.Net.Http.Json;
using System.Text.Json;
using Enhanzer.Api.Interfaces;
using Enhanzer.Api.Models.DTOs;

namespace Enhanzer.Api.Services;

public class ExternalPosApiService(
    HttpClient httpClient,
    ILogger<ExternalPosApiService> logger) : IExternalPosApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ExternalLoginResponseDto> LoginAsync(ExternalLoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync("api/External_Api/POS_Api/Invoke", request, JsonOptions, cancellationToken);
            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("External POS API returned {StatusCode}: {Body}", response.StatusCode, rawResponse);
                return new ExternalLoginResponseDto
                {
                    Success = false,
                    Message = "External login service rejected the request.",
                    RawResponse = rawResponse
                };
            }

            return ParseResponse(rawResponse);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "External POS API request timed out.");
            return new ExternalLoginResponseDto
            {
                Success = false,
                Message = "External login service timed out."
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "External POS API request failed.");
            return new ExternalLoginResponseDto
            {
                Success = false,
                Message = "Could not reach the external login service."
            };
        }
    }

    private static ExternalLoginResponseDto ParseResponse(string rawResponse)
    {
        if (string.IsNullOrWhiteSpace(rawResponse))
        {
            return new ExternalLoginResponseDto
            {
                Success = false,
                Message = "External login service returned an empty response.",
                RawResponse = rawResponse
            };
        }

        try
        {
            using var document = JsonDocument.Parse(rawResponse);
            var root = document.RootElement;
            var userLocations = ExtractUserLocations(root);
            var success = DetermineSuccess(root, userLocations.Count);
            var message = ExtractMessage(root, success, userLocations.Count);

            return new ExternalLoginResponseDto
            {
                Success = success,
                Message = message,
                UserLocations = userLocations,
                RawResponse = rawResponse
            };
        }
        catch (JsonException)
        {
            return new ExternalLoginResponseDto
            {
                Success = false,
                Message = "External login service returned malformed JSON.",
                RawResponse = rawResponse
            };
        }
    }

    private static List<UserLocationDto> ExtractUserLocations(JsonElement element)
    {
        if (TryFindProperty(element, "User_Locations", out var locationsElement) &&
            locationsElement.ValueKind == JsonValueKind.Array)
        {
            return locationsElement.EnumerateArray()
                .Select(location => new UserLocationDto
                {
                    Location_Code = ReadString(location, "Location_Code"),
                    Location_Name = ReadString(location, "Location_Name")
                })
                .Where(location => !string.IsNullOrWhiteSpace(location.Location_Code) || !string.IsNullOrWhiteSpace(location.Location_Name))
                .ToList();
        }

        return [];
    }

    private static bool DetermineSuccess(JsonElement root, int locationCount)
    {
        if (TryReadBoolean(root, out var booleanResult))
        {
            return booleanResult;
        }

        if (TryFindProperty(root, "Status", out var statusElement))
        {
            var status = statusElement.ToString();
            if (Matches(status, "success", "true", "1", "ok"))
            {
                return true;
            }

            if (Matches(status, "fail", "failed", "false", "0", "error"))
            {
                return false;
            }
        }

        if (TryFindProperty(root, "Message", out var messageElement))
        {
            var message = messageElement.ToString();
            if (Matches(message, "invalid", "failed", "error", "unauthorized"))
            {
                return false;
            }
        }

        return locationCount > 0;
    }

    private static string ExtractMessage(JsonElement root, bool success, int locationCount)
    {
        if (TryFindProperty(root, "Message", out var messageElement))
        {
            var message = messageElement.ToString();
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message;
            }
        }

        if (!success && locationCount == 0)
        {
            return "Login failed. No locations were returned.";
        }

        return success ? "Login successful." : "Login failed.";
    }

    private static bool TryFindProperty(JsonElement element, string propertyName, out JsonElement foundValue)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    foundValue = property.Value;
                    return true;
                }

                if (TryFindProperty(property.Value, propertyName, out foundValue))
                {
                    return true;
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (TryFindProperty(item, propertyName, out foundValue))
                {
                    return true;
                }
            }
        }

        foundValue = default;
        return false;
    }

    private static bool TryReadBoolean(JsonElement element, out bool result)
    {
        foreach (var key in new[] { "Success", "IsSuccess", "Is_Success", "Status" })
        {
            if (TryFindProperty(element, key, out var found))
            {
                if (found.ValueKind == JsonValueKind.True || found.ValueKind == JsonValueKind.False)
                {
                    result = found.GetBoolean();
                    return true;
                }

                if (bool.TryParse(found.ToString(), out var boolResult))
                {
                    result = boolResult;
                    return true;
                }
            }
        }

        result = false;
        return false;
    }

    private static string ReadString(JsonElement element, string propertyName)
    {
        if (TryFindProperty(element, propertyName, out var property))
        {
            return property.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static bool Matches(string? value, params string[] fragments)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return fragments.Any(fragment => value.Contains(fragment, StringComparison.OrdinalIgnoreCase));
    }
}
