namespace ByteDBServer.Core.Server.Connection.Models
{
    /// <summary>
    /// Inerface for object validation.
    /// </summary>
    internal interface IByteDBValidator<TValidation>
    {
        /// <summary>
        /// Base method used to check if <paramref name="value"/> is valid to <typeparamref name="TValidation"/>.
        /// </summary>
        public bool Validate(TValidation value);
    }
}
