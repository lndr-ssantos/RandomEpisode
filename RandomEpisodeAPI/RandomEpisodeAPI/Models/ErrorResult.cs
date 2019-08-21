using System;
namespace RandomEpisodeAPI.Models
{
    public class ErrorResult
    {
        public ErrorResult(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
