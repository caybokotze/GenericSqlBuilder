using System;

namespace GenericSqlBuilder
{
    public static class StaticSqlBuilder
    {
        public static SqlBuilder Sql()
        {
            return new SqlBuilder();
        }
    }
    
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

        public InsertStatement Insert(string table, string properties)
        {
            _sql += $"{table} ({properties}) ";
            return new InsertStatement(_sql, _sqlBuilderOptions);
        }

        public InsertStatement<T> Insert<T>(string table) where T : class, new()
        {
            _sql += table + " ";
            var insertStatement = new InsertStatement<T>(_sql, _sqlBuilderOptions);
            insertStatement.GenerateInsertProperties();
            return insertStatement;
        }

        public InsertStatement<T> Insert<T>(string table, Action<IInsertOptions> options) where T : class, new()
        {
            options(_sqlBuilderOptions);
            return Insert<T>(table);
        }

        public UpdateStatement Update(string table)
        {
            _sql += table + " ";
            return new UpdateStatement(_sql, _sqlBuilderOptions);
        }

        public UpdateStatement<T> Update<T>(string table) where T : class, new()
        {
            _sql += table + " ";
            var updateStatement = new UpdateStatement<T>(_sql, _sqlBuilderOptions);
            return updateStatement;
        }

        public UpdateStatement<T> Update<T>(string table, Action<IUpdateOptions> options) where T : class, new()
        {
            options(_sqlBuilderOptions);
            return Update<T>(table);
        }

        public DeleteStatement Delete()
        {
            return new DeleteStatement(_sql, _sqlBuilderOptions);
        }
    }
}