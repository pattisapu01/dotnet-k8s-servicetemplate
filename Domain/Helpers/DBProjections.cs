namespace Cloud.$ext_safeprojectname$.Domain.Helpers
{
    /// <summary>
    /// Returns a code indicating if an entity was updated in memory
    /// </summary>
    public enum EntityUpdateResult
    {
        Successfull = 1,
        NoRecordFoundOrStaleValue = 2,
        Unchanged = 3,
        BusinessRuleViolation = 4
    }

    /// <summary>
    /// Used when "Updating" entities via the corresponding "Set" properties (As the entities are immutable once created). 
    /// </summary>
    public enum EntityUpdateMode
    {
        /// <summary>
        /// Attaches the incoming entity to the "context" and directly updates a sepcific property with NO comparison to the DB value. 
        /// This means the "update" command will be sent even if the incoming value did not change
        /// </summary>
        NoCheckUpdate = 1,
        /// <summary>
        /// Loads the "latest" record from DB into the DB Context and compares the incoming values to the record from DB and updates
        /// the target property via the entities "set" function. This is a better mode for updates as it encapsulates the entities business rules within
        /// the "set" function and ONLY updates the properties that have changed
        /// </summary>
        UpdateOnlyIfDifferentfromDB = 2,
        /// <summary>
        /// Attaches the incoming entity to the "context" and compares the "oldValue" property in the DTO with the value in database (where clause). 
        /// The record will be loaded only if the "Old" value matches what the client saw
        /// Only if they are same and if incoming value is different from old value, an update will be initiated. 
        /// This provides "concurrency" guarantee that the record was not modified
        /// from the time the client started editing the record to the time it got updated.
        /// </summary>
        UpdateWithStaleValueCheck = 3
    }
}
