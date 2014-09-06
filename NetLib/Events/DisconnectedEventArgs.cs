// DisconnectedEventArgs.cs
// <copyright file="DisconnectedEventArgs.cs"> This code is protected under the MIT License. </copyright>
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
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectedEventArgs" /> class.
        /// </summary>
        /// <param name="remote"> The remote IP address which became disconnected. </param>
        public DisconnectedEventArgs(IPAddress remote)
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
