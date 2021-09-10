using System;

namespace GenericSqlBuilder
{
    public class SqlBuilder
    {
        private SqlStatementOptions _sqlStatementOptions;

        public SqlBuilder()
        {
            _sqlStatementOptions = new SqlStatementOptions();
        }
        
        public SelectStatement Select()
        {
            return new SelectStatement("* ", _sqlStatementOptions);
        }

        public SelectStatement Select(string properties)
        {
            return new SelectStatement(properties, _sqlStatementOptions);
        }

        public SelectStatement Select<T>() where T : class, new()
        {
            var selectStatement = new SelectStatement("", _sqlStatementOptions);
            selectStatement.CreateSelectStatement<T>();
            return selectStatement;
        }

        public SelectStatement Select<T>(Action<SqlStatementOptions> options) where T : class, new()
        {
            var selectStatementOptions = new SqlStatementOptions();
            options(selectStatementOptions);
            _sqlStatementOptions = selectStatementOptions;
            return Select<T>();
        }
    }
}