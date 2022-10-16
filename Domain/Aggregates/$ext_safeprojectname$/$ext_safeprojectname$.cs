using Cloud.$ext_safeprojectname$.Events.Integration;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using Cloud.$ext_safeprojectname$.Domain.Kernel;
using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Domain.Aggregates.$ext_safeprojectname$
{
    public class $ext_safeprojectname$ : Entity<int>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Quantity { get; private set; }
        public double Amount { get; private set; }
        public bool IsActive { get; private set; }        

        public $ext_safeprojectname$(string name, string description, int quantity, double amount, bool isActive)
        {
            Name = name;
            Description = description;
            Quantity = quantity;
            Amount = amount;
            IsActive = isActive;            
        }

        /// <summary>
        /// This is where the developer needs to write the "Business Rules" that make up this "Entity". 
        /// All "state" changes must be checked here. This is also the place where "Domain" & "Integration" events can be raised.
        /// Ex: 
        /// </summary>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        public override async Task<Dictionary<string, (EntityUpdateResult, DomainException)>> ValidateEntityRules(Entity<int> newEntity)
        {                        
            var result = new Dictionary<string, (EntityUpdateResult, DomainException)>();
            var $ext_safeprojectname$ = newEntity as $ext_safeprojectname$;

            //TODO: Before setting the values, do business rule checks here before mutating the entity state
            if (Name != $ext_safeprojectname$.Name)
                Name = $ext_safeprojectname$.Name;
            if (Description != $ext_safeprojectname$.Description)
                Description = $ext_safeprojectname$.Description;            
            if (IsActive != $ext_safeprojectname$.IsActive)
                IsActive = $ext_safeprojectname$.IsActive;
            if (Quantity != $ext_safeprojectname$.Quantity)
                Quantity = $ext_safeprojectname$.Quantity;
            if (Amount != $ext_safeprojectname$.Amount)
                Amount = $ext_safeprojectname$.Amount;
            IsConsistent = true;
            return result;
        }
    }
}
