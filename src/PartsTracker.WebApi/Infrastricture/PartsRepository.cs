using Microsoft.EntityFrameworkCore;
using PartsTracker.Server.Data;
using PartsTracker.WebApi.Models;
using System.Linq.Expressions;

namespace PartsTracker.WebApi.Infrastricture;

/// <summary>
/// A generic repository implementation for managing <see cref="Part"/> entities using Entity Framework Core.
/// </summary>
public class PartsRepository : IPartsRepository
{
    private readonly InventoryDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartsRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to use for data access.</param>
    public PartsRepository(InventoryDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a part by its primary key.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the part.</param>
    /// <returns>The matching <see cref="Part"/> if found; otherwise, <c>null</c>.</returns>
    public async Task<Part?> GetByIdAsync(params object[] keyValues)
    {
        if (keyValues is null || keyValues.Length == 0)
            throw new ArgumentException("At least one key value must be provided.", nameof(keyValues));

        return await _context.Parts.FindAsync(keyValues);
    }

    /// <summary>
    /// Gets all parts in the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of all <see cref="Part"/> entities.</returns>
    public async Task<IEnumerable<Part>> GetAllAsync(CancellationToken cancellationToken = default) => await _context.Parts.ToListAsync(cancellationToken);

    /// <summary>
    /// Finds parts that match a specified condition.
    /// </summary>
    /// <param name="predicate">The expression used to filter parts.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of <see cref="Part"/> entities that match the condition.</returns>
    public async Task<IEnumerable<Part>> FindAsync(Expression<Func<Part, bool>> predicate, CancellationToken cancellationToken = default) => await _context.Parts.Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    /// Validates a Part entity for business rules.
    /// </summary>
    /// <param name="entity">The <see cref="Part"/> entity to validate.</param>
    private static void ValidatePart(Part entity)
    {
        if (entity.QuantityOnHand < 0)
            throw new ArgumentException("QuantityOnHand cannot be negative.", nameof(entity));
    }

    /// <summary>
    /// Adds a new part to the context.
    /// </summary>
    /// <param name="entity">The <see cref="Part"/> entity to add.</param>
    public async Task AddAsync(Part entity)
    {
        ValidatePart(entity);
        await _context.Parts.AddAsync(entity);
    }

    /// <summary>
    /// Updates an existing part in the context.
    /// </summary>
    /// <param name="entity">The <see cref="Part"/> entity to update.</param>
    public void Update(Part entity)
    {
        ValidatePart(entity);

        _context.Entry(entity).Property("xmin").OriginalValue = entity.xmin;
        _context.Entry(entity).State = EntityState.Modified;

        _context.Parts.Update(entity);
    }

    /// <summary>
    /// Removes a part from the context.
    /// </summary>
    /// <param name="entity">The <see cref="Part"/> entity to remove.</param>
    public void Remove(Part entity) => _context.Parts.Remove(entity);

    /// <summary>
    /// Returns a queryable collection of parts.
    /// </summary>
    /// <param name="tracking">Enable tracking</param>
    /// <returns>An <see cref="IQueryable{Part}"/> that can be used for further querying.</returns>
    public IQueryable<Part> Query(bool tracking) => tracking ? _context.Parts : _context.Parts.AsNoTracking();

    /// <summary>
    /// Adds multiple parts to the context.
    /// </summary>
    /// <param name="parts">A collection of parts to add.</param>
    public async Task AddRangeAsync(IEnumerable<Part> parts) => await _context.Parts.AddRangeAsync(parts);

    /// <summary>
    /// Removes multiple parts from the context.
    /// </summary>
    /// <param name="parts">A collection of parts to remove.</param>
    public void RemoveRange(IEnumerable<Part> parts) => _context.Parts.RemoveRange(parts);


    /// <summary>
    /// Persists all changes made in the context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
