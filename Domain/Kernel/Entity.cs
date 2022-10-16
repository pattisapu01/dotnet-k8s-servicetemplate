using Cloud.$ext_safeprojectname$.Domain.Exceptions;
using Cloud.$ext_safeprojectname$.Domain.Helpers;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Domain.Kernel
{
    public abstract class Entity<TID>
    {
        /// <summary>
        /// Indicates whether "All Business Rules" pertaining to the entity are executed. Has to be "true" before the entity is persisted.
        /// </summary>
        [JsonIgnore]
        public bool IsConsistent { get; protected set; }
        /// <summary>
        /// This is another place where business rules and behavior can be implemented. Client needs to call this method on an instance of an 
        /// old entity (oldentity = from DB or passed from client) and pass a"newEntity" as a param.        
        /// if this method is successfull, IsConsistent will be set to "true" and the entity is said to be in a "consistent" state.
        /// </summary>
        /// <param name="newEntity"></param>        
        /// <returns>A dictonary of "Keyrules/bool(rule result) & List of business rules errors if applicable</returns>        
        public virtual Task<Dictionary<string, (EntityUpdateResult, DomainException)>> ValidateEntityRules(Entity<TID> newEntity) { throw new Exception("Has to be overridden"); }

        #region Entity Bag
        /// <summary>
        /// Initializes the entity with the required "interfaces/objects" for entities to run
        /// </summary>
        public virtual void SetEntityBag(EntityPropertyBag entityInitBag)
        {
            EntityBag = entityInitBag;
            IsEntityBagInited = true;
        }
        protected bool IsEntityBagInited { get; set; }
        protected EntityPropertyBag EntityBag { get; set; }
        #endregion

        public DateTime ModifiedDate { get; protected set; }
        int? _requestedHashCode;
        private TID _Id;
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TID Key
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        /// <summary>
        /// Used by Databases that provide "Partitioning/Sharding" for horizontal scaling
        /// </summary>
        [NotMapped]
        public string PartitionId { get; set; }
        /// <summary>
        /// Used by Cosmos DB for Id property (system field)
        /// </summary>
        [NotMapped]
        [JsonProperty("id")]
        public string CosmosId { get; set; }
        private List<INotification> _domainEvents;
        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            if (typeof(TID) == typeof(string))
                return Convert.ToString(this.Key?.ToString().Trim()) == default(string);
            else if (typeof(TID) == typeof(int))
                return Convert.ToInt32(this.Key) == default(int);
            else if (typeof(TID) == typeof(Int64))
                return Convert.ToInt64(this.Key) == default(long);
            else if (typeof(TID) == typeof(Guid))
                return Guid.Parse(this.Key?.ToString()) == default(Guid);
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TID>))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity<TID> item = (Entity<TID>)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Key.ToString() == this.Key.ToString();
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Key.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity<TID> left, Entity<TID> right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TID> left, Entity<TID> right)
        {
            return !(left == right);
        }
    }
}