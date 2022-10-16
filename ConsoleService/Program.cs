using ConsoleService;

ConsolService consoleService = new();

do
{
  ConsolService.PrintMenu();
  switch (Console.ReadLine())
  {
    case ConsoleConstants.readProduct:
      consoleService.Read();
      break;
    case ConsoleConstants.updateProduct:
      consoleService.Update();
      break;
    case ConsoleConstants.createProduct:
      consoleService.Create();
      break;
    case ConsoleConstants.deleteProduct:
      consoleService.Delete();
      break;
    case ConsoleConstants.printAllProducts:
      consoleService.PrintAll();
      break;
    case ConsoleConstants.GenerateReportChangedPrices:
      consoleService.GenerateReportChangedPrices();
      break;    
    case ConsoleConstants.PrintAllPriceTags:
      consoleService.PrintAllPriceTags();
      break;
    case ConsoleConstants.LoadNewPriceList:
      consoleService.LoadNewPriceList();
      break;
    case ConsoleConstants.SortByName:
      consoleService.SortByName();
      break;
    case ConsoleConstants.SortByPrice:
      consoleService.SortByPrice();
      break;
    case ConsoleConstants.exit:
      return;
    default:
      ConsolService.ShowMessage(ConsoleConstants.warning);
      break;
  }
} while (true);




