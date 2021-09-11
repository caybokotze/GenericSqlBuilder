using System;

namespace GenericSqlBuilder
{
    public class SqlBuilder
    {
        private SqlBuilderOptions _sqlBuilderOptions;
        private string _sql;

        public SqlBuilder()
        {
            _sqlBuilderOptions = new SqlBuilderOptions();
            _sql = "";
        }
        
        public SqlBuilder(SqlBuilderOptions options, string sql)
        {
            _sqlBuilderOptions = options;
            _sql = sql;
        }
        
        public SelectStatement Select()
        {
            return new SelectStatement(_sql, _sqlBuilderOptions);
        }

        public SelectStatement SelectAll()
        {
            _sql += "* ";
            return new SelectStatement(_sql, _sqlBuilderOptions);
        }

        public SelectStatement Select(string properties)
        {
            _sql += properties + " ";
            return new SelectStatement(_sql, _sqlBuilderOptions);
        }

        public SelectStatement Select<T>() where T : class, new()
        {
            var selectStatement = new SelectStatement(_sql, _sqlBuilderOptions);
            selectStatement.CreateSelectStatement<T>();
            return selectStatement;
        }

        public SelectStatement Select<T>(Action<ISelectOptions> options) where T : class, new()
        {
            options(_sqlBuilderOptions);
            return Select<T>();
        }

        public InsertStatement Insert()
        {
            return new InsertStatement(_sql, _sqlBuilderOptions);
        }

        public InsertStatement Insert(string statement)
        {
            _sql += statement + " ";
            return new InsertStatement(_sql, _sqlBuilderOptions);
        }

        public InsertStatement Insert<T>()
        {
            var insertStatement = new InsertStatement(_sql, _sqlBuilderOptions);
            insertStatement.GenerateInsertProperties<T>();
            return insertStatement;
        }

        public InsertStatement Insert<T>(Action<IInsertOptions> options)
        {
            options(_sqlBuilderOptions);
            return Insert<T>();
        }
    }
}