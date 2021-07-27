﻿using Infrastructure.Data.EntityFrameworkCore;
using Infrastructure.Data.EntityFrameworkCore.Utilities;
using Manufactures.Domain.GarmentExpenditureGoods;
using Manufactures.Domain.GarmentExpenditureGoods.ReadModels;
using Manufactures.Domain.GarmentExpenditureGoods.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufactures.Data.EntityFrameworkCore.GarmentExpenditureGoods.Repositories
{
    public class GarmentExpenditureGoodRepository : AggregateRepostory<GarmentExpenditureGood, GarmentExpenditureGoodReadModel>, IGarmentExpenditureGoodRepository
    {
        public IQueryable<GarmentExpenditureGoodReadModel> Read(int page, int size, string order, string keyword, string filter)
        {
            var data = Query;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            data = QueryHelper<GarmentExpenditureGoodReadModel>.Filter(data, FilterDictionary);

            List<string> SearchAttributes = new List<string>
            {
                "ExpenditureGoodNo",
                "ExpenditureType",
                "Article",
                "RONo",
                "UnitCode",
                "UnitName",
                "ContractNo",
                "Invoice",
                "BuyerName"
            };
            data = QueryHelper<GarmentExpenditureGoodReadModel>.Search(data, SearchAttributes, keyword);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            data = OrderDictionary.Count == 0 ? data.OrderByDescending(o => o.ModifiedDate) : QueryHelper<GarmentExpenditureGoodReadModel>.Order(data, OrderDictionary);

            //data = data.Skip((page - 1) * size).Take(size);

            return data;
        }

        public IQueryable<GarmentExpenditureGoodReadModel> ReadComplete(int page, int size, string order, string keyword, string filter)
        {
            var data = Query;

            Dictionary<string, object> FilterDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(filter);
            data = QueryHelper<GarmentExpenditureGoodReadModel>.Filter(data, FilterDictionary);

            List<string> SearchAttributes = new List<string>
            {
                "RONo",
            };
            data = QueryHelper<GarmentExpenditureGoodReadModel>.Search(data, SearchAttributes, keyword);

            Dictionary<string, string> OrderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
            data = OrderDictionary.Count == 0 ? data.OrderByDescending(o => o.ModifiedDate) : QueryHelper<GarmentExpenditureGoodReadModel>.Order(data, OrderDictionary);

            //data = data.Skip((page - 1) * size).Take(size);

            return data;
        }

        public IQueryable<object> ReadExecute(IQueryable<GarmentExpenditureGoodReadModel> query) {
            var newQuery = query.Select(garmentExpenditureGood => new
            {
                Id = garmentExpenditureGood.Identity,
                ExpenditureGoodNo = garmentExpenditureGood.ExpenditureGoodNo,
                RONo = garmentExpenditureGood.RONo,
                Article = garmentExpenditureGood.Article,
                Unit = new
                {
                    Id = garmentExpenditureGood.UnitId,
                    Code = garmentExpenditureGood.UnitCode,
                    Name = garmentExpenditureGood.UnitName
                },
                ExpenditureDate = garmentExpenditureGood.ExpenditureDate,
                ExpenditureType = garmentExpenditureGood.ExpenditureType,
                Comodity = new
                {
                    Id = garmentExpenditureGood.ComodityId,
                    Code = garmentExpenditureGood.ComodityCode,
                    Name = garmentExpenditureGood.ComodityName
                },
                Buyer = new
                {
                    Id = garmentExpenditureGood.BuyerId,
                    Code = garmentExpenditureGood.BuyerCode,
                    Name = garmentExpenditureGood.BuyerName
                },
                Invoice = garmentExpenditureGood.Invoice,
                ContractNo = garmentExpenditureGood.ContractNo,
                Carton = garmentExpenditureGood.Carton,
                Description = garmentExpenditureGood.Description,
                IsReceived = garmentExpenditureGood.IsReceived,
                PackingListId = garmentExpenditureGood.PackingListId,

                Items = garmentExpenditureGood.Items.Select(garmentExpenditureGoodItem => new {
                    Id = garmentExpenditureGoodItem.Identity,
                    ExpenditureGoodId = garmentExpenditureGoodItem.ExpenditureGoodId,
                    FinishedGoodStockId = garmentExpenditureGoodItem.FinishedGoodStockId,
                    Size = new
                    {
                        Id = garmentExpenditureGoodItem.SizeId,
                        Size = garmentExpenditureGoodItem.SizeName,
                    },
                    Quantity = garmentExpenditureGoodItem.Quantity,
                    Uom = new
                    {
                        Id = garmentExpenditureGoodItem.UomId,
                        Unit = garmentExpenditureGoodItem.UomUnit
                    },
                    Description = garmentExpenditureGoodItem.Description,
                    BasicPrice = garmentExpenditureGoodItem.BasicPrice,
                    Price = garmentExpenditureGoodItem.Price,
                    ReturQuantity = garmentExpenditureGoodItem.ReturQuantity,

                })

            });
            return newQuery;
        }

        protected override GarmentExpenditureGood Map(GarmentExpenditureGoodReadModel readModel)
        {
            return new GarmentExpenditureGood(readModel);
        }
    }
}
