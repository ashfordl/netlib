// DataReceivedEventArgs.cs
// <copyright file="DataReceivedEventArgs.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;

namespace NetLib.Events
{
    /// <summary>
    /// Contains event arguments for when a message is received.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="message"> The message received. </param>
        /// <param name="origin"> The IP Address from which the message was received. </param>
        public DataReceivedEventArgs(Message message, IPAddress origin)
        {
            this.Message = message;
            this.Origin = origin;
        }

        public Message Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IP from which the message was received.
        /// </summary>
        public IPAddress Origin
        {
            get;
            set;
        }
    }
}
