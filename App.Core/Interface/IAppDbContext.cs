using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface
{
    public interface IAppDbContext
    {
        DbSet<TEntity> Set<TEntity>()
         where TEntity : class;

     

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
