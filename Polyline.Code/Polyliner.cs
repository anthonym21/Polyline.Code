using System.Text;

namespace Polyline.Code
{
public static class Polyliner
{
    /// <summary>
    /// Encode a polyline string  using Google's encoded polyline algorithm
    /// </summary>
    /// <param name="polylineString">Unencoded polyline string</param>
    /// <returns>Polyline encoded with Google's Encoded Polyline Algorithm</returns>
    public static string Encode(string polylineString)
    {
       
        // Parse the string into a list of latitude and longitude pairs
        var stringCoordinates = polylineString.TrimStart('[').TrimEnd(']').Split("),(");
        var coordinates = stringCoordinates.Select(coord =>
        {
            var parts = coord.TrimStart('(').TrimEnd(')').Split(',');
            return new Coordinate(double.Parse(parts[0]), double.Parse(parts[1]));
        });

        return Encode(coordinates);
    }

    /// <summary>
    /// Encode a list of coordinates using Google's encoded polyline algorithm
    /// </summary>
    /// <param name="coordinates">Enumerable collection of Coordinate</param>
    /// <returns>Polyline encoded with Google's Encoded Polyline Algorithm</returns>
    public static string Encode(IEnumerable<Coordinate> coordinates)
    {

        var result = new StringBuilder();
        int prevLat = 0, prevLng = 0;

        foreach (var (lat, lng) in coordinates)
        {
            var lat5 = (int)Math.Round(lat * 1e5);
            var lng5 = (int)Math.Round(lng * 1e5);

            EncodeValue(lat5 - prevLat, result);
            EncodeValue(lng5 - prevLng, result);

            prevLat = lat5;
            prevLng = lng5;
        }

        return result.ToString();
    }
    
    private static void EncodeValue(int value, StringBuilder result)
    {
        value <<= 1;
        if (value < 0) value = ~value;

        while (value >= 0x20)
        {
            result.Append((char)((0x20 | (value & 0x1f)) + 63));
            value >>= 5;
        }

        result.Append((char)(value + 63));
    }
    
    /// <summary>
    /// Decode a polyline encoded with Google's encoded polyline algorithm and a return a list of coordinates
    /// </summary>
    /// <param name="encoded">Encoded polyline as string</param>
    /// <returns>List of coordinate structs</returns>
    public static CoordinateList DecodePolylineToObjects(string encoded)
    {
        var coordinates = new CoordinateList();
        int index = 0, len = encoded.Length;
        int lat = 0, lng = 0;

        while (index < len)
        {
            lat += DecodeValue(encoded, ref index);
            lng += DecodeValue(encoded, ref index);
            coordinates.Add(new Coordinate(lat * 1e-5, lng * 1e-5));
        }

        return coordinates;
    }

    /// <summary>
    /// Decode a polyline encoded with Google's encoded polyline algorithm and a return an
    /// uncoded polyline string
    /// </summary>
    /// <param name="encoded">A google algorithm encryped polyline string</param>
    /// <returns>a decoded polyline string</returns>
    public static string DecodePolylineToString(string encoded)
    {
        var coordinates = DecodePolylineToObjects(encoded);
        return coordinates.ToString();
    }
    
    private static int DecodeValue(string encoded, ref int index)
    {
        int value = 0, shift = 0, b;
        do
        {
            b = encoded[index++] - 63;
            value |= (b & 0x1f) << shift;
            shift += 5;
        } while (b >= 0x20);

        return (value & 1) != 0 ? ~(value >> 1) : value >> 1;
    }
}
}