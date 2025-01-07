using PuppeteerSharp;

class Program
{
    private IBrowser? _browser;
    private IPage? _page;

    static async Task Main(string[] args)
    {
        var program = new Program(); // Create an instance of Program
        await program.SetupBrowserAsync(); // Call the non-static method via the instance

        // Navigate to www.seb.se
        Console.WriteLine("Navigating to the site...");
        // await program._page!.GoToAsync("https://id.seb.se/ibp/mbid");
        await program._page!.GoToAsync("https://whatismyipaddress.com/");
        // await program._page!.GoToAsync("https://www.google.com/");

        // Keep the browser open until the user closes it
        Console.WriteLine("Browser launched. Close the browser to exit.");
        await Task.Delay(-1);
    }

    private async Task SetupBrowserAsync()
    {
        Console.WriteLine("Setting up browser...");

        try
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            // Launch Puppeteer with Proxy
            Console.WriteLine("Launching browser with proxy...");
            // var proxyAddress = "http://4.225.173.22:3128";
            // var proxyAddress = "https://a613485859ece349f8c4:4490c9d98b1d7179@gw.dataimpulse.com:823";
            var proxyAddress = "https://gw.dataimpulse.com:823";
            var proxyUsername = "a613485859ece349f8c4__cr.se";
            var proxyPassword = "4490c9d98b1d7179";
            
            


            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", $"--proxy-server={proxyAddress}", "--disable-webrtc" },
            });

            _page = (await _browser.PagesAsync()).First();
            
            // _page = await _browser.NewPageAsync();
            
            await _page.AuthenticateAsync(new Credentials
            {
                Username = proxyUsername,
                Password = proxyPassword
            });
            await _page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.82 Safari/537.36");

            // Set viewport to MacBook Pro screen width and half size
            await _page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1440,
                Height = 450
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to setup the browser: {ex.Message}");
        }
    }
}
