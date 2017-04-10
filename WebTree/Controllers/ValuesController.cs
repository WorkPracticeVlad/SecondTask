﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tree.Data;
using Tree.Data.SubData;
using Tree.Manage;

namespace WebTree.Controllers
{
    public class ValuesController : ApiController
    {
        private Manager _manager;
        int _itemsPerPage = 3;
        public ValuesController()
        {
            _manager = new Manager();
        }    
        [HttpGet]
        [Route("api/values/pagesbyproperty/{id}")]
        public int PagesByProperty(string id = "")
        {
            return _manager.ValuesRepository.CountPagesPropertyFiltered(_itemsPerPage,"",id);
        }
        [HttpGet]
        [Route("api/values/pagesbyproperty/{id}/{filter}")]
        public int PagesByProperty(string id = "", string filter="")
        {
            return _manager.ValuesRepository.CountPagesPropertyFiltered(_itemsPerPage, filter,id);
        }
        [HttpGet]
        [Route("api/values/byproperty/{id}/{page}")]
        public List<OrganizationUnitToProperty> ByProperty(string id, int page)
        {
            return _manager.ValuesRepository.ReadFilteredPropertyPageFromDb(page,  _itemsPerPage,  "",  id);
        }
        [HttpGet]
        [Route("api/values/byproperty/{id}/{page}/{filter}")]
        public List<OrganizationUnitToProperty> ByProperty(string id, int page ,string filter="")
        {
            return _manager.ValuesRepository.ReadFilteredPropertyPageFromDb(page, _itemsPerPage, filter,id);
        }
        [HttpGet]
        [Route("api/values/pagesbyorgunit/{id}")]
        public int PagesByOrgUnit(string id)
        {
            return _manager.ValuesRepository.CountPagesByOrgUnit(_itemsPerPage, id.Replace('-', '.'), "");
        }
        [HttpGet]
        [Route("api/values/pagesbyorgunit/{id}/{filter}")]
        public int PagesByOrgUnit(string id, string filter)
        {
            return _manager.ValuesRepository.CountPagesByOrgUnit(_itemsPerPage, id.Replace('-', '.'), filter);
        }
        [HttpGet]
        [Route("api/values/byorgunit/{id}/{page}")]
        public ForOrgUnitProperties ByOrgUnit(string id, int page)
        {
            return _manager.ValuesRepository.ReadPageOrganizationUnitValuesFilteredFromDb(id.Replace('-', '.'),page,_itemsPerPage,"");
        }
        [HttpGet]
        [Route("api/values/byorgunit/{id}/{page}/{filter}")]
        public ForOrgUnitProperties ByOrgUnit(string id, int page, string filter="")
        {
            return _manager.ValuesRepository.ReadPageOrganizationUnitValuesFilteredFromDb(id.Replace('-', '.'), page, _itemsPerPage, filter);
        }
        [HttpGet]
        [Route("api/values/pagesbycurrentorgunit/{id}")]
        public int PagesCurrentByOrgUnit(string id)
        {
            return _manager.ValuesRepository.CountPagesByCurrentOrgUnit(_itemsPerPage, id.Replace('-', '.'), "");
        }
        [HttpGet]
        [Route("api/values/pagesbycurrentorgunit/{id}/{filter}")]
        public int PagesCurrentByOrgUnit(string id, string filter)
        {
            return _manager.ValuesRepository.CountPagesByCurrentOrgUnit(_itemsPerPage, id.Replace('-', '.'), filter);
        }
        [HttpGet]
        [Route("api/values/bycurrentorgunit/{id}/{page}")]
        public ForOrgUnitProperties ByCurrentOrgUnit(string id, int page)
        {
            return _manager.ValuesRepository.ReadPageOrganizationUnitCurrentValuesFilteredFromDb(id.Replace('-', '.'), page, _itemsPerPage, "");
        }
        [HttpGet]
        [Route("api/values/bycurrentorgunit/{id}/{page}/{filter}")]
        public ForOrgUnitProperties ByCurrentOrgUnit(string id, int page, string filter = "")
        {
            return _manager.ValuesRepository.ReadPageOrganizationUnitCurrentValuesFilteredFromDb(id.Replace('-', '.'), page, _itemsPerPage, filter);
        }

    }
}
