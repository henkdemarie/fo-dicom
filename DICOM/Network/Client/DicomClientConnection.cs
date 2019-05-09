using System;
using System.Threading.Tasks;

namespace Dicom.Network.Client
{
    public interface IDicomClientConnection : IDisposable
    {
        /// <summary>
        /// Gets the network stream of this connection
        /// </summary>
        INetworkStream NetworkStream { get; }

        /// <summary>
        /// Gets the long running listener task that waits for incoming DICOM communication from the server.
        /// </summary>
        Task Listener { get; }

        /// <summary>
        /// Opens a long running listener task that waits for incoming DICOM communication
        /// </summary>
        void StartListener();

        /// <summary>
        /// Send association request.
        /// </summary>
        /// <param name="association">DICOM association.</param>
        Task SendAssociationRequestAsync(DicomAssociation association);

        /// <summary>
        /// Send association release request.
        /// </summary>
        Task SendAssociationReleaseRequestAsync();

        /// <summary>
        /// Send abort request.
        /// </summary>
        /// <param name="source">Abort source.</param>
        /// <param name="reason">Abort reason.</param>
        Task SendAbortAsync(DicomAbortSource source, DicomAbortReason reason);

        /// <summary>
        /// Callback for handling association accept scenarios.
        /// </summary>
        /// <param name="association">Accepted association.</param>
        Task OnReceiveAssociationAcceptAsync(DicomAssociation association);

        /// <summary>
        /// Callback for handling association reject scenarios.
        /// </summary>
        /// <param name="result">Specification of rejection result.</param>
        /// <param name="source">Source of rejection.</param>
        /// <param name="reason">Detailed reason for rejection.</param>
        Task OnReceiveAssociationRejectAsync(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason);

        /// <summary>
        /// Callback on response from an association release.
        /// </summary>
        Task OnReceiveAssociationReleaseResponseAsync();

        /// <summary>
        /// Callback on receiving an abort message.
        /// </summary>
        /// <param name="source">Abort source.</param>
        /// <param name="reason">Detailed reason for abort.</param>
        Task OnReceiveAbortAsync(DicomAbortSource source, DicomAbortReason reason);

        /// <summary>
        /// Callback when connection is closed.
        /// </summary>
        /// <param name="exception">Exception, if any, that forced connection to close.</param>
        Task OnConnectionClosedAsync(Exception exception);

        /// <summary>
        /// Send request from service.
        /// </summary>
        /// <param name="request">Request to send.</param>
        Task SendRequestAsync(DicomRequest request);

        /// <summary>
        /// Sometimes, DICOM requests can be enqueued but not immediately sent. This can happen for the following reasons:
        ///   -- The same DicomService is already sending other requests
        ///   -- The active association is temporarily saturated (too many open pending requests), see <see cref="DicomAssociation.MaxAsyncOpsInvoked"/>
        /// </summary>
        Task SendNextMessageAsync();

        /// <summary>
        /// Callback when a request has been completed (a final response was received, causing it to be removed from the pending queue)
        /// </summary>
        /// <param name="request">The original request that was sent, which has now been fulfilled.</param>
        /// <param name="response">The final response from the DICOM server</param>
        Task OnRequestCompletedAsync(DicomRequest request, DicomResponse response);
    }

    public class DicomClientConnection : DicomService, IDicomClientConnection
    {
        private DicomClient DicomClient { get; }

        public INetworkStream NetworkStream { get; }
        public Task Listener { get; private set; }

        public DicomClientConnection(DicomClient dicomClient, INetworkStream networkStream)
            : base(networkStream, dicomClient.FallbackEncoding, dicomClient.Logger)
        {
            DicomClient = dicomClient;
            NetworkStream = networkStream;
        }

        public void StartListener()
        {
            if (Listener != null) return;

            Listener = Task.Factory.StartNew(RunAsync, TaskCreationOptions.LongRunning);
        }

        public new Task SendAssociationRequestAsync(DicomAssociation association)
        {
            return base.SendAssociationRequestAsync(association);
        }

        public new Task SendAssociationReleaseRequestAsync()
        {
            return base.SendAssociationReleaseRequestAsync();
        }

        public new Task SendAbortAsync(DicomAbortSource source, DicomAbortReason reason)
        {
            return base.SendAbortAsync(source, reason);
        }

        public new Task SendRequestAsync(DicomRequest request)
        {
            return base.SendRequestAsync(request);
        }

        public new Task SendNextMessageAsync()
        {
            return base.SendNextMessageAsync();
        }

        protected override Task OnSendQueueEmptyAsync()
        {
            return DicomClient.OnSendQueueEmptyAsync();
        }

        public Task OnReceiveAssociationAcceptAsync(DicomAssociation association)
        {
            return DicomClient.OnReceiveAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationRejectAsync(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason)
        {
            return DicomClient.OnReceiveAssociationRejectAsync(result, source, reason);
        }

        public Task OnReceiveAssociationReleaseResponseAsync()
        {
            return DicomClient.OnReceiveAssociationReleaseResponseAsync();
        }

        public Task OnReceiveAbortAsync(DicomAbortSource source, DicomAbortReason reason)
        {
            return DicomClient.OnReceiveAbortAsync(source, reason);
        }

        public Task OnConnectionClosedAsync(Exception exception)
        {
            return DicomClient.OnConnectionClosedAsync(exception);
        }

        public Task OnRequestCompletedAsync(DicomRequest request, DicomResponse response)
        {
            return DicomClient.OnRequestCompletedAsync(request, response);
        }
    }
}
