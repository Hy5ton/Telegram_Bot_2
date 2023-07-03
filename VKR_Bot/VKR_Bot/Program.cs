using System;
using System.IO.Ports;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using VKR_Bot;
using System.Drawing;
using QuickChart;
using System.Diagnostics.Metrics;
using Telegram.Bot.Types.ReplyMarkups;
using System.Buffers;
using System.Threading.Tasks;
using System.Threading;

namespace VKR_Bot
{
    class Programm
    {
        static SerialPort serialPort;
        public static string username = string.Empty;
        public static string soil_moisture = string.Empty;
        public static string temperature = string.Empty;
        public static string date = string.Empty;
        public static string time = string.Empty;
        public static string dataFromDataBase = string.Empty;
        public static int[] number = new int[24];
        public static string[]? dateArray = new string[24];
        public static string[]? timeArray = new string[24];
        public static string[]? temperatureArray = new string[24];
        public static string[]? moisureArray = new string[24];
        public static int[] numberOfWeek = new int[168];
        public static string[]? dateArrayOfWeek = new string[168];
        public static string[]? timeArrayOfWeek = new string[168];
        public static string[]? temperatureArrayOfWeek = new string[168];
        public static string[]? moisureArrayOfWeek = new string[168];
        public static string msg = string.Empty;
        public static string equipmentPosition = string.Empty;
        public static int labelTemp;
        public static int labelSoil;
        public static int errorCount = 1;
        public static int paragraphTemp = 0;
        public static int paragraphSoil;

        static void ReadSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.PortName = "COM9";
            serialPort.BaudRate = 9600;
            serialPort.Open();
        }
        static void Main(string[] args)
        {
            ReadSerialPort();
            var client = new TelegramBotClient("5645372685:AAGURI3IqzHLEN6MwvRGSoJU3q7f9xSoYsg");
            client.StartReceiving(Update, Error);
            int num = 0;
            TimerCallback tm = new TimerCallback(convertTime);
            Timer timer = new Timer(tm, num, 1000, 1000);
            object obj = null;
            Console.ReadLine();
        }
        
        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if(message.Text != null)
            {
                Console.WriteLine($"{message.Chat.Username ?? "анон"}  |  {message.Text}");
                username = message.Chat.Username;
                if (message.Text.ToLower().Contains("hello"))
                {
                    DBcommand cmd = new DBcommand();
                    cmd.readLastDay(username, date, time, temperature, soil_moisture, number, dateArray, timeArray, 
                        temperatureArray, moisureArray);
                    cmd.readLastWeek(username, date, time, temperature, soil_moisture, numberOfWeek, dateArrayOfWeek, timeArrayOfWeek,
                        temperatureArrayOfWeek, moisureArrayOfWeek);
                    return;
                }
                if (message.Text.ToLower().Contains("table"))
                {
                    editText();
                    DBcommand cmd = new DBcommand();
                    cmd.readParametrs();
                    return;
                }
                if (message.Text.ToLower().Contains("grafofdaytemp"))
                {
                    DBcommand cmd = new DBcommand();
                    cmd.readLastDay(username, date, time, temperature, soil_moisture, number, dateArray, timeArray,
                        temperatureArray, moisureArray);
                    createGrafAboutLastDayTemp();
                    try
                    {
                        await using Stream stream = System.IO.File.OpenRead("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/tempOfDay.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            photo: InputFile.FromStream(stream: stream, fileName: "tempOfDay.png"),
                            caption: "График изменения температуры за последние 24 часа");
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отсутствует статистика за последние 24 часа");
                    }
                    return;
                }
                if (message.Text.ToLower().Contains("grafofdaysoil"))
                {
                    DBcommand cmd = new DBcommand();
                    cmd.readLastDay(username, date, time, temperature, soil_moisture, number, dateArray, timeArray,
                        temperatureArray, moisureArray);
                    createGrafAboutLastDaySoil();
                    try
                    {
                        await using Stream stream = System.IO.File.OpenRead("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/soilOfDay.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            photo: InputFile.FromStream(stream: stream, fileName: "soilOfDay.png"),
                            caption: "График изменения влажности почвы за последние 24 часа");
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отсутствует статистика за последние 24 часа");
                    }
                    return;
                }
                if (message.Text.ToLower().Contains("grafofweektemp"))
                {
                    DBcommand cmd = new DBcommand();
                    cmd.readLastWeek(username, date, time, temperature, soil_moisture, numberOfWeek, dateArrayOfWeek, timeArrayOfWeek,
                        temperatureArrayOfWeek, moisureArrayOfWeek);
                    try
                    {
                        createGrafAboutLastWeekTemp();
                        await using Stream stream = System.IO.File.OpenRead("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/tempOfWeek.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            photo: InputFile.FromStream(stream: stream, fileName: "tempOfWeek.png"),
                            caption: "График изменения температуры за последнюю неделю");
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отсутствует статистика за неделю");
                    }
                    return;
                }
                if (message.Text.ToLower().Contains("grafofweeksoil"))
                {
                    DBcommand cmd = new DBcommand();
                    cmd.readLastWeek(username, date, time, temperature, soil_moisture, numberOfWeek, dateArrayOfWeek, timeArrayOfWeek,
                        temperatureArrayOfWeek, moisureArrayOfWeek);
                    try
                    {
                        createGrafAboutLastWeekSoil();
                        await using Stream stream = System.IO.File.OpenRead("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/soilOfWeek.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            photo: InputFile.FromStream(stream: stream, fileName: "soilOfWeek.png"),
                            caption: "График изменения влажности почвы за последнюю неделю");
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Отсутствует статистика за неделю");
                    }
                    return;
                }
                if (message.Text.ToLower().Contains("status"))
                {
                    editText();
                    await botClient.SendTextMessageAsync(message.Chat.Id, temperature + " Temperature");
                    await botClient.SendTextMessageAsync(message.Chat.Id, int.Parse(soil_moisture) - 130 + "% Soil moisture");
                    if (labelTemp == 0)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Система вентиляции не активна");
                    }
                    else if (labelTemp == 1)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Система вентиляции активна");
                    }
                    if (labelSoil == 0)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Система полива не активна");
                    }
                    else if (labelSoil == 1)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Система полива активна");
                    }
                    DBcommand cmd = new DBcommand();
                    return;
                }
            }
        }
        
        public static void editText()
        {
            msg.Remove(0);
            Thread.Sleep(1000);
            while (errorCount == 1)
            {
                try
                {
                    msg = serialPort.ReadExisting();
                    equipmentPosition = msg;
                    equipmentPosition = equipmentPosition.Remove(38);
                    labelTemp = int.Parse(equipmentPosition.Remove(0, 37));
                    equipmentPosition = msg;
                    equipmentPosition = equipmentPosition.Remove(37);
                    labelSoil = int.Parse(equipmentPosition.Remove(0, 36));
                    msg = msg.Remove(36);
                    temperature = msg.Remove(34);
                    temperature = temperature.Remove(0, 32);
                    soil_moisture = msg.Remove(0, 14);
                    soil_moisture = soil_moisture.Remove(4);
                    date = DateOnly.FromDateTime(DateTime.Now).ToString();
                    time = TimeOnly.FromDateTime(DateTime.Now).ToString();
                    msg = msg.Remove(0);
                    errorCount = 0;
                }
                catch
                {
                    msg = msg.Remove(0);
                    errorCount = 1;
                }
            }
            return;
        }
        public static void addStringToDataBase(object obj) 
        {
            editText();
            DBcommand cmd = new DBcommand();
            cmd.addParametrs(username, date, time, temperature, soil_moisture);
            return;
        }
        public static void createGrafAboutLastDaySoil()
        {
            DBcommand cmd = new DBcommand();
            cmd.readLastDay(username, date, time, temperature, soil_moisture, number, dateArray, timeArray,
                temperatureArray, moisureArray);
            string forLabel = "[00, 01, 02, 03, 04, 05, 06, 07, 08, 0, 10, 11, 12, 13, 14," +
                "15, 16, 17, 18, 19, 20, 21, 22, 23]";
            string arraySoil = "[";
            for (int i = 0; i < 24; i++)
            {
                if (i != 23)
                {
                    arraySoil = arraySoil + (int.Parse(moisureArray[i]) - 130) + ", ";
                }
                if (i == 23)
                {
                    arraySoil = arraySoil + (int.Parse(moisureArray[i]) - 130) + "]";
                }
            }
            Chart qc = new Chart();
            qc.Width = 720;
            qc.Height = 720;
            qc.Config = @$"{{
              type: 'line',
              data: {{
                labels: {forLabel},
                datasets: [
                {{
                  label: 'Влажность почвы в процентах',
                  fill: false,
                  backgroundColor: 'rgb(54, 162, 235)',
                  borderColor: 'rgb(54, 162, 235)',
                  data: {arraySoil},
                }},
                ]
              }}
            }}";
            qc.ToFile("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/soilOfDay.png");
        }
        public static void createGrafAboutLastDayTemp()
        {
            DBcommand cmd = new DBcommand();
            cmd.readLastDay(username, date, time, temperature, soil_moisture, number, dateArray, timeArray,
                temperatureArray, moisureArray);
            string arrayTemp = "[";
            string forLabel = "[00, 01, 02, 03, 04, 05, 06, 07, 08, 0, 10, 11, 12, 13, 14," +
                "15, 16, 17, 18, 19, 20, 21, 22, 23]";
            for (int i = 0; i < 24; i++)
            {
                if (i != 23)
                { 
                    arrayTemp = arrayTemp + temperatureArray[i] + ", "; 
                }
                if (i == 23)
                { 
                    arrayTemp = arrayTemp + temperatureArray[i] + "]"; 
                }
            }
            Chart qc = new Chart();
            qc.Width = 720;
            qc.Height = 720;
            qc.Config = @$"{{
              type: 'line',
              data: {{
                labels: {forLabel},
                datasets: [{{
                  label: 'Температура в градусах Цельсия',
			      backgroundColor: 'rgb(255, 99, 132)',
			      borderColor: 'rgb(255, 99, 132)',
                  data: {arrayTemp}, 
                  fill: false,
                }},
                ]
              }}
            }}";
            qc.ToFile("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/tempOfDay.png");
        }

        public static void createGrafAboutLastWeekSoil()
        {
            DBcommand cmd = new DBcommand();
            cmd.readLastWeek(username, date, time, temperature, soil_moisture, numberOfWeek, dateArrayOfWeek, timeArrayOfWeek,
                temperatureArrayOfWeek, moisureArrayOfWeek);
            int[] arraySoilOfWeek = new int[7];
            string forLabel = "[";
            string soilOfDay = "[";
            int k = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    arraySoilOfWeek[i] = arraySoilOfWeek[i] + int.Parse(moisureArrayOfWeek[k]);
                    k++;
                }
                arraySoilOfWeek[i] = arraySoilOfWeek[i] / 24;
                if (i < 6)
                {
                    soilOfDay = soilOfDay + (arraySoilOfWeek[i] - 130) + ", ";
                }
                if (i == 6)
                {
                    soilOfDay = soilOfDay + (arraySoilOfWeek[i] - 130) + "]";
                }
            }
            for (int i = 0; i < 144; i = i + 24)
            {
                forLabel = forLabel + "'" + dateArrayOfWeek[i] + "', ";
            }
            forLabel = forLabel + "'" + dateArrayOfWeek[167] + "']";
            Chart qc = new Chart();
            qc.Width = 720;
            qc.Height = 720;
            qc.Config = @$"{{
              type: 'line',
              data: {{
                labels: {forLabel},
                datasets: [{{
                  label: 'Влажность почвы в процентах',
                  fill: false,
                  backgroundColor: 'rgb(54, 162, 235)',
                  borderColor: 'rgb(54, 162, 235)',
                  data: {soilOfDay},
                }},
                ]
              }}
            }}";
            qc.ToFile("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/soilOfWeek.png");
        }

        public static void createGrafAboutLastWeekTemp()
        {
            DBcommand cmd = new DBcommand();
            cmd.readLastWeek(username, date, time, temperature, soil_moisture, numberOfWeek, dateArrayOfWeek, timeArrayOfWeek,
                temperatureArrayOfWeek, moisureArrayOfWeek);
            int[] arrayTempOfWeek = new int[7];
            string forLabel = "[";
            string tempOfDay = "[";
            string soilOfDay = "[";
            int k = 0;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    arrayTempOfWeek[i] = arrayTempOfWeek[i] + int.Parse(temperatureArrayOfWeek[k]);
                    k++;
                }
                arrayTempOfWeek[i] = arrayTempOfWeek[i] / 24;
                if (i < 6)
                {
                    tempOfDay = tempOfDay + arrayTempOfWeek[i] + ", ";
                }
                if(i == 6)
                {
                    tempOfDay = tempOfDay + arrayTempOfWeek[i] + "]"; 
                }
            }
            for (int i = 0; i < 144; i = i + 24)
            {
                forLabel = forLabel + "'" +  dateArrayOfWeek[i] + "', ";
            }
            forLabel = forLabel + "'" + dateArrayOfWeek[167] + "']";
            Chart qc = new Chart();
            qc.Width = 720;
            qc.Height = 720;
            qc.Config = @$"{{
              type: 'line',
              data: {{
                labels: {forLabel},
                datasets: [{{
                  label: 'Температура в градусах Цельсия',
			      backgroundColor: 'rgb(255, 99, 132)',
			      borderColor: 'rgb(255, 99, 132)',
                  data: {tempOfDay}, 
                  fill: false,
                }},
                ]
              }}
            }}";
            qc.ToFile("C:/Users/ПК/source/repos/VKR_Bot/VKR_Bot/bin/Debug/net6.0/tempOfWeek.png");
        }
        public static void convertTime(object obj)
        {
            string x = DateTime.Now.ToString();
            int y = new int();
            if(x.Length == 18)
            {
                y = 10;
            }
            if(x.Length == 19)
            {
                y = 11;
            }
            x = x.Remove(0, y); 
            x = x.Remove(2, 1);
            x = x.Remove(4, 1);
            int x1 = int.Parse(x);
            if(x1 == 0 || x1 == 10000 || x1 == 20000 || x1 == 30000 || x1 == 40000 || x1 == 50000 || x1 == 60000
            || x1 == 70000 || x1 == 80000 || x1 == 90000 || x1 == 100000 || x1 == 110000 || x1 == 120000 || x1 == 130000
            || x1 == 140000 || x1 == 150000 || x1 == 160000 || x1 == 170000 || x1 == 180000 || x1 == 190000 || x1 == 200000
            || x1 == 210000 || x1 == 220000 || x1 == 230000)
            {
                addStringToDataBase(obj);
            }
        }
        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            Console.WriteLine(arg2);
            return Task.CompletedTask;
        }
    }
}
