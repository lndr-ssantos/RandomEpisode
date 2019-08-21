using System;
using CSharpFunctionalExtensions;

namespace RandomEpisodeAPI.Infra.Gateways
{
    public class Title
    {
        public string Value { get; }

        protected Title(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title can not be empty nor null");
            }

            Value = title;
        }

        public static Result<Title> Create(string title)
        {
            try
            {
                var titleAsVO = new Title(title);
                return Result.Ok(titleAsVO);
            }
            catch (ArgumentException ae)
            {
                return Result.Fail<Title>(ae.Message);
            }
        }
    }
}
