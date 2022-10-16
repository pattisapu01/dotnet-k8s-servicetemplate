using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cloud.$ext_safeprojectname$.Events
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            Changes = new ChangeLog();
        }
        [System.Text.Json.Serialization.JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
            Changes = new ChangeLog();
        }

        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// If the "item" has to go into a "specific" partition on the service bus, this 'virtual' method can be overridden by the derived event type to provide the 'PartitionKey'
        /// </summary>
        /// <returns></returns>
        public virtual string GetPartitionId()
        {
            return "0";
        }
        /// <summary>
        /// Clients can override this if they want to publish the message with a specific key. Brokers like "kafka" allow for key-value publishing
        /// where "key" determines the partition id the message goes to
        /// </summary>
        /// <returns></returns>
        public virtual string GetKeyForEventBus()
        {
            return null;
        }

        /// <summary>
        /// Contains the 'old' instance of the Integration event. If "Insert", this will be blank.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public IntegrationEvent OldInstance { get; protected set; }
        public ChangeLog Changes { get; protected set; }

        /// <summary>
        /// This function will be be called from IntegrationEventService.AddAndSaveEventAsync(). This function inspects the incoming properties and constraucts the
        /// diff dictionary if the attribute MessageBusDiffTracker is used on an attribute
        /// </summary>
        /// <param name="propertiesToDiff"></param>
        /// <param name="evt"></param>
        public void ConstructDiffPayload(List<PropertyInfo> propertiesToDiff, IntegrationEvent evt)
        {
            foreach (var prop in propertiesToDiff)
            {
                if (prop.PropertyType == typeof(string) || prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double) ||
                    prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Int64) || prop.PropertyType == typeof(DateTime))
                {
                    if (evt.OldInstance == null)
                        continue;
                    object newValue = prop != null ? prop.GetValue(evt, null) : null;
                    if (newValue == null)
                        continue;
                    //check old value
                    object oldValue = prop != null ? prop.GetValue(evt.OldInstance, null) : null;
                    if (oldValue?.ToString() == newValue?.ToString())
                        continue;
                    Changes.OldValues.Add(prop.Name, oldValue?.ToString());
                }
            }
        }
        /// <summary>
        /// If any "properties" are decorated with MessageBusDiffTracker attribute, the 'OldInstance" of the "Integrationevent" must be set if "diffs" are needed in "Changes" property in your Integrationevent payload
        /// This can be called from your Entity.CheckInVariants() function
        /// </summary>
        /// <param name="IntegrationEvent"></param>
        public void SetOldInstance(IntegrationEvent IntegrationEvent)
        {
            OldInstance = IntegrationEvent;
        }

        /// <summary>
        /// Identifies the "message source. Useful to prevent "cyclic" update scenarios.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Used by IntegrationEventHandlers to start a transaction when BaseTransactionBehavior executes as part of processing Kafka messages.
        /// Note: We create a new 'scoped' DI context in BaseKafkaPoller when messages are received so as to resolve the dependencies again.
        /// </summary>
        public bool AreTransactionsEnabled { get; set; }
    }

    public record ChangeLog
    {
        public ChangeLog()
        {
            OldValues = new Dictionary<string, string>();
        }
        public enum ChangeType
        {
            Add,
            Update,
            Delete
        }

        /// <summary>
        /// Contains the "Field Name" and the "Old Value" as each DictionaryItem
        /// </summary>
        public Dictionary<string, string> OldValues { get; set; }
    }
}
