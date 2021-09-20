using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GenericSqlBuilder
{
    public class Statement : IStatement
    {
        private List<string> _statements;
        private SqlBuilderOptions _sqlBuilderOptions;
        protected bool StatementIsComplete = true;
        protected StatementType StatementType;

        public Statement(SqlBuilderOptions sqlBuilderOptions)
        {
            _sqlBuilderOptions = sqlBuilderOptions;
            _statements = new List<string>();
        }
        
        public Statement(List<string> statements, SqlBuilderOptions sqlBuilderOptions)
        {
            _statements = statements;
            _sqlBuilderOptions = sqlBuilderOptions;
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

            _sqlBuilderOptions.ClearAll();
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
            _sqlBuilderOptions.IsAppendStatement = true;
            return new SqlBuilder(_sqlBuilderOptions, GenerateSqlStatement() + " ");
        }
        
        public Statement Append(string statement)
        {
            AddStatement(statement + " ");
            return this;
        }

        public string Build()
        {
            ShouldThrowOnInvalidStatement();
            return GenerateSqlStatement();
        }

        private void ShouldThrowOnInvalidStatement()
        {
            if (StatementType == StatementType.Delete)
            {
                if (!StatementIsComplete)
                {
                    throw new InvalidSqlStatementException(
                        "Your delete statement syntax is incorrect. You need to define a FROM and a WHERE clause.");
                }
            }
        }
    }
}