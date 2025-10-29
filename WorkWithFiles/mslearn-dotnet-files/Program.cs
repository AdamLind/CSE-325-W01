using Newtonsoft.Json; 

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);
var salesSummary = GenerateSummaryReport(salesFiles, salesTotal);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");
File.AppendAllText(Path.Combine(salesTotalDir, "salesSummary.txt"), $"{salesSummary}{Environment.NewLine}");

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

string GenerateSummaryReport(IEnumerable<string> salesFiles, double salesTotal)
{
    var details = new List<(string Folder, string FileName, double Total)>();


    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);


        // Deserialize JSON file into SalesData object
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);


        if (data != null)
        {
            // Get filename and parent folder
            string fileName = Path.GetFileName(file);
            string folderName = Path.GetFileName(Path.GetDirectoryName(file)!);


            details.Add((folderName, fileName, data.Total));
        }
    }


    // Build summary string
    string summary = @$"Sales Summary
----------------------------
 Total Sales: {salesTotal:C}


 Details:
{string.Join(Environment.NewLine, details.Select(d => $"  {d.Folder}\\{d.FileName}: {d.Total:C}"))}
";


    return summary;
}


record SalesData(double Total);