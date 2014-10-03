// ConnectedEventArgs.cs
// <copyright file="ConnectedEventArgs.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetLib.Events
{
    /// <summary>
    /// Contains event arguments for when the remote connects.
    /// </summary>
    public class ConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedEventArgs" /> class.
        /// </summary>
        /// <param name="remote"> The remote IP address which became connected. </param>
        public ConnectedEventArgs(IPAddress remote)
        {
            this.RemoteIP = remote;
        }

        /// <summary>
        /// Gets or sets the remote IP.
        /// </summary>
        public IPAddress RemoteIP
        {
            get;
            set;
        }
    }
}
