using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Base
{
    public abstract class BaseRepo<TContext, TEntity> where TContext:IUnitOfWork
    {
        protected readonly TContext _context;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public BaseRepo(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <summary>
        /// 'Add' method to "Insert" a new record. 'IsConsistent' must be set to 'True' before 'Add' is called
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract TEntity Add(TEntity entity);
        /// <summary>
        /// Updates ALL properties of entity to "updated. This is normally not required. Once an "entity" is tracked by Entity framework, any updates
        /// to the properties will automatcially kept track.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract Task<EntityUpdateResult> Update(TEntity item);

        /// <summary>
        /// Attaches a "newItem" by detaching the previous entity first
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public virtual EntityUpdateResult Attach(TEntity newItem) { throw new NotImplementedException(); }
        protected static void DisplayStates(IEnumerable<EntityEntry> entries)
        {
            foreach (var entry in entries)
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: { entry.State.ToString()}");
            }
        }
    }
}
