using System.Reflection;
using RmMiddleware.Widgets.Mocks.CreateBayanAccountsJsonFromMmasChart;
using Newtonsoft.Json.Linq;

var lines = File.ReadAllLines(@"Z:\CreditLens\CL Integrations\Bayan Integration\Account Mapping\Mock.csv");

var mocks = lines.Select(line => line.Split(","))
   .Skip(1).Select(splits => new Mock
   {
      ITEM_COD = splits[0],
      DOC_COD = splits[1],
      Y2018 = splits[2],
      Y2017 = splits[3],
      Y2016 = splits[4]
   })
   .ToList();

var root = Assembly.GetExecutingAssembly().Location;
var path = Path.Combine(Path.GetDirectoryName(root) ?? string.Empty, "Mock.json");
var jObject = JObject.Parse(File.ReadAllText(path));

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
var jArray = (JArray)(
   jObject["CB_ME_ProductResponse"]?["ProductResponse"]?[0]?["CB_ME_ProductOutput"]?["B2BResponse"]?[
         "Product"]?
      ["BIRDATA"]?["FIN_INFO"]?["FIN_STAT"]?[0]?["FIT"]);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

for (int i = 0; i < 3; i++)
{
   jArray?.Clear();
   
   foreach (var mock in mocks)
   {
      var fit = new FIT();

      fit.DOC_COD = mock.DOC_COD;
      fit.ITEM_DESC = "Mock";
      fit.DOC_DESC = "Mock";
      fit.ITEM_COD = mock.ITEM_COD;
      fit.SRT = "5";
      fit.EC = "0000";
      fit.EV = i switch
      {
         0 => mock.Y2016,
         1 => mock.Y2017,
         _ => mock.Y2018
      };

      jArray?.Add(JToken.FromObject(fit));
   }

   File.WriteAllText(@"Z:\CreditLens\CL Integrations\Bayan Integration\Account Mapping\Mock" + i +".json",jObject?.ToString());
}