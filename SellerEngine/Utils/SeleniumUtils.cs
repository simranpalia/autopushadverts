using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace SellerEngine.Utils
{
    public class SeleniumUtils
    {

        public static string LoginAndPublishIBilik(string userEmail, string userPwd)
        {

            string driverPath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/");

            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverUrl"]);

            string addId = string.Empty;
            try
            {

                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");

                driver.Navigate().GoToUrl("http://www.ibilik.my/login");

                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));

                wait.Until(x => x.FindElement(By.Id("email")));

                IWebElement email = driver.FindElement(By.Id("email"));

                IWebElement pwd = driver.FindElement(By.Id("password"));

                IWebElement loginBtn = driver.FindElement(By.Id("login"));

                Actions actions = new Actions(driver);
                actions.MoveToElement(email);
                actions.Click();
                actions.SendKeys(userEmail);
                actions.Build().Perform();

                Actions actions2 = new Actions(driver);
                actions2.MoveToElement(pwd);
                actions2.Click();
                actions2.SendKeys(userPwd);
                actions2.Build().Perform();


                Actions actions3 = new Actions(driver);
                actions3.MoveToElement(loginBtn);
                actions3.Click();
                actions3.Build().Perform();

                driver.Navigate().GoToUrl("http://www.ibilik.my/dashboard");

                driver.Navigate().GoToUrl("http://www.ibilik.my/rooms/new");

                //create add,
                actions = null;
                actions = new Actions(driver);
                IWebElement location = driver.FindElement(By.Id("room_location_id"));
                actions.MoveToElement(location);
                actions.Click();
                actions.SendKeys("Johor");
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement title = driver.FindElement(By.Id("room_title"));
                actions.MoveToElement(title);
                actions.Click();
                actions.SendKeys("This is my private room");
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement description = driver.FindElement(By.Id("room_description"));
                actions.MoveToElement(description);
                actions.Click();
                actions.SendKeys("That's how my room looks like...!");
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement airCondCheckBox = driver.FindElement(By.Id("room_air_cond"));
                airCondCheckBox.Click();

                actions = null;
                actions = new Actions(driver);
                IWebElement roomPrice = driver.FindElement(By.Id("room_rental"));
                actions.MoveToElement(roomPrice);
                actions.Click();
                actions.SendKeys("250");
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                string filePath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/Car.jpg");

                IWebElement fileInput = driver.FindElement(By.Id("photo_image"));
                fileInput.SendKeys(filePath);

                actions = null;
                actions = new Actions(driver);
                IWebElement publishAdd = driver.FindElement(By.Id("submit-btn"));
                actions.MoveToElement(publishAdd);
                actions.Click();
                actions.Build().Perform();

                //pull advertisement from Url
                string advertisementrUrl = driver.Url;

                addId = Regex.Match(advertisementrUrl, @"\d+").Value;

                if (string.IsNullOrWhiteSpace(addId))
                {
                    //check for error
                    actions = null;
                    actions = new Actions(driver);
                    IWebElement errorDiv = driver.FindElement(By.ClassName("alert-error"));

                    if (errorDiv != null)
                        addId = errorDiv.Text;

                }
            }
            catch (Exception ex)
            {

            }

            finally
            {
                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");
                driver.Close();
                driver.Dispose();
            }

            return addId.ToString();
        }

        internal static string LoginAndDeleteIBilik(string userEmail, string userPwd, string addId)
        {
            string driverPath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/");

            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverUrl"]); //new ChromeDriver(driverPath); //new ChromeDriver(driverPath);

            bool hasError = false;

            try
            {

                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");

                driver.Navigate().GoToUrl("http://www.ibilik.my/login");

                //Thread.Sleep(7000);

                IWebElement email = driver.FindElement(By.Id("email"));

                IWebElement pwd = driver.FindElement(By.Id("password"));

                IWebElement loginBtn = driver.FindElement(By.Id("login"));

                Actions actions = new Actions(driver);
                actions.MoveToElement(email);
                actions.Click();
                actions.SendKeys(userEmail);
                actions.Build().Perform();

                Actions actions2 = new Actions(driver);
                actions2.MoveToElement(pwd);
                actions2.Click();
                actions2.SendKeys(userPwd);
                actions2.Build().Perform();

                Actions actions3 = new Actions(driver);
                actions3.MoveToElement(loginBtn);
                actions3.Click();
                actions3.Build().Perform();

                driver.Navigate().GoToUrl("http://www.ibilik.my/dashboard");

                driver.Navigate().GoToUrl("http://www.ibilik.my/my_rooms");

                actions = null;
                actions = new Actions(driver);
                IWebElement removeLink = driver.FindElement(By.CssSelector("a[href *= '/rooms/" + addId + "?request_uri=%2Fmy_rooms']"));
                actions.MoveToElement(removeLink);
                actions.Click();
                actions.Build().Perform();

                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();

                //DAL.DeleteAdvert(addId.ToString());

                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");

            }
            catch (Exception ex)
            {

            }
            finally
            {
                driver.Close();
                driver.Dispose();
            }

            return addId.ToString();
        }

        internal static string LoginAndPublishIBilik(string userEmail, string userPwd, AdvertModule vm, string localPath, string publicId)
        {
            string driverPath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/");

            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverUrl"]); //new ChromeDriver(driverPath); //new ChromeDriver(driverPath);

            string addId = string.Empty;
            bool hasError = false;

            try
            {

                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");

                driver.Navigate().GoToUrl("http://www.ibilik.my/login");

                IWebElement email = driver.FindElement(By.Id("email"));

                IWebElement pwd = driver.FindElement(By.Id("password"));

                IWebElement loginBtn = driver.FindElement(By.Id("login"));

                Actions actions = new Actions(driver);
                actions.MoveToElement(email);
                actions.Click();
                actions.SendKeys(userEmail);
                actions.Build().Perform();

                Actions actions2 = new Actions(driver);
                actions2.MoveToElement(pwd);
                actions2.Click();
                actions2.SendKeys(userPwd);
                actions2.Build().Perform();


                Actions actions3 = new Actions(driver);
                actions3.MoveToElement(loginBtn);
                actions3.Click();
                actions3.Build().Perform();

                driver.Navigate().GoToUrl("http://www.ibilik.my/dashboard");

                driver.Navigate().GoToUrl("http://www.ibilik.my/rooms/new");

                //create add,
                actions = null;
                actions = new Actions(driver);
                IWebElement location = driver.FindElement(By.Id("room_location_id"));
                actions.MoveToElement(location);
                actions.Click();
                actions.SendKeys(vm.Location);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement title = driver.FindElement(By.Id("room_title"));
                actions.MoveToElement(title);
                actions.Click();
                actions.SendKeys(vm.Title);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement description = driver.FindElement(By.Id("room_description"));
                actions.MoveToElement(description);
                actions.Click();
                actions.SendKeys(vm.Summary);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                if (vm.AirCond)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_air_cond"));
                    airCondCheckBox.Click();
                }
                if (vm.CookingAllowed)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_cooking"));
                    airCondCheckBox.Click();
                }
                if (vm.Internet)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_internet"));
                    airCondCheckBox.Click();
                }
                if (vm.NearKTM)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_near_train"));
                    airCondCheckBox.Click();
                }

                actions = null;
                actions = new Actions(driver);
                IWebElement roomPrice = driver.FindElement(By.Id("room_rental"));
                roomPrice.Clear();
                actions.MoveToElement(roomPrice);
                actions.Click();
                //actions.SendKeys("250");
                actions.SendKeys(vm.Price.GetValueOrDefault().ToString());
                actions.Build().Perform();

                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    actions = null;
                    actions = new Actions(driver);
                    string filePath = localPath;

                    IWebElement fileInput = driver.FindElement(By.Id("photo_image"));

                    //download file from cloud and pass it to send keys and remove it then
                    string remoteUri = localPath;
                    string localUrl = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/App_Data"), publicId + ".jpg");

                    WebClient myWebClient = new WebClient();
                    myWebClient.DownloadFile(remoteUri, localUrl);

                    fileInput.SendKeys(localUrl);
                }

                actions = null;
                actions = new Actions(driver);
                IWebElement publishAdd = driver.FindElement(By.Id("submit-btn"));
                actions.MoveToElement(publishAdd);
                actions.Click();
                actions.Build().Perform();

                //pull advertisement from Url
                string advertisementrUrl = driver.Url;

                addId = Regex.Match(advertisementrUrl, @"\d+").Value;

                if (string.IsNullOrWhiteSpace(addId))
                {
                    //check for error
                    actions = null;
                    actions = new Actions(driver);
                    IWebElement errorDiv = driver.FindElement(By.ClassName("alert-error"));

                    if (errorDiv != null)
                    {
                        addId = errorDiv.Text;
                        hasError = true;
                    }
                }

                if (!hasError)
                    vm.EntityUrl = driver.Url;

                //save to DB
                DAL.AddUpdateAdvert(vm, addId, hasError, "iBilik");

            }
            catch (Exception ex)
            {

            }

            finally
            {
                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");
                driver.Close();
                driver.Dispose();
            }

            return addId.ToString();
        }

        internal static string LoginAndEditIBilik(string userEmail, string userPwd, AdvertModule vm, string localPath)
        {
            string driverPath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/");

            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverUrl"]); //new ChromeDriver(driverPath); //new ChromeDriver(driverPath);

            string addId = string.Empty;
            bool hasError = false;

            try
            {

                driver.Navigate().GoToUrl("http://www.ibilik.my/logout?request_uri=%2F");

                driver.Navigate().GoToUrl("http://www.ibilik.my/login");

                //Thread.Sleep(7000);

                IWebElement email = driver.FindElement(By.Id("email"));

                IWebElement pwd = driver.FindElement(By.Id("password"));

                IWebElement loginBtn = driver.FindElement(By.Id("login"));

                Actions actions = new Actions(driver);
                actions.MoveToElement(email);
                actions.Click();
                actions.SendKeys(userEmail);
                actions.Build().Perform();

                Actions actions2 = new Actions(driver);
                actions2.MoveToElement(pwd);
                actions2.Click();
                actions2.SendKeys(userPwd);
                actions2.Build().Perform();


                Actions actions3 = new Actions(driver);
                actions3.MoveToElement(loginBtn);
                actions3.Click();
                actions3.Build().Perform();

                driver.Navigate().GoToUrl("http://www.ibilik.my/dashboard");

                driver.Navigate().GoToUrl("http://www.ibilik.my/rooms/" + vm.AdvertId + "/edit");

                actions = null;
                actions = new Actions(driver);
                IWebElement title = driver.FindElement(By.Id("room_title"));
                title.Clear();
                actions.MoveToElement(title);
                actions.Click();
                actions.SendKeys(vm.Title);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement description = driver.FindElement(By.Id("room_description"));
                description.Clear();
                actions.MoveToElement(description);
                actions.Click();
                actions.SendKeys(vm.Summary);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                if (vm.AirCond)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_air_cond"));
                    airCondCheckBox.Click();
                }

                if (vm.CookingAllowed)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_cooking"));
                    airCondCheckBox.Click();
                }
                if (vm.Internet)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_internet"));
                    airCondCheckBox.Click();
                }
                if (vm.NearKTM)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Id("room_near_train"));
                    airCondCheckBox.Click();
                }

                actions = null;
                actions = new Actions(driver);
                IWebElement roomPrice = driver.FindElement(By.Id("room_rental"));
                roomPrice.Clear();
                actions.MoveToElement(roomPrice);
                actions.Click();
                actions.SendKeys(vm.Price.GetValueOrDefault().ToString());
                actions.Build().Perform();

                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    actions = null;
                    actions = new Actions(driver);
                    //string filePath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/Car.jpg");
                    string filePath = localPath;

                    IWebElement fileInput = driver.FindElement(By.Id("photo_image"));
                    fileInput.SendKeys(localPath);

                    vm.PicUrl = localPath;
                }

                actions = null;
                actions = new Actions(driver);
                IWebElement publishAdd = driver.FindElement(By.Id("submit-btn"));
                actions.MoveToElement(publishAdd);
                actions.Click();
                actions.Build().Perform();

                //pull advertisement from Url
                string advertisementrUrl = driver.Url;

                addId = Regex.Match(advertisementrUrl, @"\d+").Value;

                if (string.IsNullOrWhiteSpace(addId))
                {
                    //check for error
                    actions = null;
                    actions = new Actions(driver);
                    IWebElement errorDiv = driver.FindElement(By.ClassName("alert-error"));

                    if (errorDiv != null)
                    {
                        addId = errorDiv.Text;
                        hasError = true;
                    }
                }

                //save to DB
                DAL.AddUpdateAdvert(vm, addId, hasError, "iBilik");
            }
            catch (Exception ex)
            {

            }

            finally
            {
                driver.Close();
                driver.Dispose();
            }



            return addId.ToString();
        }

        public static void LoginAndPublishMudah(string userEmail, string userPwd, string localPath, string publicId, AdvertModule vm, string myName, string myNumber)
        {
            string driverPath = System.Web.HttpContext.Current.Server.MapPath("/App_Data/");

            IWebDriver driver = new ChromeDriver(ConfigurationManager.AppSettings["ChromeDriverUrl"]);
            try
            {
                driver.Navigate().GoToUrl("https://www2.mudah.my/useraccount.html?logout=1");

                WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));

                wait.Until(x => x.FindElement(By.Id("email")));

                IWebElement email = driver.FindElement(By.Id("email"));

                IWebElement pwd = driver.FindElement(By.Id("passwd"));

                IWebElement loginBtn = driver.FindElement(By.Id("btn_signin"));

                Actions actions = new Actions(driver);
                actions.MoveToElement(email);
                actions.Click();
                actions.SendKeys(userEmail);
                actions.Build().Perform();

                Actions actions2 = new Actions(driver);
                actions2.MoveToElement(pwd);
                actions2.Click();
                actions2.SendKeys(userPwd);
                actions2.Build().Perform();

                Actions actions3 = new Actions(driver);
                actions3.MoveToElement(loginBtn);
                actions3.Click();
                actions3.Build().Perform();

                while (driver.FindElements(By.Id("category_group")).Any() == false)
                {
                    driver.Navigate().GoToUrl("https://www2.mudah.my/ai/form/0?ca=9_s");
                }


                //create add,
                actions = null;
                actions = new Actions(driver);
                IWebElement category = driver.FindElement(By.Id("category_group"));
                actions.MoveToElement(category);
                actions.Click();
                actions.SendKeys("Accommodation & Homestays");
                actions.Build().Perform();

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));

                actions = null;
                actions = new Actions(driver);
                IWebElement iAmOwner = driver.FindElement(By.Id("p_ad"));
                iAmOwner.Click();

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));

                actions = null;
                actions = new Actions(driver);
                IWebElement forRent = driver.FindElement(By.Id("ru"));
                forRent.Click();

                if (!string.IsNullOrWhiteSpace(localPath))
                {
                    actions = null;
                    actions = new Actions(driver);
                    string filePath = localPath;

                    IWebElement fileInput = driver.FindElement(By.Id("image_0"));

                    //download file from cloud and pass it to send keys and remove it then
                    string remoteUri = localPath;
                    string localUrl = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/App_Data"), publicId + ".jpg");

                    WebClient myWebClient = new WebClient();
                    myWebClient.DownloadFile(remoteUri, localUrl);

                    fileInput.SendKeys(localUrl);
                }

                Thread.Sleep(5000);
                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));

                actions = null;
                actions = new Actions(driver);
                IWebElement accomodType = driver.FindElement(By.Id("accommodation_type"));
                actions.MoveToElement(accomodType);
                actions.Click();
                actions.SendKeys("Hotel and Resorts");
                actions.Build().Perform();

                Thread.Sleep(2000);
                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));

                if (vm.AirCond)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Name("additional_facilities1"));
                    airCondCheckBox.Click();
                }
                if (vm.CookingAllowed)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Name("additional_facilities2"));
                    airCondCheckBox.Click();
                }
                if (vm.Internet)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Name("additional_facilities5"));
                    airCondCheckBox.Click();
                }
                if (vm.NearKTM)
                {
                    IWebElement airCondCheckBox = driver.FindElement(By.Name("additional_facilities3"));
                    airCondCheckBox.Click();
                }


                actions = null;
                actions = new Actions(driver);
                IWebElement title = driver.FindElement(By.Id("subject"));
                actions.MoveToElement(title);
                actions.Click();
                actions.SendKeys(vm.Title);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement description = driver.FindElement(By.Id("body"));
                actions.MoveToElement(description);
                actions.Click();
                actions.SendKeys(vm.Summary);
                actions.Build().Perform();


                actions = null;
                actions = new Actions(driver);
                IWebElement roomPrice = driver.FindElement(By.Id("monthly_rent"));
                roomPrice.Clear();
                actions.MoveToElement(roomPrice);
                actions.Click();
                actions.SendKeys(vm.Price.GetValueOrDefault().ToString());
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement location = driver.FindElement(By.Id("region"));
                actions.MoveToElement(location);
                actions.Click();
                actions.SendKeys(vm.Location);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement city = driver.FindElement(By.Id("area"));
                actions.MoveToElement(city);
                actions.Click();
                actions.SendKeys("Others");
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement name = driver.FindElement(By.Id("name"));
                actions.MoveToElement(name);
                actions.Click();
                actions.SendKeys(myName);
                actions.Build().Perform();

                actions = null;
                actions = new Actions(driver);
                IWebElement iAmProfessional = driver.FindElement(By.Id("m_ad"));
                iAmProfessional.Click();

                //actions = null;
                //actions = new Actions(driver);
                //IWebElement phoneNumber = driver.FindElement(By.Id("phone"));
                //actions.MoveToElement(phoneNumber);
                //actions.Click();
                //actions.SendKeys(myNumber);
                //actions.Build().Perform();

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));
                Thread.Sleep(2000);

                actions = null;
                actions = new Actions(driver);
                IWebElement keepLoginChat = driver.FindElement(By.Id("keeplogintochat"));
                keepLoginChat.Click();

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));
                Thread.Sleep(2000);

                if (driver.FindElements(By.Id("err_keeplogintochat")).Any())
                {
                    keepLoginChat.Click();
                }

                actions = null;
                actions = new Actions(driver);
                IWebElement publishAdd = driver.FindElement(By.Id("c_publish"));
                actions.MoveToElement(publishAdd);
                actions.Click();
                actions.Build().Perform();

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));
                Thread.Sleep(2000);

                //click no, if no picture
                if (string.IsNullOrWhiteSpace(localPath))
                {
                    actions = null;
                    actions = new Actions(driver);
                    IWebElement publishWithoutPic = driver.FindElement(By.Id("photo-notification-btn-no"));
                    actions.MoveToElement(publishWithoutPic);
                    actions.Click();
                    actions.Build().Perform();
                }

                wait.Until(d => (bool)(d as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0"));
                Thread.Sleep(2000);

                actions = null;
                actions = new Actions(driver);
                IWebElement skipAddons = driver.FindElement(By.Id("ty-link"));
                actions.MoveToElement(skipAddons);
                actions.Click();
                actions.Build().Perform();

                DAL.AddUpdateAdvert(vm, "N.A", false, "Mudah");
            }
            catch (Exception ex)
            {
                var action = new Actions(driver);
                IWebElement skipAddons = driver.FindElement(By.ClassName("error_insert"));
                var msg = skipAddons.Text;

                DAL.AddUpdateAdvert(vm, ex.Message, true, "Mudah");
            }
            finally
            {
                driver.Dispose();
            }
        }
    }
}