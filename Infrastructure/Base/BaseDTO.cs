using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Microsoft.AspNetCore.Http;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Base
{
    /// <summary>
    /// Use this class as base if "entity" DTO participates in Update
    /// </summary>
    public class BaseUpdateDTO : BaseDTO
    {
        /// <summary>
        /// This is a client supplied value
        /// </summary>
        public EntityUpdateMode UpdateMode { get; set; }
    }

    public class BaseDTO
    {
        public string APIMessage { get; set; }
    }
}
