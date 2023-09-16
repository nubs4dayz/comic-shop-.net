using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shop.Domain.DTO;
using System.Text;

namespace Shop.Web.Controllers
{
    public class UserController : Controller
    {
        [Authorize(Roles = "Administrator")]
        [HttpGet("[action]")]
        public IActionResult ImportUsers()
        {
            return View();
        }

        [HttpPost("[action]")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ImportUsers(IFormFile file)
        {

            //make a copy
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";


            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from uploaded file

            List<UserRegistrationDto> users = getUsersFromExcelFile(file.FileName);

            HttpClient client = new HttpClient();

            string URL = "https://localhost:7060/api/Admin/ImportAllUsers";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Comics");
        }

        private List<UserRegistrationDto> getUsersFromExcelFile(string fileName)
        {

            string pathToFile = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            List<UserRegistrationDto> userList = new List<UserRegistrationDto>();

            using (var stream = System.IO.File.Open(pathToFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        userList.Add(new UserRegistrationDto
                        {
                            FirstName = reader.GetValue(0).ToString(),
                            LastName = reader.GetValue(1).ToString(),
                            Email = reader.GetValue(2).ToString(),
                            Password = reader.GetValue(3).ToString(),
                            ConfirmPassword = reader.GetValue(4).ToString(),
                            PhoneNumber = reader.GetValue(5).ToString()
                        });
                    }
                }
            }

            return userList;

        }
    }
}
