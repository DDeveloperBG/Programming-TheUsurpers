﻿namespace App.Services.Data.UpdateRecords
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using App.Web.ViewModels.Register;

    public interface ICardHoldersService
    {
        public List<string> ValidateRegisterInput(RegisterInputModel input);

        public Task AddAsync(ulong paymentCardNumber, string paymentCardValidUntilText, string userId);

        IEnumerable<T> GetAllActiveDiscounts<T>();
    }
}
