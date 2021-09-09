using System;
using System.Collections.Generic;
using System.Threading;

namespace GenericSqlBuilder
{

    public class Test
    {

        public int Name { get; set; }
        void Foo()
        {
            new SqlBuilder()
                .Select<Test>(s =>
                {
                    s.IgnoreProperty(nameof(FooMoo.Age));
                })
                .From("shipments")
                .Where("id")
                .Equals("@Id")
                .And("name")
                .Like("@Name");
        }
    }

    public class FooMoo
    {
        public int Age { get; set; }
    }
    public class SqlBuilder
    {
        public SelectStatement Select(string properties)
        {
            return new SelectStatement(properties);
        }
        
        public SelectStatement Select<T>()
        {
            return new SelectStatement();
        }

        public SelectStatement Select<T>(Action<SqlStatementOptions<T>> options) where T : new()
        {
            var selectStatementOptions = new SqlStatementOptions<T>();
            options(selectStatementOptions);
            return new SelectStatement();
        }
    }

    public class SqlStatementOptions<T> where T : new()
    {
        private Casing _casing;
        public List<string> PropertiesToIgnore { get; set; }

        public void UsePropertyCase(Casing casing)
        {
            _casing = casing;
        }

        public T Props()
        {
            return new T();
        }

        public void IgnoreProperty(string name)
        {
            
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

    public class GenericPropertyBuilder
    {
        
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