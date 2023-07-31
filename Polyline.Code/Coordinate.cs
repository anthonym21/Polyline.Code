using System.Text.RegularExpressions;

namespace Polyline.Code;

public class Coordinate : IEquatable<Coordinate>
{
    private const double Tolerance = 0.0000001;
    private static readonly Regex Format = new Regex(@"\((?<latitude>-?\d{1,3}\.\d{1,6}),(?<longitude>-?\d{1,3}\.\d{1,6})\)",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);
    public Coordinate(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
    }

    public double Latitude { get; }
    public double Longitude { get; }

    public static bool Validate(Coordinate coordinate)
    {
        return coordinate.Latitude is < -90 or > 90 ||
               coordinate.Longitude is < -180 or > 180;
    }

    public bool Equals(Coordinate? other)
    {
        if (other is null)
            return false;
        return Math.Abs(Longitude - other.Longitude) < Tolerance &&
               Math.Abs(Latitude - other.Latitude) < Tolerance;
    }

    public override string ToString()
    {
        return $"({Latitude},{Longitude})";
    }

    public static Coordinate Parse(string? coordinate)
    {
        if (coordinate is null)
            throw new ArgumentNullException(nameof(coordinate));

        var match = Format.Match(coordinate);
        if (!match.Success)
            throw new FormatException("Invalid coordinate format");
        var longitude = double.Parse(match.Groups["longitude"].Value);
        var latitude = double.Parse(match.Groups["latitude"].Value);
        
        return new Coordinate(longitude, latitude);
    }

    public override bool Equals(object? obj)
    {
        return obj is Coordinate point && Equals(point);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Longitude, Latitude);
    }

    public static bool operator ==(Coordinate left, Coordinate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Coordinate left, Coordinate right)
    {
        return !left.Equals(right);
    }

    public void Deconstruct(out double lat, out double lng)
    {
        lat = Latitude;
        lng = Longitude;
    }
}