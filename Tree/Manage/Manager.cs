using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree.Data;
using Tree.DB;

namespace Tree.Manage
{
    public class Manager
    {
        private string _pathToEnivorment;
        private TreeCrawler _treeCrawler;
        public OrganizationUnitRepository OrgUnitsRepository { get; private set; }
        public PropertyRepository PropertiesRepository { get; private set; }
        public OrganizationUnitToPropertyRepository ValuesRepository { get; private set; }

        public Manager()
        {
            _pathToEnivorment = ConfigurationManager.AppSettings["pathToEnviroment"];
            _treeCrawler = new TreeCrawler(_pathToEnivorment);
            OrgUnitsRepository = new OrganizationUnitRepository("[dbo].[OrganizationUnits]", 5);
            PropertiesRepository = new PropertyRepository("[dbo].[Properties]", 5);
            ValuesRepository = new OrganizationUnitToPropertyRepository("[dbo].[OrganizationUnitToProperties]", 5);
        }
        public void RefreshTree()
        {
            DeleteTree();
            InsertTree();      
        }
        void InsertTree()
        {
            _treeCrawler.EnterEnvironment();
            OrgUnitsRepository.InsertToDb(_treeCrawler.OrgUnits.Values.ToList());
            PropertiesRepository.InsertToDb(_treeCrawler.Properties.Values.ToList());
            ValuesRepository.InsertToDb(_treeCrawler.OrgUnitToProperties.Values.ToList());
        }
        void DeleteTree()
        {
            ValuesRepository.DeleteDataFromDb();
            OrgUnitsRepository.DeleteDataFromDb();
            PropertiesRepository.DeleteDataFromDb();
        }
    }
}
