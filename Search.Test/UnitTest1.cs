using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace test
{
    public class Program
    {
        //получение результата поиска с сервера
        public static string Request(int page) 
        {
            string uri = "https://docs.microsoft.com/api/search?search=linq&locale=ru-ru&scoringprofile=search_for_en_us_a_b_test&facet=category&%24top=10&%24skip=" + page + "0&expandScope=true";
            WebRequest request = WebRequest.Create(uri);

            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                response.Close();
                return responseFromServer;
            }

        }
        //проверка наличия слова linq 
        public static bool Check(int c) 
        {
            int page = 0;
            int PageResult = 0;
            for (int j = 0; j < c; j++)
            {
                string temp = Request(page);
                int WordResult = 0;
                for (int i = 0; i < 10; i++)
                {   //парсинг полученного с сервера результата
                    string responseString = temp.Substring(temp.IndexOf("title") + 1, temp.Length - (temp.Length - temp.IndexOf("url")) - temp.IndexOf("title") - 1);
                    temp = temp.Remove(0, temp.IndexOf("displayUrl") + 1);
                    //если linq найдено, то конкретный результат удовлетворяет 
                    if (Regex.IsMatch(responseString.ToLower(), "\\blinq\\b") == true)
                        WordResult += 1; 
                }

                //если со страницы каждый результат удовлетворяет, то страница удовлетворяет 
                if (WordResult == 10) 
                    PageResult += 1;


                Console.WriteLine(PageResult);
                page += 1;


            }
            bool result = false;
            //если все страницы удовлетворяют, то тест пройден
            if (PageResult == c) 
                result = true;
            return result;

        }
    }
    public class Tests
    {
        [Test]
        public void SearchTest()
        {
            bool sum = Program.Check(5);
            Assert.AreEqual(sum, true);
        }
    }
}