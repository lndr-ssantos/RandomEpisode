using System;
namespace RandomEpisodeAPI.Models.TvShows
{
    public class NumberPicker
    {
        public int Value { get; }

        public NumberPicker(int maxNumber)
        {
            if(maxNumber <= 0)
            {
                throw new ArgumentException("Number must be greater than 0");
            }
            var random = new Random();
            Value = random.Next(1, maxNumber);
        }
    }
}
