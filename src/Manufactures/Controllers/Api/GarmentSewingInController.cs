﻿using Barebone.Controllers;
using Infrastructure.Data.EntityFrameworkCore.Utilities;
using Manufactures.Domain.GarmentSewingIns.Commands;
using Manufactures.Domain.GarmentSewingIns.Repositories;
using Manufactures.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Manufactures.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("sewing-ins")]
    public class GarmentSewingInController : ControllerApiBase
    {
        private readonly IGarmentSewingInRepository _garmentSewingInRepository;
        private readonly IGarmentSewingInItemRepository _garmentSewingInItemRepository;

        public GarmentSewingInController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _garmentSewingInRepository = Storage.GetRepository<IGarmentSewingInRepository>();
            _garmentSewingInItemRepository = Storage.GetRepository<IGarmentSewingInItemRepository>();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1, int size = 25, string order = "{}", [Bind(Prefix = "Select[]")]List<string> select = null, string keyword = null, string filter = "{}")
        {
            VerifyUser();

            var query = _garmentSewingInRepository.Read(page, size, order, keyword, filter);
            var count = query.Count();

            var garmentSewingInDto = _garmentSewingInRepository.Find(query).Select(o => new GarmentSewingInListDto(o)).ToArray();
            var garmentSewingInItemDto = _garmentSewingInItemRepository.Find(_garmentSewingInItemRepository.Query).Select(o => new GarmentSewingInItemDto(o)).ToList();
            var garmentSewingInItemDtoArray = _garmentSewingInItemRepository.Find(_garmentSewingInItemRepository.Query).Select(o => new GarmentSewingInItemDto(o)).ToArray();

            Parallel.ForEach(garmentSewingInDto, itemDto =>
            {
                var garmentSewingDOItems = garmentSewingInItemDto.Where(x => x.SewingInId == itemDto.Id).ToList();

                itemDto.Items = garmentSewingDOItems;

                itemDto.Items = itemDto.Items.OrderBy(x => x.Id).ToList();

                itemDto.Products = itemDto.Items.Select(i => i.Product.Code).ToList();
                itemDto.TotalQuantity = itemDto.Items.Sum(i => i.Quantity);
                itemDto.TotalRemainingQuantity = itemDto.Items.Sum(i => i.RemainingQuantity);
            });

            if (!string.IsNullOrEmpty(keyword))
            {
                garmentSewingInItemDtoArray = garmentSewingInItemDto.Where(x => x.Product.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToArray();
                List<GarmentSewingInListDto> ListTemp = new List<GarmentSewingInListDto>();
                foreach (var a in garmentSewingInItemDtoArray)
                {
                    var temp = garmentSewingInDto.Where(x => x.Id.Equals(a.SewingInId)).ToArray();
                    foreach (var b in temp)
                    {
                        ListTemp.Add(b);
                    }
                }

                var garmentSewingInDtoList = garmentSewingInDto.Where(x => x.SewingInNo.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                    || x.Article.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                    || x.RONo.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                    || x.Unit.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                    || x.UnitFrom.Code.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                                    ).ToList();

                var i = 0;
                foreach (var data in ListTemp)
                {
                    i = 0;
                    foreach (var item in garmentSewingInDtoList)
                    {
                        if (data.Id == item.Id)
                        {
                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        garmentSewingInDtoList.Add(data);
                    }
                }
                var garmentSewingInDtoListArray = garmentSewingInDtoList.ToArray();
                if (order != "{}")
                {
                    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
                    garmentSewingInDtoListArray = QueryHelper<GarmentSewingInListDto>.Order(garmentSewingInDtoList.AsQueryable(), OrderDictionary).ToArray();
                }
                else
                {
                    garmentSewingInDtoListArray = garmentSewingInDtoList.OrderByDescending(x => x.LastModifiedDate).ToArray();
                }

                garmentSewingInDtoListArray = garmentSewingInDtoListArray.Take(size).Skip((page - 1) * size).ToArray();

                await Task.Yield();
                return Ok(garmentSewingInDtoListArray, info: new
                {
                    page,
                    size,
                    count = count
                });
            }
            else
            {
                //if (order != "{}")
                //{
                //    Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
                //    garmentSewingInDto = QueryHelper<GarmentSewingInListDto>.Order(garmentSewingInDto.AsQueryable(), OrderDictionary).ToArray();
                //}
                //else
                //{
                //    garmentSewingInDto = garmentSewingInDto.OrderByDescending(x => x.LastModifiedDate).ToArray();
                //}

                garmentSewingInDto = garmentSewingInDto.Take(size).Skip((page - 1) * size).ToArray();

                await Task.Yield();
                return Ok(garmentSewingInDto, info: new
                {
                    page,
                    size,
                    count = count
                });
            }
            //List<GarmentSewingInListDto> garmentSewingInListDtos = _garmentSewingInRepository.Find(query).Select(sewingIn =>
            //{
            //    var items = _garmentSewingInItemRepository.Query.Where(o => o.SewingInId == sewingIn.Identity).Select(sewingInItem => new
            //    {
            //        sewingInItem.ProductCode,
            //        sewingInItem.Quantity,
            //    }).ToList();

            //    return new GarmentSewingInListDto(sewingIn)
            //    {
            //        Products = items.Select(i => i.ProductCode).ToList(),
            //        TotalQuantity = items.Sum(i => i.Quantity),
            //    };
            //}).ToList();

            //await Task.Yield();
            //return Ok(garmentSewingInListDtos, info: new
            //{
            //    page,
            //    size,
            //    count
            //});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            Guid guid = Guid.Parse(id);

            VerifyUser();

            GarmentSewingInDto garmentSewingIn = _garmentSewingInRepository.Find(o => o.Identity == guid).Select(sewingIn => new GarmentSewingInDto(sewingIn)
            {
                Items = _garmentSewingInItemRepository.Find(o => o.SewingInId == sewingIn.Identity).Select(sewingInItem => new GarmentSewingInItemDto(sewingInItem)).ToList()
            }
            ).FirstOrDefault();

            await Task.Yield();
            return Ok(garmentSewingIn);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PlaceGarmentSewingInCommand command)
        {
            try
            {
                VerifyUser();

                var order = await Mediator.Send(command);

                return Ok(order.Identity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            VerifyUser();
            var garmentSewingInId = Guid.Parse(id);

            if (!Guid.TryParse(id, out Guid orderId))
                return NotFound();

            RemoveGarmentSewingInCommand command = new RemoveGarmentSewingInCommand(garmentSewingInId);

            var order = await Mediator.Send(command);

            return Ok(order.Identity);
        }
    }
}