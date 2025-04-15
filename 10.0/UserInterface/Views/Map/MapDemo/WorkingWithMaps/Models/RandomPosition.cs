namespace WorkingWithMaps.Models
{
    static class RandomPosition
    {
        static Random Random = new Random(Environment.TickCount);

        public static Location Next(Location location, double latitudeRange, double longitudeRange)
        {
            return new Location(
                location.Latitude + (Random.NextDouble() * 2 - 1) * latitudeRange,
                location.Longitude + (Random.NextDouble() * 2 - 1) * longitudeRange);
        }
    }
}
