using EmailExtractor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;
using Windows.UI.WindowManagement;

await EliminateServers();
return;

Console.Write("Please enter your initials or * for all: ");
var initials = Console.ReadLine();
if (initials == null || initials == "")
{
    initials = "DQ";
}
else if (initials != "*")
{
    initials = initials.ToUpper().PadRight(2).Substring(0,2);
}
using (var context = new pPrismMasterContext())
{
    var existingservers = (await context.AutoEmailServers.ToListAsync()).Where(e => e.EmployeesOutlook == initials);
    if (existingservers.Any())
    {
        Console.WriteLine("/nDeleting {0} servers that existed for {1}",existingservers.Count(),initials);
        context.Database.ExecuteSqlRaw("DELETE FROM dbo.AutoEmailServers WHERE [EmployeesOutlook] = '" + initials + "'");
    }
    var xx = (await context.AutoEmail.ToListAsync())
        .Where(e => e.Email.Split("@").Count() == 2 && e.User == initials)
        .OrderBy(e => e.Email.Split("@")[1])
        .GroupBy(e => e.Email.Split("@")[1].ToUpper())
        .Select(e => new { server = e.Key, employee = e.First().User } )
        .ToList();
    var zz = xx
        .Where(e => context.AutoEmailServers.Where(z => z.ServerName == e.server).Count() == 0)
        .ToList();
    Console.WriteLine("Attempting to add {0} new servers, {1} existed on someone else's emails", zz.Count, xx.Count - zz.Count);
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

static async Task EliminateServers()
{
    using (var context = new pPrismMasterContext())
    {
        var eservers = (await context.AutoEmailServers.ToListAsync()).Select(e => new { aServerName = e.ServerName.ToUpper() }).ToList();
        var ABC = await context.AutoEmail.ToListAsync();
        var emails = (await context.AutoEmail.ToListAsync()).Select(e => new 
        { 
            Name = e.Name, 
            Email = e.Email,
            Subject = e.Subject,
            MailDate = e.MailDate,
            User = e.User,
            aServerName = e.Email.ToUpper().Split("@").Last()});
        var both = from e in emails
                   join s in eservers on e.aServerName equals s.aServerName into jemail
                   from j in jemail.DefaultIfEmpty()
                   select new AutoEmailserverVerified
                   {
                       Email = e.Email,
                       Name = e.Name,
                       User = e.User,
                       MailDate = e.MailDate,
                       Subject = e.Subject,
                       ServerName = j?.aServerName ?? String.Empty
                   };

        context.AutoEmailserverVerifieds.AddRange(both);
        int pp = 5;                   
        //var join = from eml in emails

        //           join srv in eservers on eml.Email.Split("@").Last().ToLower() equals srv.ServerName.ToLower() into joinedList
        //           select new { emails, servers = joinedList.Select(e => e.ServerName) };
        //join.ToList().ForEach(dd => Console.WriteLine(dd.emails))
        //var JoinedList = (await context.AutoEmailServers.ToListAsync())
        //    .Join((await context.AutoEmail.ToListAsync()),
        //    autoemailservers => autoemailservers.ServerName.ToLower(),
        //    autoemail => autoemail.Email.Split("@").LastOrDefault().ToLower(),
        //    (autoemailservers, autoemail) => new
        //    {
        //        Name = autoemail.Name,
        //        Email = autoemail.Email,
        //        Subject = autoemail.Subject,
        //        Maildate = autoemail.MailDate,
        //        User = autoemail.User,
        //        Server = autoemailservers.ServerName
        //    }).ToList();
        //var existingservers = (await context.AutoEmailServers.ToListAsync()).Where(e => e.EmployeesOutlook == initials);
        //if (existingservers.Any())
        //{
        //    Console.WriteLine("/nDeleting {0} servers that existed for {1}", existingservers.Count(), initials);
        //    context.Database.ExecuteSqlRaw("DELETE FROM dbo.AutoEmailServers WHERE [EmployeesOutlook] = '" + initials + "'");
        //}
        //var xx = (await context.AutoEmail.ToListAsync())
        //    .Where(e => e.Email.Split("@").Count() == 2 && e.User == initials)
        //    .OrderBy(e => e.Email.Split("@")[1])
        //    .GroupBy(e => e.Email.Split("@")[1].ToUpper())
        //    .Select(e => new { server = e.Key, employee = e.First().User })
        //    .ToList();
        //var zz = xx
        //    .Where(e => context.AutoEmailServers.Where(z => z.ServerName == e.server).Count() == 0)
        //    .ToList();
        //Console.WriteLine("Attempting to add {0} new servers, {1} existed on someone else's emails", zz.Count, xx.Count - zz.Count);
        //foreach (var yy in zz)
        //{
        //    context.AutoEmailServers.Add(new()
        //    {
        //        ServerName = yy.server,
        //        EmployeesOutlook = yy.employee,
        //        Verified = false
        //    });
        //    Console.WriteLine(yy);
        //}
        //context.SaveChanges();
    }

}