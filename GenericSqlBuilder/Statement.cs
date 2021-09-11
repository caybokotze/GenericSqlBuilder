using System.Collections.Generic;
using System.Linq;

namespace GenericSqlBuilder
{
    public class Statement : IStatement
    {
        private List<string> _statements;
        private SqlBuilderOptions _sqlBuilderSqlBuilderOptions;

        public Statement(SqlBuilderOptions sqlBuilderOptions)
        {
            _sqlBuilderSqlBuilderOptions = sqlBuilderOptions;
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

        protected void AddStatement(string statement, bool trimPrevious = false)
        {
            if (trimPrevious)
            {
                var recentItem = _statements.Last();
                _statements.Remove(recentItem);
                _statements.Add(recentItem.TrimEnd());
            }
            _statements.Add(statement);
        }

        protected List<string> GetStatements()
        {
            return _statements;
        }

        public SqlBuilder AppendStatement()
        {
            _sqlBuilderSqlBuilderOptions.IsAppendStatement = true;
            return new SqlBuilder(_sqlBuilderSqlBuilderOptions, GenerateSqlStatement() + " ");
        }
        
        public Statement Append(string statement)
        {
            AddStatement(statement + " ");
            return this;
        }

        public string Build()
        {
            return GenerateSqlStatement();
        }
    }
}