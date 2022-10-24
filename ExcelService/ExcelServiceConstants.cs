using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelService
{
  public class ExcelServiceConstants
  {
    public const string DefaultPriceListFileName = "ProductsPriceList.xlsx";
    public const string DefaultPriceTagsFileName = "PriceTags.xlsx";
    public const string DefaultReportFileName = "Report.xlsx";
    public const string DefaultReportExcelSheetName = "ChangedPricesReport";
    public const string DefaultPriceTagsSheetName = "PriceTags";
    public const string DefaultPriceListSheetName = "PriceList";
    public static readonly string ExcelFilePath = Path.Combine(Environment.CurrentDirectory, "ProductsPriceList.xlsx");
    public static readonly string PriceTagsFilePath = Path.Combine(Environment.CurrentDirectory, "PriceTags.xlsx");
    public static readonly string reportFilePath = Path.Combine(Environment.CurrentDirectory, "Report.xlsx");
  }
}
