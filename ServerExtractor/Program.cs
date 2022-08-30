using EmailExtractor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;


using (var context = new pPrismMasterContext())
{
    context.Database.ExecuteSqlRaw("DELETE FROM dbo.AutoEmailServers WHERE [EmployeesOutlook] = 'KZ'");
    var xx = (await context.AutoEmail.ToListAsync())
        .Where(e => e.Email.Split("@").Count() == 2 && e.User == "KZ")
        .OrderBy(e => e.Email.Split("@")[1])
        .GroupBy(e => e.Email.Split("@")[1].ToUpper())
        .Select(e => new { server = e.Key, employee = e.First().User } )
        .ToList();
    var zz = xx
        .Where(e => context.AutoEmailServers.Where(z => z.ServerName == e.server).Count() == 0)
        .ToList();
    foreach (var yy in zz)
    {
        context.AutoEmailServers.Add(new()
        {
            ServerName = yy.server,
            EmployeesOutlook = yy.employee,
            Verified = false
        });
        Console.WriteLine(yy);
    }
    context.SaveChanges();
}

