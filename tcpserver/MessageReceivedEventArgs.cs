﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcpserver
{
    public class MessageReceivedEventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
