using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string EntityName, object Key) : base($"{EntityName} with key {Key} not found.") { }
    }
}
