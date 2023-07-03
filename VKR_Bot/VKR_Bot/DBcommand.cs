using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace VKR_Bot
{
    internal class DBcommand
    {
        async public void addParametrs(string username, string date, string time, string temperature, string soil_moisture)
        {
            DataBase db = new DataBase();
            db.ConnectSql();
            db.sqlConnection.Open();

            SqlCommand command = new SqlCommand("INSERT INTO [Table] (username, date, time," +
            "temperature, soil_moisture)" +
            "VALUES (@username, @date, @time, @temperature, @soil_moisture)", db.sqlConnection);

            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("date", date);
            command.Parameters.AddWithValue("time", time);
            command.Parameters.AddWithValue("temperature", temperature);
            command.Parameters.AddWithValue("soil_moisture", soil_moisture);

            await command.ExecuteNonQueryAsync();
            db.sqlConnection.Close();
        }

        async public void readParametrs()
        {
            DataBase db = new DataBase();
            db.ConnectSql();
            db.sqlConnection.Open();

            List<string> list = new List<string>();

            SqlCommand command = new SqlCommand("SELECT Id, username, date, time," +
            "temperature, soil_moisture FROM [Table]", db.sqlConnection); 
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                string columnName1 = reader.GetName(0);
                string columnName2 = reader.GetName(1);
                string columnName3 = reader.GetName(2);
                string columnName4 = reader.GetName(3);
                string columnName5 = reader.GetName(4);
                string columnName6 = reader.GetName(5);

                Console.WriteLine($"{columnName1}\t{columnName2}\t{columnName3}\t{columnName4}\t{columnName5}\t{columnName6}");

                while (reader.Read()) // построчно считываем данные
                {
                    int id = (int)reader.GetValue(0);
                    var username = reader.GetValue(1);
                    var date = reader.GetValue(2);
                    var time = reader.GetValue(3);
                    var temperature = reader.GetValue(4);
                    var soil_moisture = reader.GetValue(5);

                    Console.WriteLine($"{id} \t{username} \t{date} \t {time} \t {temperature} \t {soil_moisture}");
                }
            }
            reader.Close();

            await command.ExecuteNonQueryAsync();
            db.sqlConnection.Close();
            return;
        }

        async public void readLastDay(string username, string date, string time, string temperature, string soil_moisture,
            int []number, string[] dateArray, string[] timeArray, string[] temperatureArray,string[] moisureArray)
        {
            DataBase db = new DataBase();
            db.ConnectSql();
            db.sqlConnection.Open();
            int i = 24;
            int y = 0;
            while (i != 0)
            {
                SqlCommand command = new SqlCommand($"SELECT * FROM [Table] WHERE Id = (SELECT MAX(Id) {-i} FROM [Table])", db.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                i--;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = (int)reader.GetValue(0);
                        number[y] = id;
                        username = reader.GetValue(1).ToString();
                        date = reader.GetValue(2).ToString();
                        dateArray[y] = date;
                        time = reader.GetValue(3).ToString();
                        timeArray[y] = time;
                        temperature = reader.GetValue(4).ToString();
                        temperatureArray[y] = temperature;
                        soil_moisture = reader.GetValue(5).ToString();
                        moisureArray[y] = soil_moisture;
                        y++;
                    }
                }
                reader.Close();
                await command.ExecuteNonQueryAsync();
            }
            db.sqlConnection.Close();
            return;
        }
        async public void readLastWeek(string username, string date, string time, string temperature, string soil_moisture,
            int[] numberOfWeek, string[] dateArrayOfWeek, string[] timeArrayOfWeek, string[] temperatureArrayOfWeek, string[] moisureArrayOfWeek)
        {
            DataBase db = new DataBase();
            db.ConnectSql();
            db.sqlConnection.Open();
            int i = 168;
            int y = 0;
            while(i != 0)
            {
                SqlCommand command = new SqlCommand($"SELECT * FROM [Table] WHERE Id = (SELECT MAX(Id) {-i} FROM [Table])", db.sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                i--;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = (int)reader.GetValue(0);
                        numberOfWeek[y] = id;
                        username = reader.GetValue(1).ToString();
                        date = reader.GetValue(2).ToString();
                        dateArrayOfWeek[y] = date;
                        time = reader.GetValue(3).ToString();
                        timeArrayOfWeek[y] = time;
                        temperature = reader.GetValue(4).ToString();
                        temperatureArrayOfWeek[y] = temperature;
                        soil_moisture = reader.GetValue(5).ToString();
                        moisureArrayOfWeek[y] = soil_moisture;
                        y++;
                    }
                }
                reader.Close();
                await command.ExecuteNonQueryAsync();
            }
            db.sqlConnection.Close();
            return;
        }
    }
}
