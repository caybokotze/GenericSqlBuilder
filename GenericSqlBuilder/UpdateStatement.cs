namespace GenericSqlBuilder
{
    public class UpdateStatement : Statement
    {
        private readonly string _sql;
        private readonly SqlBuilderOptions _sqlBuilderOptions;

        public UpdateStatement(string sql, SqlBuilderOptions sqlBuilderOptions) : base(sqlBuilderOptions)
        {
            _sql = sql;
            _sqlBuilderOptions = sqlBuilderOptions;
        }

        public UpdateStatement Set(string clause)
        {
            AddStatement(clause);
            return this;
        }

        public UpdateStatement Where(string clause)
        {
            AddStatement(clause);
            return this;
        }

        public void GenerateUpdateProperties()
        {
            
        }
    }
}