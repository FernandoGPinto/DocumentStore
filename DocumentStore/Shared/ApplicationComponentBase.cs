using Microsoft.AspNetCore.Components;
using System.Threading;
using System;

namespace DocumentStore.Shared
{
    public abstract class ApplicationComponentBase : ComponentBase, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken => (_cancellationTokenSource ??= new()).Token;

        public void Dispose()
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
