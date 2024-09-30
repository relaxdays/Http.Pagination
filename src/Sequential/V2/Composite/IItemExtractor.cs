using JetBrains.Annotations;

namespace Sequential.V2.Composite;

[PublicAPI]
public interface IItemExtractor<in TPaginationContext, out TItem>
{
    IAsyncEnumerable<TItem> ExtractItemsAsync(TPaginationContext context,
        CancellationToken cancellationToken = default);
}
