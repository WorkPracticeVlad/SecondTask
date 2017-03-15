using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Tree.Data;

namespace Tree
{
    public class TreeCrawler
    {
        Dictionary<string, OrganizationUnit> _orgUnits = new Dictionary<string, OrganizationUnit>();
        Dictionary<string, Property> _properties = new Dictionary<string, Property>();
        Dictionary<string, OrganizationUnitToProperty> _orgUnitToProperties = new Dictionary<string, OrganizationUnitToProperty>();
        char backSlash = '\\';
        char borderChar = '.';
        readonly string configPathAtr = "configPath";
        private string _pathToEnviroment;

        public Dictionary<string, OrganizationUnit> OrgUnits { get { return _orgUnits; } }
        public Dictionary<string, Property> Properties { get { return _properties; } }
        public Dictionary<string, OrganizationUnitToProperty> OrganizationUnitToProperties { get { return _orgUnitToProperties; } }
        public TreeCrawler(string pathToEnviroment)
        {
            _pathToEnviroment = pathToEnviroment;
        }
        public void EnterEnvironment()
        {
            foreach (XmlNode instance in ReadXmlByPath(_pathToEnviroment).GetElementsByTagName("instance"))
            {
                var orgUnitId = AddOrgUnit(instance, null);
                var valuesInUnit = AddPopertiesFromNode(instance);
                AddValues(orgUnitId, valuesInUnit);
                ConsiderFolder(TakeById(valuesInUnit, configPathAtr).Value, orgUnitId);
            }
        }
        void ConsiderFolder(string path, string parentId)
        {
            var orgUnitId = GetOrgUnitId(path);
            if (File.Exists(path + "\\organization_units.xml"))
                ConsiderXmlNode(ReadXmlByPath(path + "\\organization_units.xml").SelectSingleNode("/organization-units"), orgUnitId);
            if (File.Exists(path + "\\config.xml"))
                AddValues(orgUnitId, AddProperties(path + "\\config.xml"));
            if (!path.EndsWith(backSlash + "organization"))
                parentId = AddOrgUnit(path, orgUnitId, parentId);
            foreach (var folderPath in Directory.GetDirectories(path))
            {
                ConsiderFolder(folderPath, parentId);
            }
        }
        string AddOrgUnit(string path, string orgUnitId, string parenId)
        {
            string descr = null;
            if (File.Exists(path + "\\identity.xml"))
                descr = ReadXmlByPath(path + "\\identity.xml").SelectSingleNode("/organization-unit/@description")?.Value;
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = descr,
                    IsVirtual = false,
                    ParentIdentity = parenId
                });
            return orgUnitId.Replace(backSlash, borderChar);
        }
        string AddOrgUnit(XmlNode node, string parentId)
        {
            if (String.IsNullOrEmpty(parentId))
                parentId = "Instanse";
            var orgUnitId = parentId + borderChar + node.Attributes["id"]?.Value;
            var description = node.Attributes["description"]?.Value;
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = description,
                    IsVirtual = true,
                    ParentIdentity = parentId
                });
            return orgUnitId.Replace(backSlash, borderChar);
        }
        private string GetOrgUnitId(string path)
        {
            if (path.EndsWith(backSlash + "organization"))
                return null;
            if (path.EndsWith(backSlash + "config"))
                return "RootConfig";
            string orgUnitId = path.Substring(path.LastIndexOf(backSlash + "organization") + 14);
            return orgUnitId.Replace(backSlash, borderChar);
        }
        Dictionary<Property, string> AddProperties(string path)
        {
            return AddPopertiesFromNode(ReadXmlByPath(path).SelectSingleNode("/configuration"));
        }
        Dictionary<Property, string> AddPopertiesFromNode(XmlNode node)
        {
            var propertiesInOrgUnitWithValue = new Dictionary<Property, string>();
            foreach (XmlNode propertyNode in node.SelectNodes(".//property"))
            {
                var property = new Property
                {
                    Name = propertyNode.Attributes["name"]?.Value,
                    Type = propertyNode.Attributes["type"]?.Value
                };
                if (!_properties.ContainsKey(property.Name))
                    _properties.Add(property.Name, property);
                else
                    if (_properties[property.Name].Type==null)
                        _properties[property.Name].Type = property.Type;
                propertiesInOrgUnitWithValue.Add(property, propertyNode.InnerXml);
            }
            return propertiesInOrgUnitWithValue;
        }
        void AddValues(string orgUnitId, Dictionary<Property, string> propertiesInOrgUnitWithValue)
        {
            foreach (var nameValueProperty in propertiesInOrgUnitWithValue)
            {
                if (!_orgUnitToProperties.ContainsKey(orgUnitId + nameValueProperty.Key.Name))
                    _orgUnitToProperties.Add(orgUnitId + nameValueProperty.Key.Name, new OrganizationUnitToProperty
                    {
                        OrganizationUnitIdentity = orgUnitId,
                        PropertyName = nameValueProperty.Key.Name,
                        Value = nameValueProperty.Value
                    });
            }
        }
        private void ConsiderXmlNode(XmlNode node, string parentId)
        {
            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "organization-unit")
                {
                    var orgUnitId = AddOrgUnit(childNode, parentId);
                    AddValues(orgUnitId, AddPopertiesFromNode(childNode));
                    ConsiderXmlNode(childNode, orgUnitId);
                }
            }
        }
        XmlDocument ReadXmlByPath(string path)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(path))
                doc.Load(path);
            return doc;
        }
        KeyValuePair<Property, string> TakeById(Dictionary<Property, string> propsValue, string nameId)
        {
            var propValue = propsValue.Single(i => i.Key.Name == nameId);
            return propValue;
        }
    }
}
