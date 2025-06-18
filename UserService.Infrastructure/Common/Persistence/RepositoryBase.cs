using UserService.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UserService.Infrastructure.Common.Persistence;

public abstract class RepositoryBase<T> where T : class
{
	protected readonly AppDbContext _db;
	protected readonly DbSet<T> _dbSet;

	protected RepositoryBase(AppDbContext db)
	{
		_db = db ?? throw new ArgumentNullException(nameof(db));
		_dbSet = _db.Set<T>();
	}

	public virtual IQueryable<T> FindAll(bool trackChanges = false) =>
		trackChanges
			? _dbSet
			: _dbSet.AsNoTracking();

	public virtual IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) =>
		trackChanges
			? _dbSet.Where(expression)
			: _dbSet.Where(expression).AsNoTracking();

	public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool trackChanges = false, CancellationToken ct = default) =>
		await FindByCondition(expression, trackChanges)
			.FirstOrDefaultAsync(ct);

	public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? expression = null, CancellationToken ct = default) =>
		expression == null
			? await _dbSet.CountAsync(ct)
			: await _dbSet.CountAsync(expression, ct);

	public virtual void Create(T entity) => _db.Set<T>().Add(entity);
	public virtual async Task CreateAsync(T entity, CancellationToken ct = default) =>
		await _dbSet.AddAsync(entity, ct);
	public virtual async Task CreateRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) =>
		await _dbSet.AddRangeAsync(entities, ct);

	public void Update(T entity) => _dbSet.Update(entity);
	public virtual void UpdateRange(IEnumerable<T> entities) => _dbSet.UpdateRange(entities);

	public void Delete(T entity) => _dbSet.Remove(entity);
	public virtual void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

	public Task SaveAsync() => _db.SaveChangesAsync();
	public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

