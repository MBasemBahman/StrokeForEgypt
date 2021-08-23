using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.AccountEntity;
using StrokeForEgypt.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.AccountEntity
{
    public class RefreshTokenController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public RefreshTokenController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Account, bool ProfileLayOut = false)
        {
            RefreshTokenFilter Filter = new RefreshTokenFilter
            {
                Id = Id,
                Fk_Account = Fk_Account,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/AccountEntity/RefreshToken/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] RefreshTokenFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<RefreshToken> result = await _UnitOfWork.RefreshToken.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Account == 0 || a.Fk_Account == dtParameters.Fk_Account));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Token.ToLower().Contains(searchBy.ToLower())
                                        || a.Expires.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<RefreshToken> DataTableManager = new DataTableManager<RefreshToken>();

            DataTableResult<RefreshToken> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.RefreshToken.Count());

            return Json(new
            {
                draw = DataTableResult.DtParameters.Draw,
                recordsTotal = DataTableResult.TotalResultsCount,
                recordsFiltered = DataTableResult.FilteredResultsCount,
                data = DataTableResult.Data
                                      .Skip(DataTableResult.DtParameters.Start)
                                      .Take(DataTableResult.DtParameters.Length)
                                      .ToList()
            });
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Details(int id)
        {
            RefreshToken RefreshToken = await _UnitOfWork.RefreshToken.GetByID(id);

            if (RefreshToken == null)
            {
                return NotFound();
            }

            return View("~/Views/AccountEntity/RefreshToken/Details.cshtml", RefreshToken);
        }
    }
}
