using PuppeteerSharp;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.StateMachine;
using Sessions.Domain.Model.User;

namespace Sessions.Infrastructure.Integrations.Se.Seb
{
    public class SeSeb01 : StateMachineBase
    {
        private const string LoginUrl = "https://id.seb.se/ibp/mbid";
        private const string HomeUrl = "https://apps.seb.se/cps/home";
        private const string AccountsUrl = "https://apps.seb.se/cps/payments/accounts/#/accounts";
        
        private const string LoginButtonSelector = "#login_button";
        private const string QrCodeSelector = ".qrcode canvas";
        private const string ModalBodyParagraphSelector = "#modalBody_p";
        
        private const int QrCodeScanIntervalMs = 1000;
        private const int QrCodeScanTimeoutMs = 30000;
        private const int WaitForSignIntervalMs = 1000;
        private const int WaitForSignTimeoutMs = 30000;
        private const int WaitForUserInformationIntervalMs = 1000;
        private const int WaitForUserInformationTimeoutMs = 30000;

        private IBrowser? _browser;
        private IPage? _page;
        private ILogger<SeSeb01> _logger;

        public SeSeb01(
            ISessionRepository sessionRepository, 
            IInputRepository inputRepository, 
            IConfiguration configuration,
            ILogger<SeSeb01> logger) 
            : base(sessionRepository, inputRepository, configuration)
        {
            _logger = logger;

        }

        protected override async Task DoAuthenticateAsync()
        {
            try
            {
                await SetupBrowserAsync();
                await GoToLoginPageAsync();
                await ClickLoginAsync();
                await WaitForCodeToAppearAsync();
                await ContinuouslyRequestCodeScanUntilSigningDetectedAsync();
                await WaitForSigningAsync();
                await GetUserInformationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
                await FireAsync(Signal.Failed);
            }
        }

        protected override Task DoFetchingBankAccountsAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task DoSelectBankAccountAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task DoFetchingTransactionHistoryAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task CleanupAsync()
        {
            try
            {
                if (_page != null)
                {
                    await _page.CloseAsync();
                    await _page.DisposeAsync();
                }

                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    await _browser.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during browser cleanup: {ex.Message}");
            }
        }

        // Browsing
        
        private async Task SetupBrowserAsync()
        {
            _logger.LogInformation("Setting up browser...");

            try
            {
                var isHeadless = _configuration.GetValue<bool>("BrowserOptions:Headless");
                var downloadBrowser = _configuration.GetValue<bool>("BrowserOptions:DownloadBrowser");

                _logger.LogInformation("Browser options: Headless={IsHeadless}, DownloadBrowser={DownloadBrowser}", isHeadless, downloadBrowser);

                if (downloadBrowser)
                {
                    _logger.LogInformation("Downloading Chromium...");
                    var browserFetcher = new BrowserFetcher();
                    await browserFetcher.DownloadAsync();
                    _logger.LogInformation("Chromium download complete.");
                }

                var launchOptions = new LaunchOptions
                {
                    Headless = isHeadless,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" },
                    Timeout = 300000 // 5 minutes
                };

                if (!downloadBrowser)
                {
                    launchOptions.ExecutablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH") ?? "/usr/bin/chromium";
                    _logger.LogInformation("Using system-installed Chromium at {ExecutablePath}", launchOptions.ExecutablePath);
                }

                _logger.LogInformation("Launching Puppeteer...");
                _browser = await Puppeteer.LaunchAsync(launchOptions);
                _logger.LogInformation("Puppeteer launched successfully.");

                var pages = await _browser.PagesAsync();
                _page = pages.First();
                _logger.LogInformation("Connected to existing page.");

                _page.Console += (sender, args) =>
                {
                    _logger.LogInformation("[Browser Console] {BrowserMessage}", args.Message.Text);
                };

                // Set viewport to MacBook Pro screen width and half size
                await _page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1440,
                    Height = 450
                });

                _logger.LogInformation("Browser setup completed successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to setup the browser: {ex.Message}", ex);
            }
        }
        
        private async Task GoToLoginPageAsync()
        {
            Console.WriteLine("Navigating to login page...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                await _page.GoToAsync(LoginUrl, WaitUntilNavigation.Networkidle0);
                
                // Check the body content for VPN block indicators
                var pageContent = await _page.GetContentAsync();
                if (pageContent.Contains("Error: Forbidden") || pageContent.Contains("Your client does not have permission"))
                {
                    throw new Exception("Access blocked due to VPN or other restrictions.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error navigating to login page: {ex.Message}");
            }
        }

        private async Task ClickLoginAsync()
        {
            Console.WriteLine("Clicking login button...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var loginButton = await _page.QuerySelectorAsync(LoginButtonSelector);
                if (loginButton == null)
                {
                    throw new InvalidOperationException("Login button not found.");
                }

                await loginButton.ClickAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clicking login button: {ex.Message}");
            }
        }

        private async Task WaitForCodeToAppearAsync()
        {
            Console.WriteLine("Waiting for QR code to appear...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(QrCodeScanTimeoutMs);

                while (DateTime.UtcNow < timeoutAt)
                {
                    var qrCanvas = await _page.QuerySelectorAsync(QrCodeSelector);
                    if (qrCanvas != null)
                    {
                        Console.WriteLine("QR code detected.");
                        return;
                    }

                    await Task.Delay(QrCodeScanIntervalMs);
                }

                throw new TimeoutException("QR code did not appear within the timeout period.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error waiting for QR code: {ex.Message}");
            }
        }

        private async Task ContinuouslyRequestCodeScanUntilSigningDetectedAsync()
        {
            Console.WriteLine("Requesting QR code scan...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(QrCodeScanTimeoutMs);

                while (DateTime.UtcNow < timeoutAt)
                {
                    // Check if the QR code is still present
                    var qrCanvas = await _page.QuerySelectorAsync(QrCodeSelector);
                    if (qrCanvas != null)
                    {
                        var qrCodeData = await qrCanvas.EvaluateFunctionAsync<string>("canvas => canvas.toDataURL('image/png')");
                        if (!string.IsNullOrEmpty(qrCodeData))
                        {
                            await RequestInputAsync(InputRequestType.Challenge, RequestParams.With("qrCodeData", qrCodeData));

                            Console.WriteLine("QR code scan requested.");
                        }
                    }

                    // Check if signing is detected
                    var modalBodyP = await _page.QuerySelectorAsync(ModalBodyParagraphSelector);
                    if (modalBodyP != null)
                    {
                        var message = await modalBodyP.EvaluateFunctionAsync<string>("node => node.textContent");
                        if (message.Trim() == "Skriv in din kod i BankID-appen och välj Legitimera eller Skriv under.")
                        {
                            Console.WriteLine("Signing detected.");
                            return;
                        }
                    }
                    
                    // Check for the signed-in indicator
                    var sessionTimeoutElement = await _page.QuerySelectorAsync("shell-site-session-timeout[user-id]");
                    if (sessionTimeoutElement != null)
                    {
                        var userId = await sessionTimeoutElement.EvaluateFunctionAsync<string>("node => node.getAttribute('user-id')");
                        if (!string.IsNullOrEmpty(userId))
                        {
                            Console.WriteLine($"User signed in successfully with user-id: {userId}");
                            return;
                        }
                    }

                    await Task.Delay(QrCodeScanIntervalMs);
                }

                throw new TimeoutException("Signing not detected within the timeout period.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during QR code scan request: {ex.Message}");
            }
        }

        private async Task WaitForSigningAsync()
        {
            Console.WriteLine("Waiting for user to sign...");
            
            await SetInputRequestParamAsync("isScanned", "1");
            
            var timeoutAt = DateTime.UtcNow.AddMilliseconds(WaitForSignTimeoutMs);
            while (DateTime.UtcNow < timeoutAt)
            {
                await Task.Delay(WaitForSignIntervalMs);

                try
                {
                    // Check for the signed-in indicator
                    var sessionTimeoutElement = await _page.QuerySelectorAsync("shell-site-session-timeout[user-id]");
                    if (sessionTimeoutElement != null)
                    {
                        var userId = await sessionTimeoutElement.EvaluateFunctionAsync<string>("node => node.getAttribute('user-id')");
                        if (!string.IsNullOrEmpty(userId))
                        {
                            Console.WriteLine($"User signed in successfully with user-id: {userId}");
                            await MarkInputProvidedAsync();
                            return;
                        }
                    }
                }
                catch (PuppeteerException ex) when (ex.Message.Contains("Execution context was destroyed"))
                {
                    Console.WriteLine("Execution context destroyed while waiting for signing, probably we were " +
                                      "redirected to the logged-in page. Ignoring...");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while waiting for user to sign: {ex.Message}");
                }
            }

            // Timeout if the user does not sign in within the specified time
            throw new Exception("Timed out waiting for the user to sign.");
        }

        private async Task GetUserInformationAsync()
        {
            Console.WriteLine("Retrieving user information...");

            var timeoutAt = DateTime.UtcNow.AddMilliseconds(WaitForUserInformationTimeoutMs);

            try
            {
                // Get the user-id from the shell-site-session-timeout element
                var sessionTimeoutElement = await _page.QuerySelectorAsync("shell-site-session-timeout[user-id]");
                if (sessionTimeoutElement == null)
                {
                    throw new InvalidOperationException("User ID element not found.");
                }

                var userId = await sessionTimeoutElement.EvaluateFunctionAsync<string>("node => node.getAttribute('user-id')");
                if (string.IsNullOrEmpty(userId) || userId.Length < 10)
                {
                    throw new InvalidOperationException("Invalid or missing user-id.");
                }

                // Extract Nin (first 10 digits of user-id)
                var nin = userId.Substring(0, 10);

                // Poll for the full name container within the shadow DOM
                string fullName = string.Empty;
                while (DateTime.UtcNow < timeoutAt)
                {
                    var shadowHost = await _page.QuerySelectorAsync("body > header > seb-shell-header > div > seb-shell-user-dropdown > seb-shell-dropdown-core > seb-shell-header-item");
                    if (shadowHost != null)
                    {
                        fullName = await shadowHost.EvaluateFunctionAsync<string>(@"
                            host => {
                                if (host.shadowRoot) {
                                    const textWrapper = host.shadowRoot.querySelector('.seb-shell-header-item__text-wrapper');
                                    if (textWrapper) {
                                        return Array.from(textWrapper.querySelectorAll('span'))
                                                    .map(span => span.textContent.trim())
                                                    .join(' ');
                                    }
                                }
                                return '';
                            }
                        ");

                        if (!string.IsNullOrEmpty(fullName))
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Name container not found or name empty in shadow DOM. Retrying...");
                    await Task.Delay(WaitForUserInformationIntervalMs);
                }

                if (string.IsNullOrEmpty(fullName))
                {
                    throw new InvalidOperationException("Name is missing or could not be retrieved within the timeout period.");
                }

                // Create User object
                var user = User.Create(nin, fullName);
                Session.SetUser(user);
                Console.WriteLine($"User created: {user.Nin}, {user.Name}");

                await FireAsync(Signal.Authenticated);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving user information: {ex.Message}");
            }
        }
    }
}
