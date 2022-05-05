using System.Collections.Generic;

namespace GenericSqlBuilder
{
    public interface IBaseOptions
    {
        void UsePropertyCase(Casing casing);
        void UseSqlVersion(Version version);
        void AddProperty(string name);
        void RemoveProperty(string name);
        void AddMultipleProperties(List<string> names);
        void RemoveMultipleProperties(List<string> names);
    }
    

    public interface ISelectOptions : IBaseOptions
    {
        void AddAlias(string alias);
        void AppendDistinct();
    }
    

    public class SqlBuilderOptions : ISelectOptions
    {
        public SqlBuilderOptions()
        {
            _removedProperties = new List<string>();
            _addedProperties = new List<string>();
            _casing = Casing.Default;
            _version = Version.Generic;
            _alias = "";
        }

        private Casing _casing;
        private readonly List<string> _removedProperties;
        private readonly List<string> _addedProperties;
        private Version _version;
        private string _alias;
        private bool _isDistinctSelect = false;

        public void ClearAll()
        {
            _addedProperties.Clear();
            _removedProperties.Clear();
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
        
        public void RemoveProperty(string name)
        {
            _removedProperties.Add(name);
        }
        
        public void RemoveMultipleProperties(List<string> names)
        {
            _removedProperties.AddRange(names);
        }

        public void AddProperty(string name)
        {
            _addedProperties.Add(name);
        }

        public void AddMultipleProperties(List<string> names)
        {
            _addedProperties.AddRange(names);
        }

        public void AppendDistinct()
        {
            _isDistinctSelect = true;
        }
        
        public bool IsDistinctSelect => _isDistinctSelect;
        
        public bool IsAppendStatement { get; set; }
        
        public string GetAlias()
        {
            return _alias;
        }

        public Casing GetCase()
        {
            return _casing;
        }

        public List<string> GetRemovedProperties()
        {
            return _removedProperties;
        }

        public List<string> GetAddedProperties()
        {
            return _addedProperties;
        }

        public Version GetSqlVersion()
        {
            return _version;
        }
    }
}