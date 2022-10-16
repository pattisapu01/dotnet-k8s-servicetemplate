using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using System;

namespace Cloud.$ext_safeprojectname$.Domain.Kernel
{
    /// <summary>
    /// Used when dealing with the "core domain" models. This "bag" has to be passed in before "CheckInvariants" is called
    /// </summary>
    public class EntityPropertyBag
    {
        /// Provides calling classes to set the "Integration" service so that *any* "Integration" events that are to be raised as part of "CheckVariants" call are raised.
        /// If this not set, "CheckVariants()" throws
        public string TenantId { get; private set; }
        public string TopicName { get; private set; }
        public string CorrelationId { get; private set; }
        
        public EntityPropertyBag(string topicName, string correlationId)
        {            
            CorrelationId = correlationId;
            TopicName = topicName;
        }
    }
}
