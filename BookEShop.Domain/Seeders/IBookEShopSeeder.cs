using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookEShop.Domain;

namespace BookEShop.Domain.Seeders;

public interface IBookEShopSeeder
{
    Task Seed();
}
