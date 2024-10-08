using JetBrains.Annotations;
using Sequential.Composite.PageRetrievers;

namespace Sequential.Composite;

[PublicAPI]
public class PaginationHandler<TPaginationContext, TItem>
    : Sequential.PaginationHandler<TPaginationContext, TItem>
    where TPaginationContext : class
{
    private readonly IPageRetriever<TPaginationContext> _pageRetriever;
    private readonly NextPageCheckers.INextPageChecker<TPaginationContext> _nextPageChecker;
    private readonly ItemExtractors.IItemExtractor<TPaginationContext, TItem> _itemExtractor;

    public PaginationHandler(
        IPageRetriever<TPaginationContext> pageRetriever,
        NextPageCheckers.INextPageChecker<TPaginationContext> nextPageChecker,
        ItemExtractors.IItemExtractor<TPaginationContext, TItem> itemExtractor)
    {
        _pageRetriever = pageRetriever;
        _nextPageChecker = nextPageChecker;
        _itemExtractor = itemExtractor;
    }

    protected override Task<TPaginationContext> GetPageAsync(
        TPaginationContext? context,
        CancellationToken cancellationToken = default)
        => _pageRetriever.GetAsync(context, cancellationToken);

    protected override Task<bool> NextPageExistsAsync(
        TPaginationContext context,
        CancellationToken cancellationToken = default)
        => _nextPageChecker.NextPageExistsAsync(context, cancellationToken);

    protected override IAsyncEnumerable<TItem> ExtractItemsAsync(
        TPaginationContext context,
        CancellationToken cancellationToken = default)
        => _itemExtractor.ExtractItemsAsync(context, cancellationToken);
}
