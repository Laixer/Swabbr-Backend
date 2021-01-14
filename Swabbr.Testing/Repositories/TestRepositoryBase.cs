using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Base class for holding test repository items.
    /// </summary>
    public abstract class TestRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : EntityBase<TPrimaryKey>
    {
        private readonly List<TEntity> Entities = new List<TEntity>();

        /// <summary>
        ///     Gets all entities in this datastore in a 
        ///     readonly collection.
        /// </summary>
        /// <returns>All entities.</returns>
        protected IReadOnlyCollection<TEntity> AllEntities()
            => Entities.AsReadOnly();

        /// <summary>
        ///     Adds an entity to this data store.
        /// </summary>
        /// <param name="entity">The entity to store.</param>
        protected void AddEntity(TEntity entity)
            => Entities.Add(entity);

        /// <summary>
        ///     Deletes an entity from this data store.
        /// </summary>
        /// <remarks>
        ///     Throws an <see cref="EntityNotFoundException"/>
        ///     if the item does not exist.
        /// </remarks>
        /// <param name="id">The entity id.</param>
        protected void DeleteEntity(TPrimaryKey id)
        {
            if (!HasEntity(id))
            {
                throw new EntityNotFoundException();
            }

            Entities.Remove(GetEntityById(id));
        }

        /// <summary>
        ///     Gets an entity by its id.
        /// </summary>
        /// <remarks>
        ///     Throws an <see cref="EntityNotFoundException"/>
        ///     if the item does not exist.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        protected TEntity GetEntityById(TPrimaryKey id)
        { 
            if (!HasEntity(id))
            {
                throw new EntityNotFoundException();
            }

            return Entities.Where(x => x.Id.Equals(id)).Single();
        }

        /// <summary>
        ///     Checks if an entity exists in this data store.
        /// </summary>
        /// <param name="id">Entity id.</param>
        protected bool HasEntity(TPrimaryKey id)
            => Entities.Where(x => x.Id.Equals(id)).Count() == 1;

        /// <summary>
        ///     Updates an entity in this data store.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Throws an <see cref="EntityNotFoundException"/>
        ///         if the item does not exist.
        ///     </para>
        ///     <para>
        ///         This calls <see cref="DeleteEntity(TPrimaryKey)"/>,
        ///         then calls <see cref="AddEntity(TEntity)"/>.
        ///     </para>
        /// </remarks>
        /// <param name="entity">The entity with updated properties.</param>
        protected void UpdateEntity(TEntity entity)
        {
            if (!HasEntity(entity.Id))
            {
                throw new EntityNotFoundException();
            }

            DeleteEntity(entity.Id);
            AddEntity(entity);
        }
    }
}
