﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using EmailServerAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EmailServerAPI.Models
{
    public partial class pPrismMasterContext
    {
        private IpPrismMasterContextProcedures _procedures;

        public virtual IpPrismMasterContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new pPrismMasterContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public IpPrismMasterContextProcedures GetProcedures()
        {
            return Procedures;
        }

        protected void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
        }
    }

    public partial class pPrismMasterContextProcedures : IpPrismMasterContextProcedures
    {
        private readonly pPrismMasterContext _context;

        public pPrismMasterContextProcedures(pPrismMasterContext context)
        {
            _context = context;
        }

        public virtual async Task<int> EmailDeleterAsync(string param1, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "param1",
                    Size = 2,
                    Value = param1 ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.Database.ExecuteSqlRawAsync("EXEC @returnValue = [dbo].[EmailDeleter] @param1", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}
