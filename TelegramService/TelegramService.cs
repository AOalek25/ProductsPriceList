using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using ProducstLibrary.Model;
using ProducstLibrary;
using ProducstLibrary.Exceptions;

namespace TelegramBOT
{
  internal class TelegramService
  {
    private static TelegramBotClient client = new("5778299393:AAFVASD3aJhUSUZLKceKl4h9OUEt_pXNSBY");
    private static readonly ProductsRepo<Product> ProductsBook = new();

    public static void StartMessenger()
    {
      Console.WriteLine("Запущен бот " + client.GetMeAsync().Result.FirstName);
      var cts = new CancellationTokenSource();
      var cancellationToken = cts.Token;
      var receiverOptions = new ReceiverOptions { AllowedUpdates = { }, };
      client.StartReceiving
      (
          HandleUpdateAsync,
          HandleErrorAsync,
          receiverOptions,
          cancellationToken
      );
      Console.ReadLine();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      Console.WriteLine(JsonConvert.SerializeObject(update));
      if (update.Type == UpdateType.Message && update.Message != null)
      {
        await HandleMessage(botClient, update.Message);
        return;
      }
      if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
      {
        await HandleCallBackQuery(botClient, update.CallbackQuery);
        return;
      }
    }

    private static async Task HandleMessage(ITelegramBotClient botClient, Message message)
    {
      if (message.Text?.ToLower() == "/start")
      {
        await botClient.SendTextMessageAsync(message.Chat.Id, text: $"Привет, {message.Chat.FirstName}!");
        InlineKeyboardMarkup keyboard = new(new[]
         {
          new[]
          {
            InlineKeyboardButton.WithCallbackData(text: "Распечатать прайс-лист", callbackData: "PrintStudents"),
            InlineKeyboardButton.WithCallbackData(text: "Добавить новый продукт", callbackData: "AddStudent")
          }
        });
        await botClient.SendTextMessageAsync(message.Chat.Id, text: $"Выбери команду:", replyMarkup: keyboard);
        return;
      }
      if (message.Text != null && message.Text.StartsWith("Add"))
      {
        string[] values = message.Text.Split(' ');
        if ((values.Length == 5) && decimal.TryParse(values[3], out decimal price))
          try
          {
            ProductsBook.Create(new Product(values[1], values[2], price));
            await botClient.SendTextMessageAsync(message.Chat.Id, text: $"Продукт {values[1]} {values[2]} успешно добавлен");
            return;
          }
          catch (ProductAlreadyExistException ex)
          {
            await botClient.SendTextMessageAsync(message.Chat.Id, text: $" Ошибка: {ex.Message}");
            return;
          }
          catch (ArgumentException ex)
          {
            await botClient.SendTextMessageAsync(message.Chat.Id, text: $" Ошибка: {ex.Message}");
            return;
          }
      }
      await botClient.SendTextMessageAsync(message.Chat.Id, text: $"Такую команду я не знаю:\n{message?.Text} :( ");
      return;
    }

    private static async Task HandleCallBackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
      if (callbackQuery.Data != null && callbackQuery.Message != null && callbackQuery.Data.StartsWith("PrintPriceList"))
      {
        foreach (Product student in ProductsBook)
          await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: student.PrintInfo());
        return;
      }
      if (callbackQuery.Data != null && callbackQuery.Message != null && callbackQuery.Data.StartsWith("AddProduct"))
      {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "Введите данные о продукте командой: Add Название поризодитель цена");
        return;
      }
      return;
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      Console.WriteLine(JsonConvert.SerializeObject(exception));
      return Task.CompletedTask;
    }


  }
}
