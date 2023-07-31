using System.Collections;
using System.Globalization;

namespace Polyline.Code;

public class CoordinateList : IEnumerable<Coordinate>
{
    private readonly List<Coordinate> _coordinates = new();
    public CoordinateList() { }

    public CoordinateList(IEnumerable<Coordinate> coordinates) { _coordinates.AddRange(coordinates);}

    public IEnumerator<Coordinate> GetEnumerator()
    {
        return _coordinates.GetEnumerator();
    }

    public override string ToString()
    {
        return $"[{string.Join(",", this)}]";
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static CoordinateList Parse(string? coordinateList)
    {
        if (coordinateList is null)
            throw new ArgumentNullException(nameof(coordinateList));

        var coordinates = new CoordinateList();
        var stringCoordinates = coordinateList.TrimStart('[').TrimEnd(']').Split("),(");
        foreach (var coord in stringCoordinates)
        {
            var parts = coord.TrimStart('(').TrimEnd(')').Split(',');
            if (parts.Length != 2)
                throw new FormatException("Invalid coordinate format.");

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) ||
                !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                throw new FormatException("Invalid coordinate values.");

            coordinates.Add(new Coordinate(lat, lon));
        }

        return coordinates;
    }

    public void AddRange(IEnumerable<Coordinate> list)
    {
        _coordinates.AddRange(list);
    }
    
    public void Add(Coordinate coordinate)
    {
        _coordinates.Add(coordinate);
    }

    public int Count => _coordinates.Count;

    public Coordinate this[int index]
    {
        get => _coordinates[index];
        set => _coordinates[index] = value;
    }

    public bool Contains(Coordinate coordinate) => _coordinates.Contains(coordinate);

    public void Remove(Coordinate coordinate) => _coordinates.Remove(coordinate);

    public void Clear() => _coordinates.Clear();

    public IEnumerable<Coordinate> Find(Func<Coordinate, bool> predicate) => _coordinates.Where(predicate);

}