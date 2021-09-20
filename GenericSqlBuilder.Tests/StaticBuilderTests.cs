using NExpect;
using NUnit.Framework;
using static GenericSqlBuilder.StaticSqlBuilder;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    public class StaticBuilderTests
    {
        public class WhenImportingSqlBuilderStatically
        {
            [Test]
            public void ShouldBuildStatement()
            {
                // arrange
                var expected = "SELECT Name FROM people;";
                // act
                var sql = Sql()
                    .Select("Name")
                    .From("people")
                    .Build();
                // assert
                Expect(sql).To.Equal(expected);
            }
        }
    }
}