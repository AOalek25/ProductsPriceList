﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelService
{
  public class ExcelServiceConstants
  {
    public const string PriceListFileName = "PriceList.xlsx";
    public const string PriceListSheetName = "PriceList";
    public const string NewPriceListFileName = "NewProductsPriceList.xlsx";

    public const string PriceTagsFileName = "PriceTags.xlsx";
    public const string PriceTagsSheetName = "PriceTags";

    public const string ReportFileName = "ReportChangedPrices.xlsx";
    public const string ReportSheetName = "ReportChangedPrices";

    public const string BlazorData = "Data/";

    public static readonly string PriceListFilePath = Path.Combine(Environment.CurrentDirectory, PriceListFileName);
    public static readonly string PriceTagsFilePath = Path.Combine(Environment.CurrentDirectory, PriceTagsFileName);
    public static readonly string ReportFilePath = Path.Combine(Environment.CurrentDirectory, ReportFileName);
  }
}
