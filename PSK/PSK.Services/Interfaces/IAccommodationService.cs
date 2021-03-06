﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;

namespace PSK.Services.Interfaces
{
    public interface IAccommodationService
    {
        Task<IEnumerable<AccommodationDto>> GetAvailableAccommodations(Guid tripId);
    }
}