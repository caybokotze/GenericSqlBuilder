using GenericSqlBuilder.Tests.TestModels;
using NExpect;
using NUnit.Framework;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class InsertStatementTests
    {
        [TestFixture]
        public class WhenBuildingInsertStatements
        {
            [TestFixture]
            public class WithGenericBuilder
            {
                [TestFixture]
                public class WithOptions
                {
                    [TestFixture]
                    public class WithAppendLastInsert
                    {
                        [Test]
                        public void ShouldBuildInsertStatementAndAppendLastSelected()
                        {
                            // arrange
                            var expected =
                                "INSERT INTO people (id, name, surname, email) VALUES (@Id, @Name, @Surname, @Email); SELECT LAST_INSERT_ID();";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Person>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.LowerCase);
                                })
                                .Values()
                                .AppendStatement()
                                .Select()
                                .LastInserted(Version.MySql);
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                    }

                    [TestFixture]
                    public class WithAppendedCustomProperties
                    {
                        [Test]
                        public void ShouldAppendCustomPropertyAndAttribute()
                        {
                            // arrange
                            var expected =
                                "INSERT INTO people (Id, Name, Surname, Email, Height) VALUES (@Id, @Name, @Surname, @Email, @Height);";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Person>("people", o =>
                                {
                                    o.AddInsertProperty("Height");
                                })
                                .Values()
                                .Build();
                            // assert
                            Expect(sql).To.Equal(expected);
                        }
                    }

                    [TestFixture]
                    public class WithRemovedCustomProperties
                    {
                        // arrange
                        // act
                        // assert
                    }

                    public class WithCaseConvertingOptions
                    {
                        [Test]
                        public void ShouldGenerateInsertStatementInLowerCase()
                        {
                            // arrange
                            var expected =
                                @"INSERT INTO people (id, name, surname, email) VALUES (@Id, @Name, @Surname, @Email);";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Person>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.LowerCase);
                                })
                                .Values()
                                .Build();
                            // assert
                            Expect(sql)
                                .To
                                .Equal(expected);
                        }
                        
                        [Test]
                        public void ShouldGenerateInsertStatementInUpperCase()
                        {
                            // arrange
                            var expected =
                                @"INSERT INTO people (ID, NAME, SURNAME, EMAIL) VALUES (@Id, @Name, @Surname, @Email);";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Person>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.UpperCase);
                                })
                                .Values()
                                .Build();
                            // assert
                            Expect(sql)
                                .To
                                .Equal(expected);
                        }
                        
                        [Test]
                        public void ShouldGenerateInsertStatementInKebabCase()
                        {
                            // arrange
                            var expected =
                                @"INSERT INTO people (age, does-walk) VALUES (@Id, @Name, @Surname, @Email);";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Animal>("people", o =>
                                {
                                    o.UsePropertyCase(Casing.KebabCase);
                                })
                                .Values()
                                .Build();
                            // assert
                            Expect(sql)
                                .To
                                .Equal(expected);
                        }


                        [Test]
                        public void ShouldGenerateInsertStatementInSnakeCase()
                        {
                            // arrange
                            var expected =
                                @"INSERT INTO people (age, does_walk) VALUES (@Id, @Name, @Surname, @Email);";
                            // act
                            var sql = new SqlBuilder()
                                .Insert<Animal>("people", o => { o.UsePropertyCase(Casing.SnakeCase); })
                                .Values()
                                .Build();
                            // assert
                            Expect(sql)
                                .To
                                .Equal(expected);
                        }
                    }
                }
                
                [TestFixture]
                public class WithoutOptions
                {
                    [Test]
                    public void ShouldBuildGenericInsertStatement()
                    {
                        // arrange
                        var expected =
                            @"INSERT INTO people (Id, Name, Surname, Email) 
                            VALUES (@Id, @Name, @Surname, @Email);";
                        // act
                        var sql = new SqlBuilder()
                            .Insert<Person>("people")
                            .Values()
                            .Build();
                        // assert
                        Expect(sql).To.Equal(expected.RemoveDoubleWhiteSpace());
                    }
                }
            }
            
            [TestFixture]
            public class WithoutGenericBuilder
            {
                [TestFixture]
                public class WithAppendLastInsert
                {
                    [Test]
                    public void ShouldBuildInsertStatementAndAppendLastInsertStatement_ForMySql()
                    {
                        // arrange
                        var expected =
                            @"INSERT INTO people (Id, Name, Surname, Email) 
                            VALUES (@Id, @Name, @Surname, @Email); 
                            SELECT LAST_INSERT_ID();";
                        // act
                        var sql = new SqlBuilder()
                            .Insert("people", "Id, Name, Surname, Email")
                            .Values("@Id, @Name, @Surname, @Email")
                            .AppendStatement()
                            .Select()
                            .LastInserted(Version.MySql);
                        // assert
                        Expect(sql).To.Equal(expected.RemoveDoubleWhiteSpace());
                    }
                }

                [Test]
                public void ShouldBuildBasicInsertStatement()
                {
                    // arrange
                    var expected = "INSERT INTO people (Id, Name, Surname, Email) VALUES (@Id, @Name, @Surname, @Email);";
                    // act
                    var sql = new SqlBuilder()
                        .Insert("people", "Id, Name, Surname, Email")
                        .Values("@Id, @Name, @Surname, @Email")
                        .Build();
                    // assert
                    Expect(sql).To.Equal(expected);
                }
            }
        }
    }
}