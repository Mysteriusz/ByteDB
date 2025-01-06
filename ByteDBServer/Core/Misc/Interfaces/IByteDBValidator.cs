namespace ByteDBServer.Core.Server.Connection.Models
{
    internal interface IByteDBValidator<TValidation>
    {
        /// <summary>
        /// Base method used to check if <paramref name="value"/> is valid to <typeparamref name="TValidation"/>.
        /// </summary>
        public bool Validate(TValidation value);
    }
}
