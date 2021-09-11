using GenericSqlBuilder.Tests.TestModels;
using NUnit.Framework;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class InsertStatementTests
    {
        public class WhenBuildingInsertStatements
        {
            public class WithGenericBuilder
            {
                public class WithAppendLastInsert
                {
                    
                }

                public class WithOptions
                {
                    
                }

                public class WithoutOptions
                {
                    
                }
            }

            public class WithoutGenericBuilder
            {
                public class WithAppendLastInsert
                {
                    
                }

                public class WithOptions
                {
                    
                }

                public class WithoutOptions
                {
                    public void ShouldBuildBasicInsertStatement()
                    {
                        // arrange
                        var expected = "INSERT INTO people (Id, Name, Surnam, Email) VALUES ()";
                        // act
                        var sql = new SqlBuilder()
                            .Insert<Person>()
                            .Values();
                        // assert
                    }
                }
            }
        }
    }
}