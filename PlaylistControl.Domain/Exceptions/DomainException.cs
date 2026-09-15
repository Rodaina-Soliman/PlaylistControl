using System;
using System.Collections.Generic;
using System.Text;

namespace PlaylistControl.Domain.Exceptions
{
    /// <summary>
    /// Exception specific to Domain Errors
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class.
        /// </summary>
        /// <param name="message">Message that specifies error details</param>
        public DomainException(string? message) : base(message) { }
    }
}
