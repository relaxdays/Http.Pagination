﻿using System.Runtime.CompilerServices;
using Core;
using JetBrains.Annotations;
using Sequential.Composite;

namespace Sequential;

// TODO: Probably move this to it's own package, e.g. Sequential

/// <summary>
/// Retrieves all items from a paginated resource by sequentially going from one page to the next, as long as more pages
/// are available.
/// </summary>
/// <remarks>
/// The algorithm to retrieve all pages basically looks like this:
/// <list type="number">
/// <item>
///     Retrieve the next page.
/// </item>
/// <item>
///     Transform the page, typically to extract information about the next page, as well as the items on the page.
/// </item>
/// <item>
///     Yield all items on the page.
/// </item>
/// <item>
///     While there is a next page, repeat the previous three steps.
/// </item>
/// </list>
/// </remarks>
/// <typeparam name="TPage">The type of the transformed pages.</typeparam>
/// <typeparam name="TTransformedPage">The type of the transformed pages.</typeparam>
/// <typeparam name="TItem">The type of the items.</typeparam>
[PublicAPI]
public abstract class PaginationHandler<TPage, TTransformedPage, TItem> : IPaginationHandler<TItem>
{
    /// <inheritdoc />
    public async IAsyncEnumerable<TItem> GetAllItemsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IPaginationContext<TTransformedPage>? context = null;

        while (context is null || context.NextPageExists)
        {
            // TODO: This is strange, why can't we use `context?.CurrentPage`?
            var lastPage = context is null ? default : context.CurrentPage;
            var currentPage = await GetPageAsync(lastPage, cancellationToken);
            
            context = await ExtractContextAsync(currentPage, cancellationToken);

            await foreach (var item in ExtractItemsAsync(context.CurrentPage, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }

    /// <summary>
    /// Retrieves the first page of the paginated resource.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The first page.</returns>
    protected abstract Task<TPage> GetPageAsync(
        // TODO: Should we take the whole context instead?
        TTransformedPage? currentPage,
        CancellationToken cancellationToken = default);

    // TODO: Should this be its own method, or should it be implicit in TPage, i.e. we always operate on transformed pages?
    /// <summary>
    /// Transforms the given <paramref name="page"/>, typically to extract information about the next page, as well as
    /// the items on the page.
    /// </summary>
    /// <param name="page">The page to transform.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The transformed page.</returns>
    protected abstract Task<IPaginationContext<TTransformedPage>> ExtractContextAsync(
        TPage page, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts the items from the given <paramref name="page"/>.
    /// </summary>
    /// <param name="page">The page to extract the items from.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>An async enumerable of the items.</returns>
    protected abstract IAsyncEnumerable<TItem> ExtractItemsAsync(
        TTransformedPage page, CancellationToken cancellationToken = default);
}
