using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrderManagementApp.Common.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() : base ("The record was modified by another user. Please reload it and try again.") { }
    }
}
