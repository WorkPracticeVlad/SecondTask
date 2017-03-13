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
            foreach (XmlNode instance in ReadXmlByPath(path).SelectSingleNode("/instances"))
            {
                var orgUnitId = AddOrganizationUnit(instance, null);
                var valuesInUnit = AddPopsFromNode(instance.FirstChild);
                AddValues(orgUnitId, valuesInUnit);
                ConsiderFolder(TakeById(valuesInUnit, "configPath").Value, orgUnitId);
                int x = 1;
            }
        }
        void ConsiderFolder(string path, string parentId)
        {
            var files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                ConsiderFolderContent(path, files);
                if (files.Contains(path + "\\identity.xml"))
                    parentId = AddOrganizationUnit(path, path + "\\identity.xml", parentId);
                else
                    parentId = AddOrganizationUnit(path, null, parentId);
            }
            foreach (var folderPath in Directory.GetDirectories(path))
            {
                ConsiderFolder(folderPath, parentId);
            }
        }
        string AddOrganizationUnit(string path, string pathToDescription, string parenId)
        {
            string orgUnitId = GetOrgUnitId(path);
            var descr = "";
            if (!String.IsNullOrEmpty(pathToDescription))
                descr = ReadXmlByPath(pathToDescription).SelectSingleNode("/organization-unit/@description").Value;
            if (!_orgUnits.ContainsKey(orgUnitId))
                _orgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = descr,
                    IsVirtual = false,
                    ParentIdentity = parenId
                });
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
        private string GetOrgUnitId(string path)
        {
            string orgUnitId = path.Substring(path.LastIndexOf(backSlash) + 1);
            return orgUnitId;
        }
        Dictionary<Property, string> AddProps(string path)
        {
            return AddPopsFromNode(ReadXmlByPath(path).SelectSingleNode("/configuration"));
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
        void AddValues(string orgUnitId, Dictionary<Property, string> propsInOrgUnitWithValue)
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
        private void ConsiderFolderContent(string path, string[] files)
        {
            if (files.Contains(path + "\\organization_units.xml"))
                ConsiderXmlNode(ReadXmlByPath(path + "\\organization_units.xml").SelectSingleNode("/organization-units"), GetOrgUnitId(path));
            if (files.Contains(path + "\\config.xml"))
                AddValues(GetOrgUnitId(path), AddProps(path + "\\config.xml"));
        }
        private void ConsiderXmlNode(XmlNode node, string parentId)
        {
            foreach (XmlNode childNode in node)
            {
                if (childNode.Name == "organization-unit")
                {
                    var orgUnitId = AddOrganizationUnit(childNode, parentId);
                    AddValues(orgUnitId, AddPopsFromNode(childNode));
                    ConsiderXmlNode(childNode, orgUnitId);
                }
            }
        }
        XmlDocument ReadXmlByPath(string path)
        {
            XmlDocument doc = new XmlDocument();
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
