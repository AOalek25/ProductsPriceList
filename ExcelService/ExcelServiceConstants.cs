using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelService
{
  public class ExcelServiceConstants
  {
    public const string DefaultExcelFileName = "ProductsPriceList.xlsx";
    public const string DefaultReportExcelSheetName = "ChangedPricesReport";
    public const string DefaultPriceTagsSheetName = "PriceTags";
    public const string DefaultPriceListSheetName = "PriceList";
    public static readonly string ExcelFilePath = Path.Combine(Environment.CurrentDirectory, "ProductsPriceList.xlsx");
  }
}
