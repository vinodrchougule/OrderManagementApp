using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrderManagementApp.Common.Data
{
    public interface IDbConnectionFactory
    {
        public IDbConnection CreateConnection();
    }
}
