using System.Collections.Generic;

namespace GenericSqlBuilder
{
    public class Options
    {
        public Options()
        {
            IgnoreProps = new List<string>();
            Casing = Casing.Default;
            Version = Version.Generic;
            Alias = "";
        }

        protected Casing Casing;
        protected readonly List<string> IgnoreProps;
        protected Version Version;
        protected string Alias;
        
        public void UsePropertyCase(Casing casing)
        {
            Casing = casing;
        }

        public void UseSqlVersion(Version version)
        {
            Version = version;
        }

        public void AddAlias(string alias)
        {
            Alias = alias;
        }
        
        public void AddIgnoreProperty(string name)
        {
            IgnoreProps.Add(name);
        }
    }
    
    public class SqlBuilderOptions : Options
    {
        public bool IsAppendStatement { get; set; }
        public string GetAlias()
        {
            return Alias;
        }

        public Casing GetCase()
        {
            return Casing;
        }

        public List<string> GetIgnoredProperties()
        {
            return IgnoreProps;
        }

        public Version GetSqlVersion()
        {
            return Version;
        }
    }
}