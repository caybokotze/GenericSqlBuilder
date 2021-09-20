using GenericSqlBuilder.Tests.TestModels;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class UpdateStatementTests
    {
        [TestFixture]
        public class WhenBuildingUpdateStatements
        {
            [TestFixture]
            public class WithGenericBuilder
            {
                [TestFixture]
                public class WithOptions
                {
                    [TestFixture]
                    public class WithCasing
                    {
                        [Test]
                        public void ShouldBuildUpdateStatementWithLowerCaseProperties()
                        {
                            // arrange
                            var expected =
                                "UPDATE people SET id = @Id, name = @Name, surname = @Surname, email = @Email WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Update<Person>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.LowerCase);
                                })
                                .Set()
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }

                        [Test]
                        public void ShouldBuildUpdateStatementWithSnakeCaseProperties()
                        {
                            // arrange
                            var expected =
                                "UPDATE people SET age = @Age, does_walk = @DoesWalk WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Update<Animal>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.SnakeCase);
                                })
                                .Set()
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }

                        [Test]
                        public void ShouldBuildUpdateStatementWithKebabCaseProperties()
                        {
                            // arrange
                            var expected =
                                "UPDATE people SET age = @Age, does-walk = @DoesWalk WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Update<Animal>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.KebabCase);
                                })
                                .Set()
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                    }

                    public class WithSqlVersioning
                    {
                        [Test]
                        public void ShouldBuildUpdateStatementForMySql()
                        {
                            // arrange
                            var expected =
                                "UPDATE people SET `age` = @Age, `does-walk` = @DoesWalk WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Update<Animal>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.KebabCase);
                                    o.UseSqlVersion(Version.MySql);
                                })
                                .Set()
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }

                        [Test]
                        public void ShouldGenerateUpdateStatementForMsSql()
                        {
                            // arrange
                            var expected =
                                "UPDATE people SET [Age] = @Age, [DoesWalk] = @DoesWalk WHERE id = @Id;";
                            // act
                            var sql = new SqlBuilder()
                                .Update<Animal>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.PascalCase);
                                    o.UseSqlVersion(Version.MsSql);
                                })
                                .Set()
                                .Where("id = @Id")
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                    }
                    
                }

                [TestFixture]
                public class WithoutOptions
                {
                    [Test]
                    public void ShouldGeneratePlainSqlStatement()
                    {
                        // arrange
                        var expected =
                            "UPDATE people SET Age = @Age, DoesWalk = @DoesWalk WHERE id = @Id;";
                        // act
                        var sql = new SqlBuilder()
                            .Update<Animal>("people")
                            .Set()
                            .Where("id = @Id")
                            .Build();
                        // assert
                        Expect(sql).To.Equal(expected);
                    }
                }
            }

            [TestFixture]
            public class WithoutGenericBuilder
            {
                [Test]
                public void ShouldBuildUpdateStatement()
                {
                    // arrange
                    var expected =
                        "UPDATE people SET id = @Id, name = @Name, surname = @Surname, email = @Email WHERE id = @Id;";
                    // act
                    var sql = new SqlBuilder()
                        .Update("people")
                        .Set("id = @Id, name = @Name, surname = @Surname, email = @Email")
                        .Where("id = @Id")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }
            }
        }
    }
}