using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class SqlBuilderTests
    {
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
        }
        
        [TestFixture]
        public class SelectStatementTests
        {
            [Test]
            public void ShouldReturnSelectLastInsertedForMySql()
            {
                // arrange
                var sql = new SqlBuilder()
                    .Select()
                    .LastInserted(For.MySql);
                // act
                // assert
                Expect(sql).To.Equal("SELECT LAST_INSERT_ID();");
            }

            [TestFixture]
            public class GenericBuilder
            {
                [Test]
                public void TestBasicSelect()
                {
                    // arrange
                    var sql = new SqlBuilder()
                        .Select<Person>()
                        .Build(For.MySql);
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT id, name, surname, email;");
                }

                [Test]
                public void ShouldReturnFullSelect()
                {
                    // arrange
                    var sql = new SqlBuilder()
                        .Select<Person>()
                        .From("people")
                        .Where("id")
                        .Equals("1")
                        .Build(For.MySql);
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT id, name, surname, email FROM people WHERE id = 1;");
                }

                [Test]
                public void ShouldReturnSelectWithAndClause()
                {
                    // arrange
                    var sql = new SqlBuilder()
                        .Select<Person>()
                        .From("people")
                        .Where("id")
                        .Equals("1")
                        .And("name")
                        .Like("%John%")
                        .Build(For.MySql);
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT id, name, surname, email FROM people WHERE id = 1;");
                }

                [TestFixture]
                public class IgnorePropertiesTests
                {
                    [Test]
                    public void ShouldBuildSelectWithRemovedProperties()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o => o.IgnoreProperty(nameof(Person.Name)));
                        // assert
                        Expect(sql).To.Equal("")
                    }
                }
                
                [Test]
                public void ShouldBuildSelectWithJoin()
                {
                    // arrange
                    var sql = new SqlBuilder()
                        .Select<Person>()
                        .From("people")
                        .Where("id")
                        .Equals("1")
                        .And("name")
                        .Like("%John%")
                        .Build(For.MySql);
                    // act
                    // assert
                    Expect(sql).To.Equal("");
                }

                [TestFixture]
                public class CasingTests
                {
                    
                }
            }
        }

        [TestFixture]
        public class StatementTests
        {
            
        }
    }
}