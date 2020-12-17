namespace FunctionApp3
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Cds.Client;

    public class CdsServiceClientPool : IDisposable
    {
        private const int DefaultPoolSize = 32;
        private int _maxSize;
        private CdsServiceClient _defaultClient;
        private int _count;
        private readonly ILogger<CdsServiceClientPool> _logger;

        private readonly ConcurrentQueue<CdsServiceClient> _pool = new ConcurrentQueue<CdsServiceClient>();

        public CdsServiceClientPool(CdsServiceClient cdsServiceClient, ILogger<CdsServiceClientPool> logger, int maxPoolSize = DefaultPoolSize)

        {
            _defaultClient = cdsServiceClient;
            _logger = logger;
            _maxSize = maxPoolSize;
        }

        public sealed class Lease : IDisposable
        {
            private CdsServiceClientPool _pool;

            public Lease(CdsServiceClientPool pool)
            {
                _pool = pool;

                Client = _pool.Rent();
            }

            public CdsServiceClient Client { get; private set; }

            void IDisposable.Dispose()
            {
                if (_pool != null)
                {
                    if (!_pool.Return(Client))
                    {
                        Client.Dispose();
                    }

                    _pool = null;
                    Client = null;
                }
            }
        }

        public CdsServiceClient Rent()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CdsServiceClientPool));
            }

            if (_pool.TryDequeue(out var client))
            {
                Interlocked.Decrement(ref _count);

                return client;
            }

            if (_defaultClient.IsReady)
            {
                _logger.LogInformation("Cloning CdsServiceClient");
                var svcClientClone = _defaultClient.Clone();

                svcClientClone.SessionTrackingId = Guid.NewGuid();

                return svcClientClone;
            }

            _logger.LogError(_defaultClient.LastCdsException, $"CdsServiceClient is 'Not Ready'.  Reason: {_defaultClient.LastCdsError}");
            throw new Exception($"CdsServiceClient is 'Not Ready'.  Reason: {_defaultClient.LastCdsError}", _defaultClient.LastCdsException);
        }

        public bool Return(CdsServiceClient client)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CdsServiceClientPool));
            }

            if (Interlocked.Increment(ref _count) <= _maxSize)
            {
                ResetState(client);

                _pool.Enqueue(client);

                return true;
            }

            Interlocked.Decrement(ref _count);

            return false;
        }

        private void ResetState(CdsServiceClient cdsServiceClient)
        {
            cdsServiceClient.CallerId = _defaultClient.CallerId;
            cdsServiceClient.MaxRetryCount = _defaultClient.MaxRetryCount;
            cdsServiceClient.RetryPauseTime = _defaultClient.RetryPauseTime;
            //cdsServiceClient.CallerAADObjectId = _defaultClient.CallerAADObjectId;
            //cdsServiceClient.SessionTrackingId = _defaultClient.SessionTrackingId;
        }

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _maxSize = 0;

                    while (_pool.TryDequeue(out var client))
                    {
                        client.Dispose();
                    }

                    if (_defaultClient != null)
                    {
                        _defaultClient.Dispose();
                        _defaultClient = null;
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
            => Dispose(true);

        #endregion IDisposable Support
    }
}