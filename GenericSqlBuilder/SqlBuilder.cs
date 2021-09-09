using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GenericSqlBuilder
{

    public class Test
    {

        public int Name { get; set; }
        void Foo()
        {
            new SqlBuilder()
                .Select<Test>(o =>
                {
                    o.UsePropertyCase(Casing.PascalCase);
                    o.IgnoreProperty(nameof(FooMoo.Age));
                })
                .From("shipments")
                .Where("id")
                .Equals("@Id")
                .And("name")
                .Like("@Name")
                .Build(For.MySql);
        }
    }

    public class FooMoo
    {
        public int Age { get; set; }
    }
    public class SqlBuilder
    {
        private string _sql;

        public SqlBuilder()
        {
            _sql = new string("");
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
            var ignoredProperties = selectStatementOptions.FetchIgnoredProperties();
            _sql = CreateSelectStatement<T>(ignoredProperties);
            return Select(_sql);
        }

        private string CreateSelectStatement<T>(List<string> remove = null)
        {
            var dataTable = GenericPropertyBuilder<T>.GetGenericProperties();
            if (remove != null && remove.Count > 0)
            {
                foreach (var item in remove)
                {
                    dataTable.Columns.Remove(item);
                }
            }

            var variables = dataTable.Columns
                .Cast<object>()
                .Aggregate("", (current, _) => string.Join(", ", current));

            variables = variables.Remove(variables.Length, -2);

            return $"({variables})";
        }
    }

    public class SqlStatementOptions
    {
        public SqlStatementOptions()
        {
            _propertiesToIgnore = new List<string>();
        }
        
        private Casing _casing;
        private readonly List<string> _propertiesToIgnore;

        public void UsePropertyCase(Casing casing)
        {
            _casing = casing;
        }

        public List<string> FetchIgnoredProperties()
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
    

    public class Statement
    {
        private List<string> _statements;

        protected Statement()
        {
            _statements = new List<string>();
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
            return "";
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
    }

    public class SelectStatement : Statement
    {
        public SelectStatement(string properties)
        {
            AddStatement("SELECT ");
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