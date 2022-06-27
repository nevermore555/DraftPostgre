using NpgsqlTypes;

namespace Draft
{
    public class Region
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public NpgsqlPolygon Polygon { get; set; }

        public Region()
        {

        }

        public Region(string Name, NpgsqlPolygon Polygon)
        {
            this.Name = Name;
            this.Polygon = Polygon;
        }
    }
}