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
                var propertiesNode = instance.FirstChild;
                var propsValueToChild = AddPopsFromNode(propertiesNode);
                AddOrganizationUnitToProperty(orgUnitId, propsValueToChild);
                var configPathPropValue = TakeById(propsValueToChild, "configPath");
                var adminDbConnectionPropValue = TakeById(propsValueToChild, "adminDbConnection");
                ConsiderFolder(configPathPropValue.Value, propsValueToChild, orgUnitId);
                int x = 1;
            }
        }
        KeyValuePair<Property, string> TakeById(Dictionary<Property, string> propsValueToChild, string nameId)
        {
            var propValue = propsValueToChild.Single(i => i.Key.Name == nameId);
            propsValueToChild.Remove(propValue.Key);
            return propValue;
        }
        void ConsiderFolder(string path, Dictionary<Property, string> propsFromParentWithValue, string parentId)
        {
            var orgUnitId = parentId;
            Dictionary<Property, string> propsWithValuesToChild = new Dictionary<Property, string>();
            var folders = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                propsWithValuesToChild = ConsiderFolderContent(path, propsFromParentWithValue, files);
                if (files.Contains(path + "\\identity.xml"))
                    orgUnitId = AddOrganizationUnit(path, path + "\\identity.xml", parentId);
                else
                    orgUnitId = AddOrganizationUnit(path, parentId);
            }
            else
                FillPropsValuesToChild(propsFromParentWithValue, propsWithValuesToChild);
            foreach (var folderPath in folders)
            {
                ConsiderFolder(folderPath, propsWithValuesToChild, orgUnitId);
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
                        Type = chilNode.Attributes["type"].Value,
                        Items = AddNodeItems(chilNode.SelectSingleNode("items"))
                    };
                    if (!_props.ContainsKey(prop.Name))
                        _props.Add(prop.Name, prop);
                    propsInOrgUnitWithValue.Add(prop, chilNode.InnerText);
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
        private Dictionary<Property, string> ConsiderFolderContent(string path, Dictionary<Property, string> propsFromParentWithValue, string[] files)
        {
            if (files.Contains(path + "\\organization_units.xml"))
                ConsiderXmlTreeOrganizationUnits(path, propsFromParentWithValue);
            return ConsiderOrganizationUnit(path, propsFromParentWithValue, files);
        }
        void ConsiderXmlTreeOrganizationUnits(string path, Dictionary<Property, string> propsFromParentWithValue)
        {
            var doc = ReadXmlByPath(path + "\\organization_units.xml");
            var nodeOrgUnits = doc.SelectSingleNode("/organization-units");
            ConsiderXmlNode(propsFromParentWithValue, nodeOrgUnits, GetOrgUnitId(path));
        }
        private void ConsiderXmlNode(Dictionary<Property, string> propsFromParentWithValue, XmlNode node, string parentId)
        {
            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "organization-unit")
                {
                    var orgUnitId = AddOrganizationUnit(childNode, parentId);
                    ConsiderXmlNode(ConsiderOrganizationUnit(childNode, propsFromParentWithValue), childNode, orgUnitId);
                }
            }
        }
        Dictionary<Property, string> ConsiderOrganizationUnit(XmlNode node, Dictionary<Property, string> propsFromParentWithValue)
        {
            return FillPropsValuesToChild(propsFromParentWithValue, AddPopsFromNode(node), node.Attributes["id"].Value);
        }
        Dictionary<Property, string> ConsiderOrganizationUnit(string path, Dictionary<Property, string> propsFromParentWithValue, string[] files)
        {
            if (files.Contains(path + "\\config.xml"))
                return FillPropsValuesToChild(propsFromParentWithValue, AddProps(path + "\\config.xml"), GetOrgUnitId(path));
            return FillPropsValuesToChild(propsFromParentWithValue, new Dictionary<Property, string>(), GetOrgUnitId(path));
        }
        Dictionary<Property, string> FillPropsValuesToChild(Dictionary<Property, string> propsFromParentWithValue, Dictionary<Property, string> propsWithValuesToChild, string orgUnitId)
        {
            foreach (var propFromParentWithValue in propsFromParentWithValue)
            {
                if (!propsWithValuesToChild.ContainsKey(propFromParentWithValue.Key))
                    propsWithValuesToChild.Add(propFromParentWithValue.Key, propFromParentWithValue.Value);
            }
            AddOrganizationUnitToProperty(orgUnitId, propsWithValuesToChild);
            return propsWithValuesToChild;
        }
        Dictionary<Property, string> FillPropsValuesToChild(Dictionary<Property, string> propsFromParentWithValue, Dictionary<Property, string> propsWithValuesToChild)
        {
            foreach (var propFromParentWithValue in propsFromParentWithValue)
            {
                if (!propsWithValuesToChild.ContainsKey(propFromParentWithValue.Key))
                    propsWithValuesToChild.Add(propFromParentWithValue.Key, propFromParentWithValue.Value);
            }
            return propsWithValuesToChild;
        }
        List<Item> AddNodeItems(XmlNode node)
        {
            if (node == null)
                return null;
            var itemsList = new List<Item>();
            foreach (XmlNode chilNode in node.ChildNodes)
            {
                itemsList.Add(new Item { RecMode = chilNode.Attributes["recMode"].Value, Value = chilNode.InnerText });
            }
            return itemsList;
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
