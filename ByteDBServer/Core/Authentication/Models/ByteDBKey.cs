using System;
using ByteDBServer.Core.Authentication;
using System.Threading.Tasks;

namespace ByteDBServer.Core.Authentication.Models
{
    internal class ByteDBKey : IDisposable
    {
        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBKey(byte[] key, TimeSpan time) 
        { ByteBDServerAuthenticator.ActiveKeys.Add(this); ExpireDate = DateTime.Now + time; Key = key; DisposalTask = CreateDisposalTask(time); }

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public byte[] Key { get; }
        public DateTime ExpireDate { get; }
        public Task DisposalTask { get; }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        private Task CreateDisposalTask(TimeSpan time)
        {
            DateTime ending = DateTime.Now + time;

            return Task.Run(async () =>
            {
                await Task.Delay(time).ConfigureAwait(false);
                Dispose();
            });
        }

        //
        // ----------------------------- DISPOSING ----------------------------- 
        //

        private bool _disposed = false;
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ByteBDServerAuthenticator.ActiveKeys.Remove(this);
                    Array.Clear(Key, 0, Key.Length);
                    DisposalTask.Dispose();
                }

                _disposed = true;
            }
        }
        ~ByteDBKey()
        {
            Dispose(false);
        }
    }
}
