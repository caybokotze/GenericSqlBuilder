using System;

namespace GenericSqlBuilder
{
    public class InvalidSqlStatementException : Exception
    {
        public InvalidSqlStatementException() : base("The statement specified is invalid")
        {
            
        }
        public InvalidSqlStatementException(string message) : base(message) 
        {
            
        }
    }
}