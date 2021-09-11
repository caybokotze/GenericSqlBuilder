using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;
using NExpect;
using NUnit.Framework;

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
                    
                }

                [TestFixture]
                public class WithoutOptions
                {
                    
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
                        "UPDATE people SET id = @Id, name = @Name, surname = @Surname, email = @Email WHERE id = @Id";
                    // act
                    var sql = new SqlBuilder()
                        .Update("people")
                        .Set("name = @Name, surname = @Surname, email = @Email")
                        .Where("id = @Id")
                        .Build();
                    // assert
                    Expectations.Expect(sql).To.Equal(expected);
                }
            }
        }
    }
}