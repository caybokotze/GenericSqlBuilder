using System.Collections.Generic;
using System.Linq;

namespace GenericSqlBuilder
{
    public class Statement : IStatement
    {
        private List<string> _statements;
        private SqlBuilderOptions _sqlBuilderOptions;

        public Statement(SqlBuilderOptions options)
        {
            _sqlBuilderOptions = options;
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
            
            if (!_sqlBuilderOptions.IsAppendStatement())
            {
                result += ";";
            }

            if (_sqlBuilderOptions.IsAppendStatement())
            {
                result += ", ";
            }
            
            return result;
        }

        protected void AddStatement(string statement)
        {
            _statements.Add(statement);
        }

        public SqlBuilder Append()
        {
            _sqlBuilderOptions.SetAppendStatement(true);
            return new SqlBuilder(_sqlBuilderOptions, GenerateSqlStatement());
        }
        
        public Statement Append(string statement)
        {
            AddStatement(statement);
            return this;
        }

        public string Build()
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