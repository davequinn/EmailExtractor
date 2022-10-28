// See https://aka.ms/new-console-template for more information
using ConsoleApp1.Models;

Console.WriteLine("Hello, World!");
using (var contx = new pPrismMasterContext())
{
    var kinbus = contx.AutoEmail.ToList()
        .OrderBy(x => x.Email)
        .ThenByDescending(x => x.Name)
        .ThenByDescending(x => x.User)
        .GroupBy(e => e.Email.ToUpper())
        //.Select(e => new { e.Key, myitems = e.ToList() })
        //.Select(e => e.myitems.First())
        .ToList();
    foreach (var mmm in kinbus)
    {
        Console.WriteLine(mmm.First().Email);
    }
    for (int i = 0; i < kinbus.Count; i++)
    {
        //Console.WriteLine(kinbus[i].Name);
    }
}
