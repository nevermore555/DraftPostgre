using Draft;
using NpgsqlTypes;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var DB = new PgsqlDB("Host=localhost;Port=5432;Database=draft;Username=postgres;Password=postgres");

        NpgsqlPolygon polygon = new NpgsqlPolygon(new NpgsqlPoint(1, 1), new NpgsqlPoint(3, 1), new NpgsqlPoint(2, 3), new NpgsqlPoint(1, 1));
        Region region = new Region("Odessa", polygon);
        var error = new Error();
        var ID = await DB.Create(region, error);

        if (error.ErrorNumber == 1)
        {
            Console.Error.WriteLine("Please retry request");
        }
        else if (error.ErrorNumber == 0)
        {
            Console.WriteLine("You have created Region name: {0}, Error = {1}.", ID, error.ErrorNumber);
        }

        error.ErrorNumber = 0;
        var searchRegion = await DB.GetRegion(1, error);

        if (error.ErrorNumber == 1)
        {
            Console.Error.WriteLine("Please retry request");
        }
        else if (error.ErrorNumber == 0)
        {
            Console.WriteLine("Region:\n\tID: {0}\n\tName: {1}\n\tPolygon: {2}, Error = {3}.", searchRegion.Id, searchRegion.Name, searchRegion.Polygon.ToString(), error.ErrorNumber);
        }

    }
}