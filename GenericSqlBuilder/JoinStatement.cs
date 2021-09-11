using System.Collections.Generic;
using System.Threading;

namespace GenericSqlBuilder
{
    public class JoinStatement : IStatement
    {
        private List<string> _statements;
        private readonly SqlBuilderOptions _options;

        public JoinStatement(List<string> statements, SqlBuilderOptions options)
        {
            _statements = statements;
            _options = options;
        }

        public SelectStatement On(string property)
        {
            AddStatement($"ON {property} ");
            return new SelectStatement(_statements, _options);
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