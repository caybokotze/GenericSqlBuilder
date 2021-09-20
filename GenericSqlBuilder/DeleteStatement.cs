using System.Data;

namespace GenericSqlBuilder
{
    public class DeleteStatement : Statement
    {
        private bool _fromClauseHasExecuted;

        public DeleteStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            StatementIsComplete = false;
            _fromClauseHasExecuted = false;
            StatementType = StatementType.Delete;
            AddStatement(sql);
            AddStatement("DELETE ");
        }

        public DeleteStatement Where(string clause)
        {
            AddStatement("WHERE ");
            AddStatement($"{clause} ");
            StatementIsComplete = _fromClauseHasExecuted;
            return this;
        }

        public DeleteStatement From(string clause)
        {
            AddStatement("FROM ");
            AddStatement($"{clause} ");
            _fromClauseHasExecuted = true;
            return this;
        }
    }
}