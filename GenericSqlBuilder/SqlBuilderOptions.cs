using System.Collections.Generic;

namespace GenericSqlBuilder
{
    public interface IBaseOptions
    {
        void UsePropertyCase(Casing casing);
        void UseSqlVersion(Version version);
        void AddProperty(string name);
        void RemoveProperty(string name);
        void AddMultipleProperties(IEnumerable<string> names);
        void RemoveMultipleProperties(IEnumerable<string> names);
    }
    

    public interface ISelectOptions : IBaseOptions
    {
        void AddAlias(string alias);
        void AppendDistinct();
    }

    public interface IInsertOptions : IBaseOptions
    {
        void AddIgnore();
        void AddOnDuplicateKeyUpdate();
    }
    

    public class SqlBuilderOptions : ISelectOptions, IInsertOptions
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
        private bool _isDistinctSelect;
        private bool _addIgnore;
        private bool _addOnDuplicateKeyUpdate;

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

        public void AddIgnore()
        {
            _addIgnore = true;
        }

        public void AddOnDuplicateKeyUpdate()
        {
            _addOnDuplicateKeyUpdate = true;
        }

        public bool GetOnDuplicateKeyUpdate()
        {
            return _addOnDuplicateKeyUpdate;
        }

        public bool GetAddIgnore()
        {
            return _addIgnore;
        }

        public void AddAlias(string alias)
        {
            _alias = alias;
        }
        
        public void RemoveProperty(string name)
        {
            _removedProperties.Add(name);
        }
        
        public void RemoveMultipleProperties(IEnumerable<string> names)
        {
            _removedProperties.AddRange(names);
        }

        public void AddProperty(string name)
        {
            _addedProperties.Add(name);
        }

        public void AddMultipleProperties(IEnumerable<string> names)
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