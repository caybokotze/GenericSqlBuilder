using System.Collections.Generic;

namespace GenericSqlBuilder
{
    public interface IBaseOptions
    {
        void UsePropertyCase(Casing casing);
    }
    
    public interface ISelectOptions : IBaseOptions
    {
        void UseSqlVersion(Version version);
        void AddAlias(string alias);
        void RemoveSelectProperty(string name);
        void AddSelectProperty(string name);
    }

    public interface IInsertOptions : IBaseOptions
    {
        void RemoveInsertProperty(string name);
        void AddInsertProperty(string name);
    }

    public class SqlBuilderOptions : ISelectOptions, IInsertOptions
    {
        public SqlBuilderOptions()
        {
            _addedInsertProperties = new List<string>();
            _removedInsertProperties = new List<string>();
            _removedSelectProperties = new List<string>();
            _addedSelectProperties = new List<string>();
            _casing = Casing.Default;
            _version = Version.Generic;
            _alias = "";
        }

        private Casing _casing;
        private readonly List<string> _removedSelectProperties;
        private readonly List<string> _addedSelectProperties;
        private readonly List<string> _addedInsertProperties;
        private readonly List<string> _removedInsertProperties;
        private Version _version;
        private string _alias;

        public void ClearAll()
        {
            _addedInsertProperties.Clear();
            _addedSelectProperties.Clear();
            _removedInsertProperties.Clear();
            _removedSelectProperties.Clear();
        }
        
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
        
        public void RemoveSelectProperty(string name)
        {
            _removedSelectProperties.Add(name);
        }

        public void AddSelectProperty(string name)
        {
            _addedSelectProperties.Add(name);
        }
        public bool IsAppendStatement { get; set; }
        public string GetAlias()
        {
            return _alias;
        }

        public Casing GetCase()
        {
            return _casing;
        }

        public List<string> GetRemovedSelectProperties()
        {
            return _removedSelectProperties;
        }

        public List<string> GetAddedSelectProperties()
        {
            return _addedSelectProperties;
        }

        public Version GetSqlVersion()
        {
            return _version;
        }

        public void RemoveInsertProperty(string name)
        {
            _removedInsertProperties.Add(name);
        }

        public List<string> GetRemovedInsertProperties()
        {
            return _removedInsertProperties;
        }

        public void AddInsertProperty(string name)
        {
            _addedInsertProperties.Add(name);
        }

        public List<string> GetAddedInsertProperties()
        {
            return _addedInsertProperties;
        }
    }
}