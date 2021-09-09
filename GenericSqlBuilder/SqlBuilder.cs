using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using PeanutButter.Utils;

namespace GenericSqlBuilder
{
    public class SqlBuilder
    {
        private string _sql;
        private SqlStatementOptions _sqlStatementOptions;

        public SqlBuilder()
        {
            _sql = new string("");
            _sqlStatementOptions = new SqlStatementOptions();
        }
        
        public SelectStatement Select()
        {
            return new SelectStatement("");
        }

        public SelectStatement Select(string properties)
        {
            return new SelectStatement(properties);
        }
        
        public SelectStatement Select<T>()
        {
            _sql = CreateSelectStatement<T>();
            return Select(_sql);
        }

        public SelectStatement Select<T>(Action<SqlStatementOptions> options) where T : new()
        {
            var selectStatementOptions = new SqlStatementOptions();
            options(selectStatementOptions);
            _sqlStatementOptions = selectStatementOptions;
            _sql = CreateSelectStatement<T>();
            return Select(_sql);
        }

        private string CreateSelectStatement<T>()
        {
            var remove = _sqlStatementOptions.GetIgnoredProperties();
            var dataTable = GenericPropertyBuilder<T>.GetGenericProperties();
            
            if (remove != null && remove.Count > 0)
            {
                foreach (var item in remove)
                {
                    dataTable.Columns.Remove(item);
                }
            }

            var variableArray = new string[dataTable.Columns.Count];
            for (var i = 0; i < dataTable.Columns.Count; i ++)
            {
                variableArray[i] = _sqlStatementOptions.GetCase() switch
                {
                    Casing.UpperCase => dataTable.Columns[i].ColumnName.ToUpperInvariant(),
                    Casing.LowerCase => dataTable.Columns[i].ColumnName.ToLowerInvariant(),
                    Casing.PascalCase => dataTable.Columns[i].ColumnName.ToPascalCase(),
                    Casing.KebabCase => dataTable.Columns[i].ColumnName.ToKebabCase(),
                    Casing.SnakeCase => dataTable.Columns[i].ColumnName.ToSnakeCase(),
                    _ => dataTable.Columns[i].ColumnName
                };
            }

            return string.Join(", ", variableArray) + " ";
        }
    }

    public class SqlStatementOptions
    {
        public SqlStatementOptions()
        {
            _propertiesToIgnore = new List<string>();
            _casing = Casing.LowerCase;
        }
        
        private Casing _casing;
        private readonly List<string> _propertiesToIgnore;

        public void UsePropertyCase(Casing casing)
        {
            _casing = casing;
        }

        public Casing GetCase()
        {
            return _casing;
        }

        public List<string> GetIgnoredProperties()
        {
            return _propertiesToIgnore;
        }

        public void IgnoreProperty(string name)
        {
            _propertiesToIgnore.Add(name);
        }
    }

    public enum Casing
    {
        UpperCase,
        LowerCase,
        KebabCase,
        SnakeCase,
        PascalCase,
    }

    public static class GenericPropertyBuilder<T>
    {
        public static DataTable GetGenericProperties()
        {
            var type = typeof(T);
            return type.GetProperties()
                .Aggregate(
                    new DataTable(),
                    (acc, cur) =>
                    {
                        acc.Columns.Add(cur.Name,
                            Nullable.GetUnderlyingType(cur.PropertyType)
                            ?? cur.PropertyType);
                        return acc;
                    });
        }
    }
    

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

        public string Build(For @for)
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

    public interface IStatement
    {
        private void AddStatement(string statement) { }
    }

    public class JoinStatement : IStatement
    {
        private List<string> _statements;
        public JoinStatement(List<string> statements)
        {
            _statements = statements;
        }

        public JoinStatement On(string property)
        {
            AddStatement($"ON {property} ");
            return this;
        }

        public Statement Equals(string property)
        {
            AddStatement($"= {property} ");
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

    public class SelectStatement : Statement
    {
        public SelectStatement(string properties)
        {
            AddStatement("SELECT ");
            AddStatement(properties);
        }

        public string LastInserted(For @for)
        {
            AddStatement("LAST_INSERT_ID() ");
            return GenerateSqlStatement();
        }
    }

    public class UpdateStatement : Statement
    {
        public UpdateStatement()
        {
            
        }
    }

    public enum For
    {
        MySql
    }
}