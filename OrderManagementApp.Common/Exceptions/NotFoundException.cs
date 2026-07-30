using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string EntityName, object Key) : base($"{EntityName} with key {Key} not found.") { }
        public NotFoundException(string EntityName, string KeyName, object Key) : base($"{EntityName} with {KeyName} {Key} not found.") { }
    }
}
