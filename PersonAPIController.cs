using Sabio.Web.Domain;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Requests;
using Sabio.Web.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sabio.Web.Controllers.Api
{
    [RoutePrefix("api/person")]
    public class PersonsApiController : ApiController
    {
        private IPersonService _personService = null;
        public PersonsApiController(IPersonService personService)
        {
            _personService = personService;
        }

        [Route , HttpPost]
        public HttpResponseMessage Create(PersonAddRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();

            response.Item = _personService.Insert(model);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage Update(PersonUpdateRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _personService.Update(model);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}"), HttpGet]
        public HttpResponseMessage SelectById(int id)
        {

            ItemResponse<Person> response = new ItemResponse<Person>()
            {
                Item = _personService.SelectById(id)
            };

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route, HttpGet]
        public HttpResponseMessage Get()
        {
            PersonService srv = new PersonService();
            List<Person> data = srv.GetAll();
            ItemsResponse<Person> res = new ItemsResponse<Person>();
            res.Items = data;
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            _personService.Delete(id);

            SuccessResponse response = new SuccessResponse();

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("search"), HttpGet]
        public HttpResponseMessage PersonSearch([FromUri]PersonSearchRequest model)
        {
            int rows = 0;
            List<PersonBase> person = _personService.PersonBaseSearch(model, out rows);
            SearchResponse<PersonBase>
                response = new SearchResponse<PersonBase>();
            response.Items = person;
            response.ResultCount = rows;

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }

        [Route("{id:int}/projects"), HttpGet]
        public HttpResponseMessage GetProjects (int id)
        {
            List<PersonProject> pj = _personService.SelectProjectByPersonId(id);
            ItemsResponse<PersonProject> response = new ItemsResponse<PersonProject>();
            response.Items = pj;
            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
    }
}

