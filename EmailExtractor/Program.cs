
using EmailExtractor.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.ExceptionServices;
using System.Runtime.Loader;


// elimate any existing records from the email database
Console.Write("Please enter your first and last initial : ");
var initials = Console.ReadLine();
Console.Write("Now enter your email address in outlook : ");
var myemail = Console.ReadLine();
Console.Write("and finaly, your outlook email password : ");
var pw = Console.ReadLine();
if (myemail == null || myemail == "")
{
    myemail = "admin@prism-data.com";
    pw = "Cat$Waggle87";
}

if (initials != null)
{
    initials = initials.ToUpper().PadRight(2).Substring(0, 2);
    using (var context = new pPrismMasterContext())
    {
        var xx = await context.Procedures.EmailDeleterAsync(initials);
    }
}
else
{
    initials = "DQ";
    using (var context = new pPrismMasterContext())
    {
        var xx = await context.Procedures.EmailDeleterAsync("DQ");
    }
}


var app = new Microsoft.Office.Interop.Outlook.Application();
var ns = app.GetNamespace("MAPI");
ns.Logon(myemail, pw, false, false);
if (ns.Accounts.Count > 0)
{
    var pdppasd = 0;
}

Console.WriteLine("Getting Folders");
List<MAPIFolder> mf = new();

for (int i = 1; i <= ns.Folders.Count; i++)
{
    if (ns.Folders[i] is MAPIFolder)
    {
        MAPIFolder folder = ns.Folders[i];
        mf.AddRange(GetFolders(folder));
        folder = null;
    }
}

GC.Collect();

List<maileditem> addresses = new();
List<AddressEntry> inbox = new();
List<cc> CCs = new();
List<Recipients> aaz = new();


foreach (var folder in mf)
{
    if (!folder.Name.ToUpper().StartsWith("Sync Issues".ToUpper()))
    {
        Console.WriteLine("{0} - items({1})", folder.FullFolderPath, folder.Items.Count);
        try
        {
            Items oitems = folder.Items;
            for (int i = 1; i <= oitems.Count; i++)
            {
                // we will treat all items as sent and received ... obviously sent ones will have the sender as ourselves
                if (oitems[i] is MailItem)
                {
                    MailItem msel = (MailItem)oitems[i];
                    if ((DateTime.Now - msel.SentOn).TotalDays < 365 * 1.5)
                    {
                        aaz.Add(msel.Recipients);
                        inbox.Add(msel.Sender);
                        if (msel.CC != null)
                        {
                            CCs.AddRange(GetInetHeaders(msel));
                        }
                    }
                    if (i % 50 == 0)
                    {
                        GC.Collect();
                    }
                    msel = null;
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}

//Items oitems = inboxFolder.Items;
List<cc> addressesfound = new();

Console.WriteLine("Total added {0}.", aaz.Count);

foreach (Recipients recipient in aaz)
{
    foreach (Recipient rec in recipient)
    {
        addressesfound.Add(new() { Email = rec.Address, Name = rec.Name });
    }
}

Console.WriteLine("Total added {0}.", CCs.Count);
foreach (var addr in CCs)
{
    addressesfound.Add(addr);
}

Console.WriteLine("Total added {0}.", inbox.Count);
foreach (var entry in inbox)
{
    if (entry != null)
    {
        if (entry.Address != null && entry.Name != null)
        {
            addressesfound.Add(new() { Email = entry.Address, Name = entry.Name });
        }
        else if (entry.Address == null && entry.Name != null)
        {
            addressesfound.Add(new() { Email = "", Name = entry.Name });
        }
        else if (entry.Address != null && entry.Name == null)
        {
            addressesfound.Add(new() { Email = entry.Address, Name = "" });
        }
    }
    // if we got here without triggering an add, name and address are null, no point in adding this dude!
}

addressesfound = addressesfound.GroupBy(e => e.Email).Select(e => new cc() { Email = e.Key, Name = e.First().Name }).ToList();

using (var context = new pPrismMasterContext())
{
    foreach (var entry in addressesfound)
    {
        if (entry != null && entry.Name != null && entry.Email != null)
        {
            if (entry.Email.Length < 256 && entry.Name.Length < 256)
            {
                Console.WriteLine(entry.Email.PadRight(40) + entry.Name);
                var xx = await context.AutoEmail.AddAsync(new()
                {
                    Email = entry.Email,
                    User = initials,
                    Name = entry.Name,
                    MailDate = DateTime.Now,
                });
            }
        }
    }
    context.SaveChanges();
}
app.Quit();


return;



int count = 0;
int vcount = 0;

//for (int i = 1; i <= oitems.Count; i++)
//{
//    if (oitems[i] is MailItem)
//    {
//        MailItem msel = (MailItem) oitems[i];
//        if ((DateTime.Now - msel.SentOn).TotalDays < 365 * 1.5)
//        {
//            aaz.Add(msel.Recipients);
//            if (msel.CC != null)
//            {
//                CCs.AddRange(GetInetHeaders(msel));
//            }
//        }
//        if (i % 50 == 0)
//        {
//            GC.Collect();
//        }
//        msel = null;
//    }
//}
//Console.WriteLine("Items wrote to CC and to aaz: {0}, {1}",CCs.Count, aaz.Count);

//inboxFolder = ns.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);
//oitems = inboxFolder.Items;
//for (int i=1;i<oitems.Count;i++)
//{
//    if (oitems[i] is MailItem)
//    {
//        MailItem msel = (MailItem)oitems[i];
//        if ((DateTime.Now - msel.SentOn).TotalDays < 365 * 1.5)
//        {
//            inbox.Add(msel.Sender);
//            if (msel.CC != null)
//            {
//                CCs.AddRange(GetInetHeaders(msel));
//            }
//        }
//        msel = null;
//    }
//    vcount++;
//    if (vcount % 100 == 0)
//    {
//        GC.Collect();
//        Console.WriteLine("Items processed so far is {0}", i);
//    }
//}

//List<maileditem> addresses = new();
//List<AddressEntry> inbox = new();
//List<string> CCs = new();



using (var context = new pPrismMasterContext())
{

    foreach (var item in context.AutoEmail.Where(e => e.User == "DQ"))
    {
        context.AutoEmail.Remove(item);
    }

    foreach (var item in addresses)
    {
        var newrec = new AutoEmail()
        {
            Email = item.emailfield,
            MailDate = item.sentreceiveddate,
            Name = item.fullname,
            User = "DQ"
        };
        context.AutoEmail.Add(newrec);
    }
    context.SaveChanges();
}

using (var context = new pPrismMasterContext())
{
    // Deleting any formerly identified emails
    Console.WriteLine("Deleting previously identified email servers");
    foreach (var item in context.AutoEmailServers.Where(e => e.EmployeesOutlook == "DQ"))
    {
        context.AutoEmailServers.Remove(item);
    }
    Console.WriteLine("Creating a list of email servers from our email addresses");
    var mylist = context.AutoEmail
        .Where(e => e.MailDate > DateTime.Now.AddDays(-1.5 * 365))
        .ToList()
        .GroupBy(e => e.Email).Select(e => e.First())
        .ToList()
        .Select(e => e.Email.Replace("'", "").Trim().Split("@")[1].ToLower())
        .GroupBy(e => e)
        .OrderBy(e => e)
        .Select(e => e.Key)
        .ToList();
    Console.WriteLine("Adding {0} email servers sent to in past 1.5 years", mylist.Count());

    foreach (var item in mylist)
    {
        var newrec = new AutoEmailServers()
        {
            ServerName = item,
            EmployeesOutlook = "DQ"
        };
        context.AutoEmailServers.Add(newrec);
        Console.WriteLine(item);
    }
    context.SaveChanges();
}

return;


//foreach (Object sel in oitems)
//{
//    try
//    {
//        count++;
//        if (sel is MailItem)
//        {
//            MailItem msel = (MailItem)sel;
//            foreach (Recipient recip in msel.Recipients)
//            {
//                if (recip.Type == (int)OlMailRecipientType.olTo)
//                {
//                    addresses.Add(new(recip.Name, recip.Address, msel.SentOn, msel.Subject));
//                    Console.WriteLine(string.Format("Name: {0}, address: {1})", recip.Name, recip.Address));
//                }
//            }
//        }
//    }
//    catch (System.Exception)
//    {
//    }
//    if (count % 100 == 0)
//    {
//        Console.WriteLine(count);
//    }
//}

//Console.WriteLine("Inbox Count : {0}", addresses.Count);
//foreach (var kk in addresses)
//{
//    Console.WriteLine(kk.emailfield + " (on) " + kk.sentreceiveddate);
//}


//ns.Logoff();
//app.Quit();


List<cc> GetInetHeaders(MailItem olkMsg)
{
    string PR_TRANSPORT_MESSAGE_HEADERS = @"http://schemas.microsoft.com/mapi/proptag/0x007D001E";
    PropertyAccessor? olkPA = olkMsg.PropertyAccessor;
    var thisreturn = olkPA.GetProperty(PR_TRANSPORT_MESSAGE_HEADERS);
    List<cc> kreturn = new();
    using (StringReader sr = new StringReader(thisreturn))
    {
        var line = sr.ReadLine();
        while (line != null)
        {
            if (line.ToUpper().StartsWith("CC: "))
            {
                // we are on a cc line ....
                var lineextract = line.Substring(4);
                while (lineextract.EndsWith(","))
                {
                    lineextract += sr.ReadLine().Substring(1);
                }
                // we have a lineextract which contains commas, sometimes inside quotes, sometimes not.  We cant use split on 
                // a comma, we need to manually split

                var parsedccs = fallsplit(lineextract);
                foreach (var addr in parsedccs)
                {
                    kreturn.Add(new(addr));
                }
                //Console.WriteLine(lineextract);
            }
            line = sr.ReadLine();
        }
    }
    olkPA = null;
    return kreturn;
}


string[] fallsplit(string linein)
{
    List<int> commas = new();
    bool insidequote = false;

    for (int i = 0; i < linein.Length; i++)
    {
        if (!insidequote && linein.Substring(i, 1) == ",")
        {
            commas.Add(i);
        }
        else if (!insidequote && linein.Substring(i, 1) == "\"")
        {
            insidequote = true;
        }
        else if (insidequote && linein.Substring(i, 1) == "\"")
        {
            insidequote = false;
        }
    }
    List<string> items = new();
    int start = 0;
    int len = 0;
    foreach (int pos in commas)
    {
        len = pos - start;
        items.Add(linein.Substring(start, len).Trim());
        start = pos + 1;
    }
    items.Add(linein.Substring(start, linein.Length - start).Trim());
    return items.ToArray();
}


List<MAPIFolder> GetFolders(MAPIFolder folder)
{
    List<MAPIFolder> retnonsense = new();
    foreach (MAPIFolder subFolder in folder.Folders)
    {
        retnonsense.AddRange(GetFolders(subFolder));
    }
    retnonsense.Add(folder);
    return retnonsense;
}

class cc
{
    string? name = null;
    string? email = null;
    public string? Email
    {
        get
        { return email; }
        set
        { email = value; }
    }
    public string? Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }
    public cc()
    {
        email = null;
        name = null;
    }
    public cc(string inputheader)
    {
        var emailstart = inputheader.IndexOf("<");
        var emailend = inputheader.IndexOf(">");
        if (emailstart != -1 && emailend != -1)
        {
            email = inputheader.Substring(emailstart + 1, emailend - emailstart - 1);
        }
        else
        {
            email = null;
        }
        if (emailstart > 0)
        {
            name = inputheader.Substring(0, emailstart - 1).Replace("\"", "");
        }
        else
        {
            name = null;
        }
    }
}
class maileditem
{
    public string emailfield { get; set; }
    public DateTime sentreceiveddate { get; set; }
    public string fullname { get; set; }
    public string subject { get; set; }
    public maileditem(string fullname, string emailfield, DateTime timesent, string subject)
    {
        this.emailfield = emailfield;
        this.sentreceiveddate = timesent;
        this.fullname = fullname;
        this.subject = subject;
    }
}




