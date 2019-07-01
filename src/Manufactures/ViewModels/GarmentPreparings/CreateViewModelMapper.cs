﻿using Manufactures.Domain.GarmentPreparings;
using Manufactures.Domain.GarmentPreparings.ValueObjects;
using Manufactures.Dtos;
using System;

namespace Manufactures.ViewModels.GarmentPreparings
{
    public class CreateViewModelMapper
    {
        public GarmentPreparing Map(CreateViewModel viewModel)
        {
            return new GarmentPreparing(Guid.NewGuid(), viewModel.UENId, viewModel.UENNo, new UnitDepartmentId(viewModel.UnitId), viewModel.UnitCode, viewModel.UnitName, viewModel.ProcessDate,
               viewModel.RONo, viewModel.Article, viewModel.IsCuttingIn);
        }

        public GarmentPreparingItem MapItem(GarmentPreparingItemDto viewModel, Guid headerId)
        {
            return new GarmentPreparingItem(Guid.NewGuid(), viewModel.UENItemId, new ProductId(viewModel.Product.Id), viewModel.Product.Code, viewModel.Product.Name, viewModel.DesignColor, viewModel.Quantity, new UomId(viewModel.Uom.Id), viewModel.Uom.Unit, viewModel.FabricType, viewModel.RemainingQuantity, viewModel.BasicPrice, headerId);
        }
    }
}