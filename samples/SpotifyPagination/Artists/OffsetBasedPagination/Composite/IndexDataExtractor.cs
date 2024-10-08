using OffsetBased;
using OffsetBased.Composite.IndexDataExtractors;

namespace SpotifyPagination.Artists.OffsetBasedPagination.Composite;

public class IndexDataExtractor : IIndexDataExtractor<GetAlbumsResponse>
{
    public Task<IndexData<int>> ExtractIndexDataAsync(GetAlbumsResponse context,
        CancellationToken cancellationToken = default) 
        => Task.FromResult(new IndexData<int>(context.Offset, context.Total));
}
