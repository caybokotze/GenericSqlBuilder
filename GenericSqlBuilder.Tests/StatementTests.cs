using System.Collections.Generic;
using System.Dynamic;
using NExpect;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class StatementTests
    {
        [TestFixture]
        public class BuildMethod
        {
            [Test]
            public void ShouldAppendAndBuildStatement()
            {
                // arrange
                var statement = Create();
                statement.Append("foo");
                // act
                var sql = statement.Build();
                // assert
                Expect(sql).To.Equal("foo;");
            }

            [Test]
            public void ShouldAppendMultipleStatementsAndBuild()
            {
                // arrange
                var statement = Create();
                statement.Append("foo").Append("foo").Append("foo");
                // act
                var sql = statement.Build();
                // assert
                Expect(sql).To.Equal("foo foo foo;");
            }

            [TestFixture]
            public class WhenInitialisedWithStatementList
            {
                [Test]
                public void ShouldAppendAllStatements()
                {
                    // arrange
                    var statementList = new List<string>()
                    {
                        "moo ",
                        "farm "
                    };
                    var statement = Create(statementList);
                    statement.Append("foo");
                    // act
                    var sql = statement.Build();
                    // assert
                    Expect(sql).To.Equal("moo farm foo;");
                }
            }
        }

        public static Statement Create(
            List<string> statements = null, 
            SqlBuilderOptions options = null)
        {
            return new Statement(statements ?? new List<string>(), options ?? new SqlBuilderOptions());
        }
    }
}
