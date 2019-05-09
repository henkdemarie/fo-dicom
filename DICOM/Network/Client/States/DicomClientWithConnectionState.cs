using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dicom.Network.Client.States
{
    public abstract class DicomClientWithConnectionState : IDicomClientState
    {
        protected DicomClientWithConnectionState(IInitialisationWithConnectionParameters parameters)
        {
            Connection = parameters.Connection ?? throw new ArgumentNullException(nameof(IInitialisationWithConnectionParameters.Connection));
        }

        public IDicomClientConnection Connection { get; }

        public abstract Task OnReceiveAssociationAcceptAsync(DicomAssociation association);

        public abstract Task OnReceiveAssociationRejectAsync(DicomRejectResult result, DicomRejectSource source, DicomRejectReason reason);

        public abstract Task OnReceiveAssociationReleaseResponseAsync();

        public abstract Task OnReceiveAbortAsync(DicomAbortSource source, DicomAbortReason reason);

        public abstract Task OnConnectionClosedAsync(Exception exception);

        public abstract Task OnSendQueueEmptyAsync();

        public abstract Task OnRequestCompletedAsync(DicomRequest request, DicomResponse response);

        public abstract Task OnEnterAsync(CancellationToken cancellationToken);

        public abstract void AddRequest(DicomRequest dicomRequest);

        public abstract Task SendAsync(CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task AbortAsync();
    }
}
