using NExpect;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using static NExpect.Expectations;

namespace GenericSqlBuilder.Tests
{
    [TestFixture]
    public class DeleteStatementTests
    {
        [TestFixture]
        public class WhenDeleteStatementIsComplete
        {
            [Test]
            public void ShouldReturnValidDeleteStatement()
            {
                // arrange
                var expected = "DELETE FROM products WHERE id = @Id;";
                // act
                var sql = new SqlBuilder()
                    .Delete()
                    .From("products")
                    .Where("id = @Id")
                    .Build();
                // assert
                Expect(sql).To.Equal(expected);
            }
        }

        [TestFixture]
        public class WhenDeleteStatementIsNotComplete
        {
            [Test]
            public void ShouldThrowException()
            {
                // arrange
                // act
                var sqlBuilder = new SqlBuilder()
                    .Delete()
                    .Where("id = @id");
                Expect(() => sqlBuilder.Build())
                    .To.Throw<InvalidSqlStatementException>()
                    .With.Message.Containing("Your delete statement syntax is incorrect.");
                // assert
            }
        }
    }
}