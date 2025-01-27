﻿namespace DWH.Controllers
{
    using System;
    using System.Threading.Tasks;

    using DWH.Data.Common.Repositories;
    using DWH.Data.Models;
    using DWH.DTOs;
    using DWH.Services.Hash;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class ShopkeeperController : ControllerBase
    {
        private const string AccessToken = "204569a39efaa36c1";
        private readonly IRepository<Shopkeeper> shopkeepers;
        private readonly IHashService hashService;

        public ShopkeeperController(
            IRepository<Shopkeeper> shopkeepers,
            IHashService hashService)
        {
            this.shopkeepers = shopkeepers;
            this.hashService = hashService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewAsync(
            [FromQuery]
            string accessToken,
            [FromBody]
            ShopkeeperInputModel input)
        {
            if (accessToken != AccessToken)
            {
                return this.BadRequest("Access token is invalid!");
            }

            var hashedPassword = this.hashService.HashText(input.Password);
            var shopkeeper = new Shopkeeper
            {
                Username = input.Username,
                Email = input.Email,
                Password = hashedPassword,
                PhoneNumber = input.PhoneNumber,
                RegisteredOn = DateTime.UtcNow,
            };

            await this.shopkeepers.AddAsync(shopkeeper);
            await this.shopkeepers.SaveChangesAsync();

            return this.Ok();
        }
    }
}
