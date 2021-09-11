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
                    
                    }
                }
                
                [TestFixture]
                public class WithoutOptions
                {
                    
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