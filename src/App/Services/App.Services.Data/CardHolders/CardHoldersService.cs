﻿namespace App.Services.Data.UpdateRecords
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using App.Data.Common.Repositories;
    using App.Data.Models;
    using App.Services.Mapping;
    using App.Web.ViewModels.Register;

    public class CardHoldersService : ICardHoldersService
    {
        private readonly IRepository<CardHolder> cardHolders;
        private readonly IRepository<Discount> discounts;

        public CardHoldersService(
            IRepository<CardHolder> cardHolders,
            IRepository<Discount> discounts)
        {
            this.cardHolders = cardHolders;
            this.discounts = discounts;
        }

        public List<string> ValidateRegisterInput(RegisterInputModel input)
        {
            List<string> errors = new List<string>();

            if (!(input.PhoneNumber.StartsWith("+359") || input.PhoneNumber.StartsWith('0')))
            {
                errors.Add("Phone number must start with +359 or 0");
            }
            else
            {
                var withoutBigStart = input.PhoneNumber.Replace("+359", "0");
                if (withoutBigStart.Length != 10)
                {
                    errors.Add("Phone number length must be 10");
                }
            }

            if (input.PaymentCardValidUntil.Length != 5)
            {
                errors.Add("Payment Card valid thru is invalid");
            }
            else
            {
                var validThruParts = input.PaymentCardValidUntil.Split('/');
                if (validThruParts.Any(x => !int.TryParse(x, out int _)))
                {
                    errors.Add("Payment Card valid thru is invalid");
                }
                else
                {
                    int month = int.Parse(input.PaymentCardValidUntil[..2]);
                    int year = int.Parse(input.PaymentCardValidUntil[3..]);

                    if (!(month > 0 && month < 13 && year >= (DateTime.Now.Year % 100) && year < 99))
                    {
                        errors.Add("Payment Card valid thru is invalid");
                    }
                }
            }

            return errors;
        }

        public async Task AddAsync(ulong paymentCardNumber, string paymentCardValidUntilText, string userId)
        {
            int month = int.Parse(paymentCardValidUntilText[..2]);
            int year = int.Parse(paymentCardValidUntilText[3..]);

            var paymentCardValidUntil = new DateTime(year, month, 1);

            var cardHolder = new CardHolder
            {
                PaymentCardValidUntil = paymentCardValidUntil,
                PaymentCardNumber = paymentCardNumber,
                RegisteredOn = DateTime.UtcNow,
                UserId = userId,
            };

            await this.cardHolders.AddAsync(cardHolder);
            await this.cardHolders.SaveChangesAsync();
        }

        public IEnumerable<T> GetAllActiveDiscounts<T>()
        {
            var nowUtc = DateTime.UtcNow;

            return this.discounts
                .AllAsNoTracking()
                .Where(x => x.Status == DiscountStatus.Active)
                .Where(x => x.StartDate <= nowUtc && nowUtc <= x.EndDate)
                .To<T>()
                .ToList();
        }
    }
}
