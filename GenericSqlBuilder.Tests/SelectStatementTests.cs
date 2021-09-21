using System.Collections.Generic;
using GenericSqlBuilder.Tests.TestModels;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class SelectStatementTests
    {
        [TestFixture]
        public class WhenBuildingSelectStatements
        {
            [TestFixture]
            public class WithoutGenericBuilder
            {
                [Test]
                public void ShouldReturnSelectAll()
                {
                    // arrange
                    var expected = "SELECT * FROM people WHERE id = @Id;";
                    // act
                    var sql = new SqlBuilder()
                        .SelectAll()
                        .From("people")
                        .Where("id = @Id")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }

                [Test]
                public void ShouldReturnBasicSelect()
                {
                    // arrange
                    var sql = new SqlBuilder()
                        .Select("Id, Name, Surname, Email")
                        .Build();
                    // act
                    // assert
                    Expect(sql).To.Equal("SELECT Id, Name, Surname, Email;");
                }
                
                [TestFixture]
                public class WithLastInsert
                {
                    [Test]
                    public void ShouldReturnSelectLastInserted_ForMySql()
                    {
                        // arrange
                        var sql = new SqlBuilder()
                            .Select()
                            .LastInserted(Version.MySql);
                        // act
                        // assert
                        Expect(sql).To.Equal("SELECT LAST_INSERT_ID();");
                    }

                    [Test]
                    public void ShouldReturnSelectLastInserted_ForMsSql()
                    {
                        // arrange
                        var sql = new SqlBuilder()
                            .Select()
                            .LastInserted(Version.MsSql);
                        // act
                        // assert
                        Expect(sql).To.Equal("SELECT SCOPE_IDENTITY();");
                    }
                }

                [TestFixture]
                public class WithAppending
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
                }
            }
            
            [TestFixture]
            public class WithGenericBuilder
            {
                [TestFixture]
                public class WhenBuildingBasicSelectStatements
                {
                    [Test]
                    public void ShouldReturnBasicSelect()
                    {
                        // arrange
                        var sql = new SqlBuilder()
                            .Select<Person>()
                            .Build();
                        // act
                        // assert
                        Expect(sql).To.Equal("SELECT Id, Name, Surname, Email;");
                    }

                    [TestFixture]
                    public class WhenSpecifyingVersionsOfMySql
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
                                .Select<Person>(o => { o.UseSqlVersion(Version.MySql); })
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
                                .Select<Person>(o => { o.UseSqlVersion(Version.MsSql); })
                                .From("people")
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(statement).To.Equal(expected);
                        }
                    }
                }

                [TestFixture]
                public class WhenBuildingAdvancedSelectStatements
                {
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
                        Expect(sql).To
                            .Equal("SELECT Id, Name, Surname, Email FROM people WHERE id = 1 AND name LIKE %John%;");
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
                        Expect(sql).To
                            .Equal("SELECT Id, Name, Surname, Email FROM people WHERE id = 1 AND name LIKE %John%;");
                    }

                    [TestFixture]
                    public class WithRemovedProperties
                    {
                        [Test]
                        public void ShouldBuildSelectWithRemovedProperties()
                        {
                            // arrange
                            // act
                            var sql = new SqlBuilder()
                                .Select<Person>(o => o.RemoveProperty(nameof(Person.Name)))
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
                                    o.RemoveProperty(nameof(Person.Name));
                                    o.RemoveProperty(nameof(Person.Surname));
                                })
                                .Build();
                            // assert
                            Expect(sql).To.Equal("SELECT Id, Email;");
                        }
                        
                        [Test]
                        public void ShouldBuildSelectWithMultipleRemovedPropertiesUsingMultiRemove()
                        {
                            // arrange
                            // act
                            var sql = new SqlBuilder()
                                .Select<Person>(o =>
                                {
                                    o.RemoveMultipleProperties(new List<string>
                                    {
                                        nameof(Person.Name),
                                        nameof(Person.Surname)
                                    });
                                })
                                .Build();
                            // assert
                            Expect(sql).To.Equal("SELECT Id, Email;");
                        }

                        [Test]
                        public void ShouldBuildSelectWithMultipleRemovedPropertiesAndLeftJoinStatement()
                        {
                            // arrange
                            var expected =
                                "SELECT Id, Email FROM people LEFT JOIN customers ON people.id = customers.id WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Select<Person>(o =>
                                {
                                    o.RemoveProperty(nameof(Person.Name));
                                    o.RemoveProperty(nameof(Person.Surname));
                                })
                                .From("people")
                                .LeftJoin("customers").On("people.id").Equals("customers.id")
                                .Where("id")
                                .Equals("@Id")
                                .Build();
                            // assert
                            Expect(sql).To
                                .Equal(expected);
                        }
                        
                        [Test]
                        public void ShouldBuildSelectWithMultipleRemovedPropertiesAndInnerJoinStatement()
                        {
                            // arrange
                            var expected =
                                "SELECT Id, Email FROM people INNER JOIN customers ON people.id = customers.id WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Select<Person>(o =>
                                {
                                    o.RemoveProperty(nameof(Person.Name));
                                    o.RemoveProperty(nameof(Person.Surname));
                                })
                                .From("people")
                                .InnerJoin("customers").On("people.id").Equals("customers.id")
                                .Where("id")
                                .Equals("@Id")
                                .Build();
                            // assert
                            Expect(sql).To
                                .Equal(expected);
                        }
                        
                        [Test]
                        public void ShouldBuildSelectWithMultipleRemovedPropertiesAndRightJoinStatement()
                        {
                            // arrange
                            var expected =
                                "SELECT Id, Email FROM people RIGHT JOIN customers ON people.id = customers.id WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Select<Person>(o =>
                                {
                                    o.RemoveProperty(nameof(Person.Name));
                                    o.RemoveProperty(nameof(Person.Surname));
                                })
                                .From("people")
                                .RightJoin("customers").On("people.id").Equals("customers.id")
                                .Where("id")
                                .Equals("@Id")
                                .Build();
                            // assert
                            Expect(sql).To
                                .Equal(expected);
                        }
                    }

                    [TestFixture]
                    public class WithAddedProperties
                    {
                        [Test]
                        public void SelectStatementShouldGenerateSelectAndAddProperties()
                        {
                            // arrange
                            var expected = "SELECT Age, DoesWalk, Height FROM animals WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Select<Animal>(o => o.AddProperty("Height"))
                                .From("animals")
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                        
                        [Test]
                        public void SelectStatementShouldGenerateSelectAndAddMultipleProperties()
                        {
                            // arrange
                            var expected = "SELECT Age, DoesWalk, Height, Width, Weight FROM animals WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Select<Animal>(o =>
                                {
                                    o.AddProperty("Height");
                                    o.AddProperty("Width");
                                    o.AddProperty("Weight");
                                })
                                .From("animals")
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                    }

                    [TestFixture]
                    public class WithCustomCasingOptions
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
                                .Select<Person>(o => { o.UsePropertyCase(Casing.UpperCase); })
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
                                .Select<Person>(o => { o.UsePropertyCase(Casing.LowerCase); })
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
                                .Select<Animal>(o => { o.UsePropertyCase(Casing.KebabCase); })
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
                                .Select<Animal>(o => { o.UsePropertyCase(Casing.CamelCase); })
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
                                .Select<Animal>(o => { o.UsePropertyCase(Casing.SnakeCase); })
                                .Build();
                            // assert
                            Expect(sql).To.Equal("SELECT age, does_walk;");
                        }
                    }

                    [TestFixture]
                    public class WithAppendSelect
                    {
                        [Test]
                        public void ShouldAppendSelectStatement()
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
                                    .Select<Person>(o => { o.AddAlias("p"); })
                                    .AppendSelect<Animal>(o => { o.AddAlias("a"); })
                                    .From("people as p")
                                    .LeftJoin("animals as a")
                                    .On("p.Id = a.Id")
                                    .Build();
                                // assert
                                Expect(sql).To.Equal(expected);
                            }
                            
                            [Test]
                            public void ShouldAppendAliasWithConsistentCasing()
                            {
                                // arrange
                                var expected =
                                    "SELECT p.id, p.name, p.surname, p.email, a.age, a.doesWalk FROM people as p LEFT JOIN animals as a ON p.Id = a.Id;";
                                // act
                                var sql = new SqlBuilder()
                                    .Select<Person>(o =>
                                    {
                                        o.AddAlias("p");
                                        o.UsePropertyCase(Casing.CamelCase);
                                    })
                                    .AppendSelect<Animal>(o => { o.AddAlias("a"); })
                                    .From("people as p")
                                    .LeftJoin("animals as a")
                                    .On("p.Id = a.Id")
                                    .Build();
                                // assert
                                Expect(sql).To.Equal(expected);
                            }
                            
                            [Test]
                            public void ShouldAppendAliasAndOverrideCasingWithSecondOption()
                            {
                                // arrange
                                var expected =
                                    "SELECT p.id, p.name, p.surname, p.email, a.AGE, a.DOESWALK FROM people as p LEFT JOIN animals as a ON p.Id = a.Id;";
                                // act
                                var sql = new SqlBuilder()
                                    .Select<Person>(o =>
                                    {
                                        o.AddAlias("p");
                                        o.UsePropertyCase(Casing.CamelCase);
                                    })
                                    .AppendSelect<Animal>(o =>
                                    {
                                        o.AddAlias("a");
                                        o.UsePropertyCase(Casing.UpperCase);
                                    })
                                    .From("people as p")
                                    .LeftJoin("animals as a")
                                    .On("p.Id = a.Id")
                                    .Build();
                                // assert
                                Expect(sql).To.Equal(expected);
                            }
                            
                            [Test]
                            public void ShouldAppendCustomProperty()
                            {
                                // arrange
                                var expected =
                                    "SELECT p.id, p.name, p.surname, p.email, a.AGE, a.DOESWALK FROM people as p LEFT JOIN animals as a ON p.Id = a.Id;";
                                // act
                                var sql = new SqlBuilder()
                                    .Select<Person>(o =>
                                    {
                                        o.AddAlias("p");
                                        o.UsePropertyCase(Casing.CamelCase);
                                    })
                                    .AppendSelect<Animal>(o =>
                                    {
                                        o.AddAlias("a");
                                        o.UsePropertyCase(Casing.UpperCase);
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
                }
            }
        }
    }
}