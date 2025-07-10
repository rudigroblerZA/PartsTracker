using PartsTracker.WebApi.Models;
using System.Linq.Expressions;

namespace PartsTracker.WebApi.Infrastricture;

/// <summary>
/// Defines a contract for repository operations on <see cref="Part"/> entities.
/// </summary>
public interface IPartsRepository
{
    /// <summary>
    /// Asynchronously adds a single <see cref="Part"/> to the context.
    /// </summary>
    /// <param name="entity">The part entity to add.</param>
    Task AddAsync(Part entity);

    /// <summary>
    /// Asynchronously finds parts matching the specified condition.
    /// </summary>
    /// <param name="predicate">The expression used to filter parts.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of parts that satisfy the condition.</returns>
    Task<IEnumerable<Part>> FindAsync(Expression<Func<Part, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all parts from the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of all <see cref="Part"/> entities.</returns>
    Task<IEnumerable<Part>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a part by its primary key values.
    /// </summary>
    /// <param name="keyValues">The key values that identify the part.</param>
    /// <returns>The matching part, or <c>null</c> if not found.</returns>
    Task<Part?> GetByIdAsync(params object[] keyValues);

    /// <summary>
    /// Returns a queryable collection of parts for further querying.
    /// </summary>
    /// <param name="tracking">If <c>true</c>, enables change tracking.</param>
    /// <returns>An <see cref="IQueryable{Part}"/> for querying parts.</returns>
    IQueryable<Part> Query(bool tracking);

    /// <summary>
    /// Removes a single part from the context.
    /// </summary>
    /// <param name="entity">The part entity to remove.</param>
    void Remove(Part entity);

    /// <summary>
    /// Asynchronously adds multiple parts to the context.
    /// </summary>
    /// <param name="parts">The collection of parts to add.</param>
    Task AddRangeAsync(IEnumerable<Part> parts);

    /// <summary>
    /// Removes multiple parts from the context.
    /// </summary>
    /// <param name="parts">The collection of parts to remove.</param>
    void RemoveRange(IEnumerable<Part> parts);

    /// <summary>
    /// Asynchronously saves all changes made in the context to the data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Asynchronously saves all changes made in the context to the data store.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsyncWaitAndRetryAsync();

    /// <summary>
    /// Updates the specified part in the context.
    /// </summary>
    /// <param name="entity">The part entity with updated values.</param>
    void Update(Part entity);
}