using Npgsql;
using NpgsqlTypes;

namespace Draft
{
    public sealed class PgsqlDB : IRegionRepository
    {
        private readonly string _connectionString;

        public PgsqlDB(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }

        public async Task<long> Create(Region region, Error error, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                await using (var command = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS Region (id bigserial primary key, name varchar(150), region polygon)", connection))
                {
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                await using (var command = new NpgsqlCommand("INSERT INTO Region (name, region) VALUES (@name, @region) RETURNING ID", connection))
                {

                    command.Parameters.AddWithValue("name", region.Name);
                    command.Parameters.AddWithValue("region", region.Polygon);

                    return (long)await command.ExecuteScalarAsync(cancellationToken);
                }
            }
            catch (NpgsqlException ex)
            {
                error.ErrorNumber = ex.IsTransient ? 1 : -1;

                if (error.ErrorNumber == -1)
                    throw new DALException();

                return 0;
            }
        }

        public async Task<Region> GetRegion(long id, Error error, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                await using (var command = new NpgsqlCommand("SELECT * FROM public.region WHERE id = @searchId;", connection))
                {
                    Region region = new Region();

                    command.Parameters.AddWithValue("searchId", id);
                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows && dataReader.Read())
                        {
                            region.Id = Convert.ToInt32(dataReader["id"]);
                            region.Name = dataReader["name"].ToString();

                            double [] pointsArray = dataReader["region"].ToString()
                                                              .Replace("(", String.Empty)
                                                              .Replace(")", String.Empty)
                                                              .Split('\u002C')
                                                              .Select(double.Parse)
                                                              .ToArray();

                            region.Polygon = new NpgsqlPolygon(pointsArray.Length/2);
                            for (int i = 0; i < pointsArray.Length; i = i + 2)
                            {
                                region.Polygon.Add(new NpgsqlPoint(pointsArray[i], pointsArray[i + 1]));
                            }
                        }
                    }

                    return region;
                }
            }
            catch (NpgsqlException ex)
            {
                error.ErrorNumber = ex.IsTransient ? 1 : -1;

                if (error.ErrorNumber == -1)
                    throw new DALException();

                return null;
            }
        }
    }
}
