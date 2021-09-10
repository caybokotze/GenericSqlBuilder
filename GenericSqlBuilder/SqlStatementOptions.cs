using System.Collections.Generic;

namespace GenericSqlBuilder
{
    public class SqlStatementOptions
    {
        public SqlStatementOptions()
        {
            _ignoreProps = new List<string>();
            _casing = Casing.LowerCase;
            _version = Version.MySql;
            _alias = "";
        }
        
        private Casing _casing;
        private readonly List<string> _ignoreProps;
        private Version _version;
        private string _alias;

        public void UsePropertyCase(Casing casing)
        {
            _casing = casing;
        }

        public void UseSqlVersion(Version version)
        {
            _version = version;
        }

        public void AddAlias(string alias)
        {
            _alias = alias;
        }

        public string GetAlias()
        {
            return _alias;
        }

        public Casing GetCase()
        {
            return _casing;
        }

        public List<string> GetIgnoredProperties()
        {
            return _ignoreProps;
        }

        public Version GetSqlVersion()
        {
            return _version;
        }

        public void IgnoreProperty(string name)
        {
            _ignoreProps.Add(name);
        }
    }
}