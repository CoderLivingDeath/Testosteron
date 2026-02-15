using System.Linq.Expressions;

/// <summary>
/// Generic Repository interface providing standard CRUD (Create, Read, Update, Delete) 
/// operations for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type that implements <see cref="class"/>.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Asynchronously adds a single entity to the data store.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the async operation 
    /// that returns the added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Asynchronously adds multiple entities to the data store.
    /// </summary>
    /// <param name="entities">An <see cref="IEnumerable{T}"/> of entities to add.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously retrieves a single entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the async operation 
    /// that returns the entity or null if not found.</returns>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Asynchronously retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the async operation 
    /// that returns an <see cref="IEnumerable{T}"/> of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Asynchronously finds entities matching the specified predicate.
    /// </summary>
    /// <param name="predicate">A <see cref="Expression{Func{T, bool}}"/> to test each entity.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the async operation 
    /// that returns matching entities.</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Asynchronously updates a single entity in the data store.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Asynchronously deletes a single entity from the data store.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Asynchronously deletes a single entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A <see cref="Task"/> representing the async operation.</returns>
    Task DeleteByIdAsync(int id);

    /// <summary>
    /// Asynchronously saves all pending changes to the data store.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the async operation 
    /// that returns the number of affected rows.</returns>
    Task<int> SaveChangesAsync();
}
