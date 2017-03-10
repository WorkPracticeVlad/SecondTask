using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Tree
{
    public class TreeCrawler
    {
        Dictionary<string, OrganizationUnit> _orgUnits = new Dictionary<string, OrganizationUnit>();
        Dictionary<string, Property> _props = new Dictionary<string, Property>();
        Dictionary<string, OrganizationUnitToProperty> _orgUnitToProps = new Dictionary<string, OrganizationUnitToProperty>();
        char backSlash = '\\';
        public Dictionary<string, OrganizationUnit> OrgUnints { get { return _orgUnits; } }
        public Dictionary<string, Property> Props { get { return _props; } }
        public Dictionary<string, OrganizationUnitToProperty> OrgUnitToProps { get { return _orgUnitToProps; } }
        public void EnterEnvironment(string path)
        {
            var doc = ReadXmlByPath(path);
            var instances = doc.SelectSingleNode("/instances");
            foreach (XmlNode instance in instances)
            {
                var orgUnitId = AddOrganizationUnit(instance, null);
                var valuesInUnit = AddPopsFromNode(instance.FirstChild);
                AddOrganizationUnitToProperty(orgUnitId, valuesInUnit);
                var configPathPropValue = TakeById(valuesInUnit, "configPath");
                ConsiderFolder(configPathPropValue.Value, orgUnitId);
                int x = 1;
            }
        }
        KeyValuePair<Property, string> TakeById(Dictionary<Property, string> propsValue, string nameId)
        {
            var propValue = propsValue.Single(i => i.Key.Name == nameId);
            return propValue;
        }
        void ConsiderFolder(string path, string parentId)
        {
            var orgUnitId = parentId;         
            var folders = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                ConsiderFolderContent(path, files);
                if (files.Contains(path + "\\identity.xml"))
                    orgUnitId = AddOrganizationUnit(path, path + "\\identity.xml", parentId);
                else
                    orgUnitId = AddOrganizationUnit(path, parentId);
            }
            foreach (var folderPath in folders)
            {
                ConsiderFolder(folderPath, orgUnitId);
            }
        }
        string AddOrganizationUnit(string path, string pathToDescription, string parenId)
        {
            string orgUnitId = GetOrgUnitId(path);
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = AddOrganizationUnitDescription(pathToDescription),
                    IsVirtual = false,
                    ParentIdentity = parenId
                });
            return orgUnitId;
        }
        string AddOrganizationUnit(string path, string parentId)
        {
            string orgUnitId = GetOrgUnitId(path);
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = "",
                    IsVirtual = false,
                    ParentIdentity = parentId
                });
            return orgUnitId;
        }
        private string GetOrgUnitId(string path)
        {
            string orgUnitId = path.Substring(path.LastIndexOf(backSlash) + 1);
            return orgUnitId;
        }
        string AddOrganizationUnit(XmlNode node, string parentId)
        {
            var orgUnitId = node.Attributes["id"].Value;
            var description = "";
            if (node.Attributes["description"] != null)
                description = node.Attributes["description"].Value;
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = description,
                    IsVirtual = true,
                    ParentIdentity = parentId
                });
            return orgUnitId;
        }
        Dictionary<Property, string> AddProps(string path)
        {
            var doc = ReadXmlByPath(path);
            var node = doc.SelectSingleNode("/configuration");
            return AddPopsFromNode(node);
        }
        Dictionary<Property, string> AddPopsFromNode(XmlNode node)
        {
            var propsInOrgUnitWithValue = new Dictionary<Property, string>();
            foreach (XmlNode chilNode in node.ChildNodes)
            {
                if (chilNode.Name == "property")
                {
                    var prop = new Property
                    {
                        Name = chilNode.Attributes["name"].Value,
                        Type = chilNode.Attributes["type"].Value
                    };
                    if (!_props.ContainsKey(prop.Name))
                        _props.Add(prop.Name, prop);
                    propsInOrgUnitWithValue.Add(prop, chilNode.InnerXml);
                }
            }
            return propsInOrgUnitWithValue;
        }
        void AddOrganizationUnitToProperty(string orgUnitId, Dictionary<Property, string> propsInOrgUnitWithValue)
        {
            foreach (var nameValueProp in propsInOrgUnitWithValue)
            {
                if (!_orgUnitToProps.ContainsKey(orgUnitId + nameValueProp.Key.Name))
                    _orgUnitToProps.Add(orgUnitId + nameValueProp.Key.Name, new OrganizationUnitToProperty
                    {
                        OrganizationUnitIdentity = orgUnitId,
                        PropertyName = nameValueProp.Key.Name,
                        Value = nameValueProp.Value
                    });
            }
        }
        private Dictionary<Property, string> ConsiderFolderContent(string path,  string[] files)
        {
            if (files.Contains(path + "\\organization_units.xml"))
                ConsiderXmlTreeOrganizationUnits(path);
            return ConsiderOrganizationUnit(path, files);
        }
        void ConsiderXmlTreeOrganizationUnits(string path)
        {
            var doc = ReadXmlByPath(path + "\\organization_units.xml");
            var nodeOrgUnits = doc.SelectSingleNode("/organization-units");
            ConsiderXmlNode(nodeOrgUnits, GetOrgUnitId(path));
        }
        private void ConsiderXmlNode( XmlNode node, string parentId)
        {
            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "organization-unit")
                {
                    var orgUnitId = AddOrganizationUnit(childNode, parentId);
                    ConsiderXmlNode( childNode, orgUnitId);
                }
            }
        }
        Dictionary<Property, string> ConsiderOrganizationUnit(XmlNode node)
        {
            return FillValues( AddPopsFromNode(node), node.Attributes["id"].Value);
        }
        Dictionary<Property, string> ConsiderOrganizationUnit(string path,  string[] files)
        {
            if (files.Contains(path + "\\config.xml"))
                return FillValues(AddProps(path + "\\config.xml"), GetOrgUnitId(path));
            return FillValues( new Dictionary<Property, string>(), GetOrgUnitId(path));
        }
        Dictionary<Property, string> FillValues( Dictionary<Property, string> valuesResult, string orgUnitId)
        {          
            AddOrganizationUnitToProperty(orgUnitId, valuesResult);
            return valuesResult;
        }
        string AddOrganizationUnitDescription(string pathToDescription)
        {
            XmlDocument doc = ReadXmlByPath(pathToDescription);
            return doc.SelectSingleNode("/organization-unit/@description").Value;
        }
        XmlDocument ReadXmlByPath(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc;
        }
    }
}
