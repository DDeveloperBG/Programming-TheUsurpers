﻿namespace App.Services.Data.DTOs
{
    using System;

    public class BankEmployeeNewRecordDTO
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
