using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Extensions
{
    public static class EFExtensions
    {
        public static T GetByPrimaryKey<T>(this BaseDBContext context, T disconnectedEntity) where T : class
        {
            var keyValues = context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(p => typeof(T).GetProperty(p.Name))
                .Select(p => p.GetValue(disconnectedEntity)).ToArray();

            var connectedEntity = context.Find<T>(keyValues);

            return connectedEntity;
        }

        public static void UpdateByPrimaryKey<T>(this BaseDBContext context, T disconnectedEntity, Action<T> updateAction) where T : class
        {
            var connectedEntity = context.GetByPrimaryKey(disconnectedEntity);

            updateAction(connectedEntity);
        }

        public static void DetachLocalEntityBeforeUpdate<T>(T item, DbContext context, Func<T, bool> predicate) where T : class
        {
            var localItem = context.Set<T>()
                .Local
                .FirstOrDefault(predicate);

            if (localItem != null)
            {
                // detach previous entities first
                context.Entry(localItem).State = EntityState.Detached;
            }
        }
    }
}
