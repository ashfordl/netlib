using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetLib.Events
{
    /// <summary>
    /// Contains event arguments for when the remote becomes disconnected.
    /// </summary>
    public class RemoteDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDisconnectedEventArgs" /> class.
        /// </summary>
        /// <param name="remote"> The remote IP address which became disconnected. </param>
        public RemoteDisconnectedEventArgs(IPAddress remote)
        {
            this.RemoteIP = remote;
        }

        /// <summary>
        /// Gets or sets the IP from which the message was received.
        /// </summary>
        public IPAddress RemoteIP
        {
            get;
            set;
        }
    }
}
