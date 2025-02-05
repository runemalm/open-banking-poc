using System.Text.Json;
using PuppeteerSharp;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.StateMachine;
using Sessions.Domain.Model.User;

namespace Sessions.Infrastructure.Integrations.Se.Klarna
{
    public class SeKlarna01 : StateMachineBase
    {
        private const string WelcomeUrl = "https://app.klarna.com/login?market=se";
        private const string OneTabPurchasesUrl = "https://app.klarna.com/manage-payments/one-tab-purchases";
        
        private IBrowser? _browser;
        private IPage? _page;
        private ILogger<SeKlarna01> _logger;

        public SeKlarna01(
            ISessionRepository sessionRepository, 
            IInputRepository inputRepository, 
            IConfiguration configuration, 
            ILogger<SeKlarna01> logger) 
            : base(sessionRepository, inputRepository, configuration)
        {
            _logger = logger;
        }

        protected override async Task DoAuthenticateAsync()
        {
            try
            {
                await SetupBrowserAsync();
                await GoToWelcomePageAsync();
                await ClickLoginButtonAsync();
                await ClickBankIdButtonAsync();
                await CaptureQrCodeAndHandleSigningInAsync();
                await GetUserInformationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Do authenticate failed: {ex.Message}");
                await FireAsync(Signal.Failed);
            }
        }

        protected override async Task DoFetchingBankAccountsAsync()
        {
            throw new NotImplementedException("Klarna do not support bank accounts.");
        }

        protected override async Task DoSelectBankAccountAsync()
        {
            throw new NotImplementedException("Klarna do not support bank accounts.");
        }

        protected override async Task DoFetchingTransactionHistoryAsync()
        {
            try
            {
                await GoToOneTabPurchasesAsync();
                await ScrapePurchasesAsync();
                await CreateTransactionHistoryFromPurchasesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Do fetching bank accounts failed: {ex.Message}");
                await FireAsync(Signal.Failed);
            }
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

        private async Task GoToWelcomePageAsync()
        {
            Console.WriteLine("Navigating to the welcome page...");
            if (_page == null)
                throw new InvalidOperationException("Browser page is not initialized.");

            await _page.GoToAsync(WelcomeUrl, WaitUntilNavigation.Networkidle0);
        }

        private async Task ClickLoginButtonAsync()
        {
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                Console.WriteLine("Waiting for the 'Sign in' button to appear...");

                // Explicit timeout for selector appearance
                var buttonTimeoutMs = 10000; // 5 seconds
                var buttonHandle = await _page.WaitForSelectorAsync("button", new WaitForSelectorOptions
                {
                    Visible = true,
                    Timeout = buttonTimeoutMs
                });

                if (buttonHandle == null)
                {
                    throw new TimeoutException($"The 'Sign in' button did not appear within {buttonTimeoutMs} ms.");
                }

                // Click the button with the exact text "Sign in"
                var buttonClicked = await _page.EvaluateFunctionAsync<bool>(@"
                    () => {
                        const buttons = Array.from(document.querySelectorAll('button'));
                        const targetButton = buttons.find(button => button.textContent.trim() === 'Sign in');
                        if (targetButton) {
                            targetButton.click();
                            return true;
                        } else {
                            console.error('Sign in button not found.');
                            return false;
                        }
                    }
                ");

                if (!buttonClicked)
                {
                    throw new InvalidOperationException("Failed to locate or click the 'Sign in' button.");
                }

                Console.WriteLine("Successfully clicked the 'Sign in' button.");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking the 'Sign in' button: {ex.Message}");
                throw;
            }
        }

        private async Task ClickBankIdButtonAsync()
        {
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                Console.WriteLine("Waiting for the page to load and display the 'Sign in with Bank ID' button...");

                // Set a timeout for navigation
                var navigationTimeoutMs = 10000; // 10 seconds
                var navigationTask = _page.WaitForNavigationAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load }
                });

                if (!navigationTask.Wait(navigationTimeoutMs))
                {
                    throw new TimeoutException("Navigation timed out.");
                }

                // Wait for the 'Sign in with Bank ID' button to appear with a timeout
                var buttonTimeoutMs = 10000; // 5 seconds
                var buttonTask = _page.WaitForSelectorAsync("#signInWithBankIdButton", new WaitForSelectorOptions { Visible = true });

                if (!buttonTask.Wait(buttonTimeoutMs))
                {
                    throw new TimeoutException("'Sign in with Bank ID' button did not appear within the timeout period.");
                }

                // Click the 'Sign in with Bank ID' button
                await _page.ClickAsync("#signInWithBankIdButton");

                Console.WriteLine("Successfully clicked the 'Sign in with Bank ID' button.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking the 'Sign in with Bank ID' button: {ex.Message}");
                throw;
            }
        }

        private async Task CaptureQrCodeAndHandleSigningInAsync()
        {
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                Console.WriteLine("Waiting for QR Code to appear...");

                // Timeout and interval configurations
                const int qrCodeCaptureTimeoutMs = 30000; // 30 seconds
                const int qrCodePollingIntervalMs = 1000; // 1 second
                const int pageNavigationTimeoutMs = 120000; // 2 minutes to navigate to the logged-in page

                var qrCodeTimeoutAt = DateTime.UtcNow.AddMilliseconds(qrCodeCaptureTimeoutMs);
                string? lastQrCodeBase64 = null;
                bool qrCodeCaptured = false; // Track if a QR code was captured

                while (DateTime.UtcNow < qrCodeTimeoutAt)
                {
                    // Query the QR code element each time to avoid stale handles
                    var qrCodeSrc = await _page.EvaluateFunctionAsync<string>(@"
                        () => {
                            const qrCodeElement = document.querySelector('img[alt=""QR Code""]');
                            return qrCodeElement ? qrCodeElement.src : null;
                        }
                    ");

                    if (string.IsNullOrEmpty(qrCodeSrc))
                    {
                        if (!qrCodeCaptured)
                        {
                            Console.WriteLine("QR Code is not visible yet. Retrying...");
                            await Task.Delay(qrCodePollingIntervalMs);
                            continue;
                        }

                        // If the QR code was captured earlier and is now gone, assume the user has signed
                        Console.WriteLine("QR Code is no longer visible. Assuming user has signed.");
                        break;
                    }

                    // Check if this QR code is new
                    if (qrCodeSrc.StartsWith("data:image/png;base64,") && qrCodeSrc != lastQrCodeBase64)
                    {
                        lastQrCodeBase64 = qrCodeSrc;
                        qrCodeCaptured = true; // Mark that we have captured a QR code
                        Console.WriteLine("New QR Code captured.");

                        // Send the QR code as challenge input
                        await RequestInputAsync(InputRequestType.Challenge, RequestParams.With("qrCodeData", qrCodeSrc));
                    }

                    await Task.Delay(qrCodePollingIntervalMs);
                }

                if (!qrCodeCaptured)
                {
                    throw new InvalidOperationException("QR Code never appeared. Cannot proceed.");
                }

                Console.WriteLine("QR Code capture process completed. Waiting for navigation to logged-in page...");

                // Wait for navigation to the logged-in page
                var navigationTimeoutAt = DateTime.UtcNow.AddMilliseconds(pageNavigationTimeoutMs);

                while (DateTime.UtcNow < navigationTimeoutAt)
                {
                    var currentUrl = _page.Url;

                    if (currentUrl.Contains("app.klarna.com"))
                    {
                        Console.WriteLine("Successfully navigated to logged-in page.");
                        await MarkInputProvidedAsync();
                        return;
                    }

                    await Task.Delay(qrCodePollingIntervalMs);
                }

                throw new TimeoutException("Page did not navigate to logged-in state within the timeout period.");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Timeout error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing QR Code: {ex.Message}");
                throw;
            }
        }
        
        private async Task GetUserInformationAsync()
        {
            const int timeoutMs = 45000;
            const int pollingIntervalMs = 500;

            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");
                
                // Wait for the network to stabilize
                Console.WriteLine("Waiting for page fully loaded.");
                await _page.WaitForNavigationAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
                });
                
                // Wait for the profile button in the <header>
                Console.WriteLine("Waiting for the profile button...");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(timeoutMs);
                bool found = false;

                while (DateTime.UtcNow < timeoutAt)
                {
                    found = await _page.EvaluateFunctionAsync<bool>(@"
                        () => {
                            const header = document.querySelector('header');
                            if (!header) return false;

                            const divs = Array.from(header.querySelectorAll('div'));
                            return divs.some(div => div.querySelectorAll('button').length === 3);
                        }
                    ");
                    if (!found) await Task.Delay(pollingIntervalMs);
                }

                if (!found)
                    throw new TimeoutException("Timed out waiting for the profile button.");

                Console.WriteLine("Profile button container detected. Attempting to click the last button...");

                bool profileButtonClicked = await _page.EvaluateFunctionAsync<bool>(@"
                    () => {
                        const header = document.querySelector('header');
                        if (!header) return false;

                        const divs = Array.from(header.querySelectorAll('div'));

                        // Find the div containing exactly 3 buttons
                        const targetDiv = divs.find(div => div.querySelectorAll('button').length === 3);
                        if (!targetDiv) return false;

                        const buttons = targetDiv.querySelectorAll('button');
                        const profileButton = buttons[buttons.length - 1];

                        if (profileButton) {
                            profileButton.click();
                            return true;
                        }

                        return false;
                    }
                ");

                if (!profileButtonClicked)
                    throw new InvalidOperationException("Failed to locate or click the profile button.");

                Console.WriteLine("Successfully clicked the profile button.");

                // Wait for the profile overlay
                Console.WriteLine("Waiting for the profile overlay...");
                await _page.WaitForSelectorAsync("div[data-testid='SettingsSection']", new WaitForSelectorOptions
                {
                    Visible = true,
                    Timeout = timeoutMs
                });

                // Click the "Account Info and Address" button
                Console.WriteLine("Clicking the 'Account Info and Address' button...");
                var accountInfoButtonClicked = await _page.EvaluateFunctionAsync<bool>(@$"
                    () => {{
                        const settingsDiv = document.querySelector('div[data-testid=""SettingsSection""]');
                        const accountInfoButton = settingsDiv?.querySelector('button:first-of-type');
                        if (!accountInfoButton) return false;

                        accountInfoButton.click();
                        return true;
                    }}
                ");
                if (!accountInfoButtonClicked)
                    throw new InvalidOperationException("Failed to locate or click the 'Account Info and Address' button.");

                // Wait for the Personal Details section
                Console.WriteLine("Waiting for the Personal Details section...");
                await _page.WaitForSelectorAsync("div[data-testid='PersonalDetailsSection']", new WaitForSelectorOptions
                {
                    Visible = true,
                    Timeout = timeoutMs
                });

                // Poll for the data within the Personal Details section
                Console.WriteLine("Waiting for the data to load in the Personal Details section...");
                var dataTimeoutAt = DateTime.UtcNow.AddMilliseconds(timeoutMs);
                (string Name, string Nin)? userInfo = null;

                while (DateTime.UtcNow < dataTimeoutAt)
                {
                    // Use EvaluateFunctionAsync<JsonElement> to get the raw result
                    var result = await _page.EvaluateFunctionAsync<JsonElement>(@$"
                        () => {{
                            const personalDetailsSection = document.querySelector('div[data-testid=""PersonalDetailsSection""]');
                            if (!personalDetailsSection) {{
                                console.log('Personal Details section not found.');
                                return {{ Name: null, Nin: null }};
                            }}

                            const regex = /^\d{{4}}-\d{{2}}-\d{{2}}$/; // Pattern for NIN (YYYY-MM-DD)
                            const spans = Array.from(personalDetailsSection.querySelectorAll('span'));

                            // Log all span contents for debugging
                            console.log(""All span texts:"", spans.map(span => span.textContent.trim()));

                            // Find the span containing the NIN
                            const ninElement = spans.find(span => {{
                                const text = span.textContent.trim();
                                console.log(`Testing span text: ""${{text}}"" against regex`);
                                return regex.test(text);
                            }});

                            if (!ninElement) {{
                                console.log('NIN element not found.');
                                return {{ Name: null, Nin: null }};
                            }}

                            const nameElement = ninElement.closest('div').parentNode.querySelector('div:nth-of-type(2)').querySelector('span')
                            if (!nameElement) {{
                                console.log('Name element not found.');
                                return {{ Name: null, Nin: null }};
                            }}

                            const name = nameElement.textContent.trim();
                            const nin = ninElement.textContent.trim();

                            console.log(`Extracted Name: ""${{name}}""`);
                            console.log(`Extracted NIN: ""${{nin}}""`);

                            // Return a plain object with both properties
                            return {{ Name: name, Nin: nin }};
                        }}
                    ");

                    // Safely extract the Name and Nin values
                    var name = result.TryGetProperty("Name", out var nameElement) && nameElement.ValueKind == JsonValueKind.String
                        ? nameElement.GetString()
                        : null;

                    var nin = result.TryGetProperty("Nin", out var ninElement) && ninElement.ValueKind == JsonValueKind.String
                        ? ninElement.GetString()
                        : null;

                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(nin))
                    {
                        userInfo = (Name: name, Nin: nin);
                        break; // Exit the loop if valid data is found
                    }

                    Console.WriteLine("Data not loaded yet. Retrying...");
                    await Task.Delay(pollingIntervalMs);
                }

                if (userInfo == null)
                    throw new InvalidOperationException("Failed to extract user information within the timeout period.");

                Console.WriteLine($"User Info: Name={userInfo.Value.Name}, NIN={userInfo.Value.Nin}");

                // Create the User object
                var user = User.Create(userInfo.Value.Nin, userInfo.Value.Name);
                Session.SetUser(user);
                Console.WriteLine($"User created: {user.Nin}, {user.Name}");
                
                await FireAsync(Signal.Authenticated);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during user information retrieval: {ex.Message}");
                throw;
            }
        }

        private async Task GoToOneTabPurchasesAsync()
        {
            throw new NotImplementedException();
        }
        
        private async Task ScrapePurchasesAsync()
        {
            throw new NotImplementedException();
        }

        private async Task CreateTransactionHistoryFromPurchasesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
