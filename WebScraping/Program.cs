using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebScraping
{
    class Program
    {
        private static ChromiumWebBrowser browser;

        static string currentUrl = "";
        static readonly string homeUrl = "https://www.cars.com/";
        static readonly string loginUrl = "https://www.cars.com/signin/?redirect_path=%2F";
        static readonly string logoutUrl = "https://www.cars.com/signout/?redirect_path=%2F";
        static readonly string searchUrl = "https://www.cars.com/shopping/results";

        static bool isInit = false;
        static bool isLoggedIn = false;
        static bool isRedirectHomeToSearch = false;
        static bool isRedirectToSearchPage2 = false;
        static bool lastStep = false;
        static bool isFirstSearchRun = false;
        static bool isSecondSearchRun = false;
        static bool isFinished = false;
        static bool isProcessing = true;

        static List<object> vehicles = new List<object>();
        static string jsonData = "";

        static void Main(string[] args)
        {
            CefRuntime.SubscribeAnyCpuAssemblyResolver();

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            var settings = new CefSettings()
            {
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            browser = new ChromiumWebBrowser(homeUrl);

            browser.AddressChanged += Browser_AddressChanged;
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.ConsoleMessage += Browser_ConsoleMessage;

            Console.WriteLine("   ╔═══════════════════════════════════════════════════╗");
            Console.WriteLine("   ║             WEB-SCRAPING-BOT ACTIVATED            ║");
            Console.WriteLine("   ╚═══════════════════════════════════════════════════╝");

            while (isProcessing)
            {
                Thread.Sleep(1000);
            }

            Cef.Shutdown();
        }

        private static void Browser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (currentUrl == "") { }
            else if (currentUrl == homeUrl && e.Address == loginUrl)
            {
                isLoggedIn = false;
            }
            else if (currentUrl == loginUrl && e.Address == homeUrl)
            {
                isLoggedIn = true;
            }
            else if (currentUrl == logoutUrl && e.Address == homeUrl)
            {
                isLoggedIn = false;
            }
            else if (currentUrl == homeUrl && e.Address.Contains(searchUrl))
            {
                isRedirectHomeToSearch = true;
            }
            else if (e.Address.Contains(searchUrl) && isRedirectToSearchPage2 && !e.Address.Contains("page=2"))
            {
                isRedirectHomeToSearch = true;
                isRedirectToSearchPage2 = false;
                isFirstSearchRun = false;
                isSecondSearchRun = false;
                lastStep = true;
            }
            else if (e.Address.Contains(searchUrl) && e.Address.Contains("page=2"))
            {
                isRedirectHomeToSearch = false;
                isRedirectToSearchPage2 = true;
            }

            currentUrl = e.Address;
        }

        private static void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                if (isFinished)
                {
                    Console.WriteLine(jsonData);
                    Console.WriteLine("process finished");
                    Console.ReadLine();
                    isProcessing = false;
                }
                else if (currentUrl == homeUrl && !isInit)
                {
                    var loginCheck = browser.EvaluateScriptAsync(Consts.LoginCheck);
                    loginCheck.ContinueWith(x =>
                    {
                        var responseLoginCheck = x.Result;
                        if (responseLoginCheck.Success)
                        {
                            isInit = true;
                            if (isLoggedIn)
                            {
                                var logout = browser.EvaluateScriptAsync(Consts.Logout);
                                logout.ContinueWith(y =>
                                {
                                    var responseLogout = y.Result;
                                    if (responseLogout.Success) { }
                                });
                            }
                        }
                    });
                }
                else if (currentUrl == homeUrl && isInit && !isLoggedIn)
                {
                    var redirectLogin = browser.EvaluateScriptAsync(Consts.RedirectLogin);
                    redirectLogin.ContinueWith(x =>
                    {
                        var response = x.Result;
                        if (response.Success) { }
                    });
                }
                else if (currentUrl == loginUrl && !isLoggedIn)
                {
                    var login = browser.EvaluateScriptAsync(Consts.Login);
                    login.ContinueWith(x =>
                    {
                        var response = x.Result;
                        if (response.Success) { }
                    });
                }
                else if (currentUrl == homeUrl && isLoggedIn)
                {
                    var searchOnHomePage = browser.EvaluateScriptAsync(Consts.SearchOnHomePage);
                    searchOnHomePage.ContinueWith(x =>
                    {
                        var response = x.Result;
                        if (response.Success) { }
                    });
                }
                else if (currentUrl.Contains(searchUrl) && isRedirectHomeToSearch)
                {
                    var vehicleDatas = browser.EvaluateScriptAsync(Consts.GetVehicleDatas);
                    vehicleDatas.ContinueWith(x =>
                    {
                        var responseVehicleDatas = x.Result;
                        if (responseVehicleDatas.Success && responseVehicleDatas.Result != null && !isFirstSearchRun)
                        {
                            isFirstSearchRun = true;
                            vehicles.AddRange((List<object>)responseVehicleDatas.Result);

                            var redirectSecondPage = browser.EvaluateScriptAsync(Consts.RedirectSecondPage);
                            redirectSecondPage.ContinueWith(y =>
                            {
                                var responseRedirectSecondPage = y.Result;
                                if (responseRedirectSecondPage.Success) { }
                            });
                        }
                    });
                }
                else if (currentUrl.Contains(searchUrl) && isRedirectToSearchPage2)
                {
                    var vehicleDatas = browser.EvaluateScriptAsync(Consts.GetVehicleDatas);
                    vehicleDatas.ContinueWith(x =>
                    {
                        var responseVehicleDatas = x.Result;
                        if (responseVehicleDatas.Success && responseVehicleDatas.Result != null && !isSecondSearchRun)
                        {
                            isSecondSearchRun = true;
                            var result = (List<object>)responseVehicleDatas.Result;
                            vehicles.AddRange(result);

                            if (!lastStep)
                            {
                                var changeSearch = browser.EvaluateScriptAsync(Consts.ChangeSearch);
                                changeSearch.ContinueWith(y => {
                                    var responseChangeSearch = y.Result;
                                    if (responseChangeSearch.Success) { }
                                });
                            }
                            else
                            {
                                jsonData = JsonConvert.SerializeObject(vehicles);
                                isFinished = true;
                            }
                        }
                    });
                }
            }
        }

        private static void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Contains("user is logged in"))
            {
                isLoggedIn = true;
            }
            else if (e.Message.Contains("user is not logged in"))
            {
                isLoggedIn = false;
            }
            else if (e.Message.Contains("user will be logout"))
            {
                isLoggedIn = false;
            }
        }
    }
}
