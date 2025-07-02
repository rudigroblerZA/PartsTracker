using PartsTracker.Models;
using System.Linq.Expressions;

namespace PartsTracker.Infrastructure
{
    public interface IPartsRepository
    {
        Task AddAsync(Part entity);
        Task<IEnumerable<Part>> FindAsync(Expression<Func<Part, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Part>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Part?> GetByIdAsync(params object[] keyValues);
        IQueryable<Part> Query(bool tracking);
        void Remove(Part entity);
        Task AddRangeAsync(IEnumerable<Part> parts);
        void RemoveRange(IEnumerable<Part> parts);
        Task<int> SaveChangesAsync();
        void Update(Part entity);
    }
}