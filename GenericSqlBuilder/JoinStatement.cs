using System.Collections.Generic;
using System.Threading;

namespace GenericSqlBuilder
{
    public class JoinStatement : IStatement
    {
        private List<string> _statements;
        public JoinStatement(List<string> statements)
        {
            _statements = statements;
        }

        public Statement On(string property)
        {
            AddStatement($"ON {property} ");
            return new Statement(_statements);
        }

        private void AddStatement(string statement)
        {
            var semaphore = new SemaphoreSlim(1, 1);
            semaphore.Wait();
            _statements.Add(statement);
            semaphore.Release();
        }
    }
}