using System;

namespace Kmd.Logic.FileSecurity.Client.Types
{
    /// <summary>
    /// Exception class to handle file security related errors.
    /// </summary>
    [Serializable]
    public class FileSecurityException : Exception
    {
        /// <summary>
        /// Gets InnerMessage.
        /// </summary>
        public string InnerMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSecurityException"/> class.
        /// </summary>
        public FileSecurityException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSecurityException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public FileSecurityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSecurityException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerMessage">Inner exception message.</param>
        public FileSecurityException(string message, string innerMessage)
            : base(message)
        {
            this.InnerMessage = innerMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSecurityException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception message.</param>
        public FileSecurityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
