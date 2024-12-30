using api_intranet_surfland.DataBase;
using api_intranet_surfland.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace api_intranet_surfland.Controllers {
    [ApiController]
    [Route("Persons")]
    public class Person : ControllerBase {

        [Authorize]
        [HttpGet("{personId}")]
        public IActionResult GetPerson(int personId) {
            var person = DBPersons.GetPerson(personId);

            if (person == null) {
                return NotFound(new IResponse {
                    status = false,
                    message = "Não foi encontrado nenhuma pessoa com este Id"
                });
            } else {
                return Ok(new IResponse {
                    status = true,
                    data = person
                });
            }
        }

        [Authorize]
        [HttpGet("Find")]
        public IActionResult FindPersons(
            [FromQuery] string? name,
            [FromQuery] string? document,
            [FromQuery] string? birthDate,
            [FromQuery] string? email,
            [FromQuery] string? phone,
            [FromQuery] string? foreigner
        ) {
            Console.WriteLine("Autorizado");
            List<DTOPerson> persons = DBPersons.FindPersons(name, document, birthDate, email, phone, foreigner);

            if (persons is null || persons.Count == 0) {
                return NotFound(new IResponse {
                    status = false,
                    message = "Não foi encontrado nenhuma pessoa com estas informações",
                });
            } else {
                return Ok(new IResponse {
                    status = true,
                    data = persons
                });
            }
        }

        [Authorize]
        [HttpPost("Create")]
        public IActionResult CreatePerson([FromBody] DTOPerson toCreatePerson) {
            List<DTOPerson> existingPersons = DBPersons.FindPersons(document: toCreatePerson.Document);

            if (existingPersons.Count == 0) {
                DTOPerson createdPerson = DBPersons.CreatePerson(toCreatePerson);

                if (createdPerson is null) {
                    return BadRequest(new IResponse {
                        status = false,
                        message = "Não foi possível criar esta pessoa"
                    });
                } else {
                    return Created("", new IResponse {
                        status = true,
                        data = createdPerson
                    });
                }
            } else {
                return BadRequest(new IResponse {
                    status = false,
                    message = "Já existe uma pessoa com este documento."
                });
            }
        }

        [Authorize]
        [HttpPut("Update")]
        public IActionResult UpdatePerson([FromBody] DTOPerson toUpdatePerson) {
            if (toUpdatePerson.PersonId is null) {
                return BadRequest(new IResponse {
                    status = false,
                    message = "É necessário informar o Id da pessoa que deseja atualizar"
                });
            } else {
                if(
                    toUpdatePerson.Name is null &&
                    toUpdatePerson.Document is null &&
                    toUpdatePerson.BirthDate is null &&
                    toUpdatePerson.Email is null &&
                    toUpdatePerson.Phone is null &&
                    toUpdatePerson.Foreigner is null
                  ) {
                    return BadRequest(new IResponse {
                        status = false,
                        message = "Não foi informado nenhum dado para ser atualizado"
                    });
                }
            }

            DTOPerson existingPerson = DBPersons.GetPerson(toUpdatePerson.PersonId ?? 0);

            if (existingPerson.PersonId is null) {
                return NotFound(new IResponse {
                    status = false,
                    message = "Não existe nenhuma pessoa com este Id"
                });
            } else {
                DTOPerson updatedPerson = DBPersons.UpdatePerson(toUpdatePerson);
                if (updatedPerson is null) {
                    return BadRequest(new IResponse {
                        status = false,
                        message = "Não foi possível atualizar esta pessoa"
                    });
                } else {
                    return Ok(new IResponse {
                        status = true,
                        data = updatedPerson
                    });
                }
            }
        }
    }
}