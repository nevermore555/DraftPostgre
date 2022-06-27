namespace Draft
{
    public interface IRegionRepository
    {
        Task<long> Create(Region region, Error error, CancellationToken cancellationToken = default);
        Task<Region>GetRegion (long id, Error error, CancellationToken cancellationToken = default);
        //Task Update(Region Name, Error error, CancellationToken cancellationToken = default);
        //Task <Region>Delete(long id, Error error, CancellationToken cancellationToken = default);
        //IAsyncEnumerable<Region> GetRegions(CancellationToken = default);
    }
}