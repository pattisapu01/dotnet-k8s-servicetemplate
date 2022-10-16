using System;

namespace Cloud.$ext_safeprojectname$.Domain.Exceptions
{
    /// <summary>
    /// Used for wrapping any service specific exceptions (ex: domain invariants/validations etc)
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
