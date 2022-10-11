using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducstLibrary
{
  public class ProductLibConstants
  {
    public const string DefaultExcelFileName = "ProductsPriceList.xlsx";
    public const string DefaultReportExcelSheetName = "ChangedPricesReport.xlsx";
    public const string DefaultPriceTagsSheetName = "PriceTags";
    public const string DefaultPriceListSheetName = "PriceList";
    public const string _alreadyExistMsg = "Такой продукт уже есть в списке";
    public const string _notFoundMsg = "Продукт с таким именем и фамилией не найден";
  }
}
