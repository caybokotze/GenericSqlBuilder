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
                            .Select<Person>(o => o.IgnoreProperty(nameof(Person.Name)))
                            .Build(For.MySql);
                        // assert
                        Expect(sql).To.Equal("SELECT id, surname, email;");
                    }
                    
                    [Test]
                    public void ShouldBuildSelectWithMultipleRemovedProperties()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.IgnoreProperty(nameof(Person.Name));
                                o.IgnoreProperty(nameof(Person.Surname));
                            })
                            .Build(For.MySql);
                        // assert
                        Expect(sql).To.Equal("SELECT id, email;");
                    }
                    
                    [Test]
                    public void ShouldBuildSelectWithMultipleRemovedPropertiesAndJoinStatement()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.IgnoreProperty(nameof(Person.Name));
                                o.IgnoreProperty(nameof(Person.Surname));
                            })
                            .From("people")
                            .LeftJoin("customers").On("people.id").Equals("customers.id")
                            .Where("id")
                            .Equals("@Id")
                            .Build(For.MySql);
                        // assert
                        Expect(sql).To.Equal("SELECT id, email FROM people LEFT JOIN customers ON people.id = customers.id WHERE id = @Id;");
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
                    [Test]
                    public void ShouldConvertPropertiesToLowerCaseByDefault()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>()
                            .Build(For.MySql);
                        // assert
                        Expect(sql).To.Equal("SELECT id, name, surname, email;");
                    }
                    
                    [Test]
                    public void ShouldConvertPropertiesToUppercase()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.UsePropertyCase(Casing.UpperCase);
                            })
                            .Build(For.MySql);
                        // assert
                        Expect(sql).To.Equal("SELECT ID, NAME, SURNAME, EMAIL;");
                    }
                }
            }
        }

        [TestFixture]
        public class StatementTests
        {
            
        }
    }
}