﻿using System;
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
        char backSlash = '\\';
        char borderChar = '.';
        readonly string configPathAtr = "configPath";
        private string _pathToEnviroment;
        public Dictionary<string, OrganizationUnit> OrgUnits { get; private set; }
        public Dictionary<string, Property> Properties { get; private set; }
        public Dictionary<string, OrganizationUnitToProperty> OrgUnitToProperties { get; private set; }
        public TreeCrawler(string pathToEnviroment)
        {
            OrgUnitToProperties = new Dictionary<string, OrganizationUnitToProperty>();
            Properties = new Dictionary<string, Property>();
            OrgUnits = new Dictionary<string, OrganizationUnit>();
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
            if (!path.EndsWith(backSlash + "organization"))
            {
                var orgUnitId = GetOrgUnitId(path, parentId);
                if (File.Exists(path + "\\organization_units.xml"))
                    ConsiderXmlNode(ReadXmlByPath(path + "\\organization_units.xml").SelectSingleNode("/organization-units"), orgUnitId);
                if (File.Exists(path + "\\config.xml"))
                    AddValues(orgUnitId, AddProperties(path + "\\config.xml"));
                    parentId = AddOrgUnit(path, orgUnitId, parentId);
            }         
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
            if (!OrgUnits.ContainsKey(orgUnitId))
                OrgUnits.Add(orgUnitId,
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
                parentId = "";
            var orgUnitId = parentId + borderChar + node.Attributes["id"]?.Value;
            var description = node.Attributes["description"]?.Value;
            if (!OrgUnits.ContainsKey(orgUnitId))
                OrgUnits.Add(orgUnitId,
                new OrganizationUnit
                {
                    Identity = orgUnitId,
                    Description = description,
                    IsVirtual = true,
                    ParentIdentity = parentId
                });
            return orgUnitId.Replace(backSlash, borderChar);
        }
        private string GetOrgUnitId(string path, string parentId)
        {
            if (parentId == ".US.RootConfig")
                parentId = "";
            if (path.EndsWith(backSlash + "config"))
                return parentId+borderChar+"RootConfig";
            string orgUnitId = parentId +borderChar +path.Substring(path.LastIndexOf(backSlash) + 1);
            return orgUnitId;
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
                if (!Properties.ContainsKey(property.Name))
                    Properties.Add(property.Name, property);
                else
                    if (Properties[property.Name].Type==null)
                        Properties[property.Name].Type = property.Type;
                propertiesInOrgUnitWithValue.Add(property, propertyNode.InnerXml);
            }
            return propertiesInOrgUnitWithValue;
        }
        void AddValues(string orgUnitId, Dictionary<Property, string> propertiesInOrgUnitWithValue)
        {
            foreach (var nameValueProperty in propertiesInOrgUnitWithValue)
            {
                if (!OrgUnitToProperties.ContainsKey(orgUnitId + nameValueProperty.Key.Name))
                    OrgUnitToProperties.Add(orgUnitId + nameValueProperty.Key.Name, new OrganizationUnitToProperty
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
