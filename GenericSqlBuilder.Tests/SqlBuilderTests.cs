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

        public class Animal
        {
            public int Age { get; set; }
            public bool DoesWalk { get; set; }
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
                    .LastInserted(Version.MySql);
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
                        .Build();
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT Id, Name, Surname, Email;");
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
                        .Build();
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT Id, Name, Surname, Email FROM people WHERE id = 1;");
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
                        .Build();
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT Id, Name, Surname, Email FROM people WHERE id = 1 AND name LIKE %John%;");
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
                            .Select<Person>(o => o.AddIgnoreProperty(nameof(Person.Name)))
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT Id, Surname, Email;");
                    }
                    
                    [Test]
                    public void ShouldBuildSelectWithMultipleRemovedProperties()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.AddIgnoreProperty(nameof(Person.Name));
                                o.AddIgnoreProperty(nameof(Person.Surname));
                            })
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT Id, Email;");
                    }
                    
                    [Test]
                    public void ShouldBuildSelectWithMultipleRemovedPropertiesAndJoinStatement()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.AddIgnoreProperty(nameof(Person.Name));
                                o.AddIgnoreProperty(nameof(Person.Surname));
                            })
                            .From("people")
                            .LeftJoin("customers").On("people.id").Equals("customers.id")
                            .Where("id")
                            .Equals("@Id")
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT Id, Email FROM people LEFT JOIN customers ON people.id = customers.id WHERE id = @Id;");
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
                        .Build();
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT Id, Name, Surname, Email FROM people WHERE id = 1 AND name LIKE %John%;");
                }

                [TestFixture]
                public class CasingTests
                {
                    [Test]
                    public void ShouldNotConvertPropertyCaseByDefault()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>()
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT Id, Name, Surname, Email;");
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
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT ID, NAME, SURNAME, EMAIL;");
                    }
                    
                    [Test]
                    public void ShouldConvertPropertiesToLowercase()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Person>(o =>
                            {
                                o.UsePropertyCase(Casing.LowerCase);
                            })
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT id, name, surname, email;");
                    }
                    
                    [Test]
                    public void ShouldConvertPropertiesToKebabCase()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Animal>(o =>
                            {
                                o.UsePropertyCase(Casing.KebabCase);
                            })
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT age, does-walk;");
                    }
                    
                    [Test]
                    public void ShouldConvertPropertiesToCamelCase()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Animal>(o =>
                            {
                                o.UsePropertyCase(Casing.CamelCase);
                            })
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT age, doesWalk;");
                    }
                    
                    
                    [Test]
                    public void ShouldConvertPropertiesToSnakeCase()
                    {
                        // arrange
                        // act
                        var sql = new SqlBuilder()
                            .Select<Animal>(o =>
                            {
                                o.UsePropertyCase(Casing.SnakeCase);
                            })
                            .Build();
                        // assert
                        Expect(sql).To.Equal("SELECT age, does_walk;");
                    }
                }
            }
        }

        [TestFixture]
        public class SqlAppendTests
        {
            [Test]
            public void ShouldAppendSelectAfterSelect()
            {
                // arrange
                var expected = "SELECT name, surname, email FROM people WHERE id = @Id; SELECT LAST_INSERT_ID();";
                // act
                var sql = new SqlBuilder()
                    .Select("name, surname, email")
                    .From("people")
                    .Where("id")
                    .Equals("@Id")
                    .AppendStatement()
                    .Select()
                    .LastInserted(Version.MySql);
                // assert
                Expect(sql).To.Equal(expected);
            }

            [TestFixture]
            public class AppendWithGenericBuilder
            {
                [Test]
                public void ShouldAppendWithGenericBuilder()
                {
                    // arrange
                    var expected = "SELECT ";
                    // act
                    // assert
                }
            }
        }

        [TestFixture]
        public class SwitchingSqlVersions
        {
            
            [Test]
            public void ShouldSwitchToNoneIfNotSpecified()
            {
                // arrange
                var expected = "SELECT Id, Name, Surname, Email FROM people WHERE id = @Id;";
                // act
                var statement = new SqlBuilder()
                    .Select<Person>()
                    .From("people")
                    .Where("id = @Id")
                    .Build();
                // assert
                Expect(statement).To.Equal(expected);
            }
            
            
            [Test]
            public void ShouldSwitchToMySqlIfMySqlIsSpecified()
            {
                // arrange
                var expected = "SELECT `Id`, `Name`, `Surname`, `Email` FROM people WHERE id = @Id;";
                // act
                var statement = new SqlBuilder()
                    .Select<Person>(o =>
                    {
                        o.UseSqlVersion(Version.MySql);
                    })
                    .From("people")
                    .Where("id = @Id")
                    .Build();
                // assert
                Expect(statement).To.Equal(expected);
            }
            
            [Test]
            public void ShouldSwitchToMsSqlIfMsSqlIsSpecified()
            {
                // arrange
                var expected = "SELECT [Id], [Name], [Surname], [Email] FROM people WHERE id = @Id;";
                // act
                var statement = new SqlBuilder()
                    .Select<Person>(o =>
                    {
                        o.UseSqlVersion(Version.MsSql);
                    })
                    .From("people")
                    .Where("id = @Id")
                    .Build();
                // assert
                Expect(statement).To.Equal(expected);
            }
        }

        [TestFixture]
        public class WhenBuildingComplexAppendedSelectStatements
        {
            [TestFixture]
            public class WithoutAliases
            {
                [Test]
                public void ShouldAppendFirstSelectWithSecondSelect()
                {
                    // arrange
                    var expected = "SELECT Id, Name, Surname, Email, Age, DoesWalk FROM people WHERE id = @Id;";
                    // act
                    var sql = new SqlBuilder()
                        .Select<Person>()
                        .AppendSelect<Animal>()
                        .From("people")
                        .Where("id")
                        .Equals("@Id")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }   
            }

            [TestFixture]
            public class WithAliases
            {
                [Test]
                public void ShouldAppendAliasToSelectStatement()
                {
                    // arrange
                    var expected =
                        "SELECT p.Id, p.Name, p.Surname, p.Email, a.Age, a.DoesWalk FROM people as p LEFT JOIN animals as a ON p.Id = a.Id;";
                    // act
                    var sql = new SqlBuilder()
                        .Select<Person>(o =>
                        {
                            o.AddAlias("p");
                        })
                        .AppendSelect<Animal>(o =>
                        {
                            o.AddAlias("a");
                        })
                        .From("people as p")
                        .LeftJoin("animals as a")
                        .On("p.Id = a.Id")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }
            }
        }

        [TestFixture]
        public class AppendSelectTests
        {
            [TestFixture]
            public class WhenAppendingSelect
            {
                [Test]
                public void ShouldAppendSelectStatement()
                {
                    // arrange
                    var expected = "SELECT id, name, surname, email, age, does_walk FROM people;";
                    // act
                    var sql = new SqlBuilder()
                        .Select<Person>(o =>
                        {
                            o.UsePropertyCase(Casing.SnakeCase);
                        })
                        .AppendSelect<Animal>()
                        .From("people")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }
            }
        }
    }
}