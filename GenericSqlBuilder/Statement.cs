using System.Collections.Generic;
using System.Threading;

namespace GenericSqlBuilder
{
    public class Statement : IStatement
    {
        private List<string> _statements;

        protected Statement()
        {
            _statements = new List<string>();
        }
        
        public Statement(List<string> statements)
        {
            _statements = statements;
        }

        protected string GenerateSqlStatement()
        {
            var result = "";
            _statements.ForEach(f =>
            {
                result += string.Join(" ", f);
            });
            result = result.Trim();
            result += ";";
            return result;
        }

        protected void AddStatement(string statement)
        {
            var semaphore = new SemaphoreSlim(1, 1);
            semaphore.Wait();
            _statements.Add(statement);
            semaphore.Release();
        }

        public string Build(Version version)
        {
            return GenerateSqlStatement();
        }
        
        public Statement From(string table)
        {
            AddStatement($"FROM {table} ");
            return this;
        }
        
        public Statement Where(string clause)
        {
            AddStatement($"WHERE {clause} ");
            return this;
        }

        public Statement And(string clause)
        {
            AddStatement($"AND {clause} ");
            return this;
        }

        public Statement And()
        {
            AddStatement($"AND ");
            return this;
        }

        public Statement Equals(string clause)
        {
            AddStatement($"= {clause} ");
            return this;
        }

        public Statement Like(string clause)
        {
            AddStatement($"LIKE {clause} ");
            return this;
        }

        public Statement Is()
        {
            AddStatement("IS ");
            return this;
        }

        public JoinStatement LeftJoin(string table)
        {
            AddStatement($"LEFT JOIN {table} ");
            return new JoinStatement(_statements);
        }

        public JoinStatement RightJoin(string table)
        {
            AddStatement($"RIGHT JOIN {table} ");
            return new JoinStatement(_statements);
        }

        public JoinStatement InnerJoin(string table)
        {
            AddStatement($"INNER JOIN {table} ");
            return new JoinStatement(_statements);
        }
    }
}