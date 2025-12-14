using System.Text.Json.Serialization;

namespace AuthService.Application.DTOs.Responses
{
    /// <summary>
    /// API Error Response (Compliant with RFC 7807).
    /// </summary>
    public class ApiErrorResponseDto
    {
        /// <summary>
        /// Unique reference that identifies the type of error.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "about:blank";

        /// <summary>
        /// Summary of the error.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// HTTP status code.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// Specific explanation of the error.
        /// </summary>
        [JsonPropertyName("details")]
        public string Details { get; set; }

        /// <summary>
        /// Identifies the specific occurrence of the problem (called URI).
        /// </summary>
        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        public ApiErrorResponseDto(int status, string title, string detail, string instance)
        {
            Status = status;
            Title = title;
            Details = detail;
            Instance = instance;
        }

    }

    /// <summary>
    /// API Error response which includes the ocurred error list.
    /// </summary>
    public class ApiValidationErrorResponseDto : ApiErrorResponseDto
    {
        /// <summary>
        /// List of ocurred errors
        /// </summary>
        [JsonPropertyName("errors")]
        public IReadOnlyCollection<string> Errors { get; set; }

        public ApiValidationErrorResponseDto(IReadOnlyCollection<string> errors, string instance = "")
            : base(400, "Validation Error", "One or more validation errors have ocurred", instance)
        {
            Errors = errors;
        }
    }
}
