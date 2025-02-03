using Sessions.Domain.Model;
using Sessions.Domain.Model.Input;
using Sessions.Domain.Model.StateMachine;
using Sessions.Domain.Model.User;
using PuppeteerSharp;

namespace Sessions.Infrastructure.Integrations.Se.Swedbank
{
    public class SeSwedbank01 : StateMachineBase
    {
        private const string LoginUrl = "https://online.swedbank.se/app/ib/logga-in";
        
        private const int AcceptCookiesButtonIntervalMs = 500;
        private const int AcceptCookiesButtonTimeoutMs = 5000;
        private const int NinInputFieldIntervalMs = 1000;
        private const int NinInputFieldTimeoutMs = 30000;
        private const int QrCodeScanIntervalMs = 2000;
        private const int QrCodeScanTimeoutMs = 60000;
        private const int WaitForSignIntervalMs = 2000;
        private const int WaitForSignTimeoutMs = 60000;

        private IBrowser? _browser;
        private IPage? _page;

        private string? _enteredNin;
        private User? _user;

        public SeSwedbank01(ISessionRepository sessionRepository, IInputRepository inputRepository, IConfiguration configuration) 
            : base(sessionRepository, inputRepository, configuration)
        {
        }

        protected override async Task DoAuthenticateAsync()
        {
            try
            {
                await SetupBrowserAsync();
                await GoToLoginPageAsync();
                await AcceptCookiesAsync();
                await WaitForAndEnterNinAsync();
                await ClickLoginButtonAsync();
                await WaitForAndCaptureQrCodeAsync();
                await WaitForSigningAsync();
                await CloseGuidedTourAsync();
                await CloseConsentPopupAsync();
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
            Console.WriteLine("Setting up browser...");
            
            try
            {
                var browserFetcher = new BrowserFetcher();
                await browserFetcher.DownloadAsync();
                var isHeadless = _configuration.GetValue<bool>("BrowserOptions:Headless");

                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = isHeadless,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" },
                    Timeout = 300000 // 5 minutes
                });

                _page = (await _browser.PagesAsync()).First();
                
                // Set viewport to MacBook Pro screen width and half size
                await _page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1440,
                    Height = 450
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to setup the browser: {ex.Message}");
            }
        }
        
        private async Task GoToLoginPageAsync()
        {
            Console.WriteLine("Navigating to login page...");
            try
            {
                await _page.GoToAsync(LoginUrl, WaitUntilNavigation.Networkidle0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error navigating to login page: {ex.Message}");
            }
        }
        
        private async Task AcceptCookiesAsync()
        {
            Console.WriteLine("Accepting cookies if prompted...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(AcceptCookiesButtonTimeoutMs);

                while (DateTime.UtcNow < timeoutAt)
                {
                    // Select the <acorn-button> element
                    var acornButton = await _page.QuerySelectorAsync("acorn-button[data-testid='acorn-button-approve-essentials']");
                    if (acornButton != null)
                    {
                        // Evaluate the shadowRoot to get the desired button
                        var shadowButtonHandle = await acornButton.EvaluateFunctionHandleAsync("element => element.shadowRoot.querySelector('div.button.submit')");
                        if (shadowButtonHandle != null && shadowButtonHandle is IElementHandle shadowButton)
                        {
                            // Click the button inside the shadow DOM
                            await shadowButton.ClickAsync();
                            Console.WriteLine("Cookies accepted.");
                            return;
                        }
                    }

                    await Task.Delay(AcceptCookiesButtonIntervalMs);
                }

                Console.WriteLine("No cookie acceptance prompt detected within the timeout.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error during cookie acceptance: {ex.Message}");
            }
        }
        
        private async Task WaitForAndEnterNinAsync()
        {
            Console.WriteLine("Waiting for NIN input field to appear...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(NinInputFieldTimeoutMs);

                IElementHandle? ninInputField = null;

                while (DateTime.UtcNow < timeoutAt)
                {
                    // Step 1: Locate the outermost shadow host element
                    var acornTextField = await _page.QuerySelectorAsync("acorn-text-field[label='Personnummer']");
                    if (acornTextField == null)
                    {
                        await Task.Delay(NinInputFieldIntervalMs);
                        continue;
                    }

                    // Step 2: Access the shadowRoot and find the input field
                    var inputHandle = await acornTextField.EvaluateFunctionHandleAsync(
                        @"element => element.shadowRoot.querySelector('input[type=""text""][placeholder=""ÅÅÅÅMMDD-XXXX""]')"
                    );

                    if (inputHandle != null)
                    {
                        ninInputField = inputHandle as IElementHandle;
                        if (ninInputField != null)
                        {
                            Console.WriteLine("NIN input field detected.");
                            break;
                        }
                    }

                    await Task.Delay(NinInputFieldIntervalMs);
                }

                if (ninInputField == null)
                    throw new TimeoutException("NIN input field did not appear within the timeout period.");

                // Request the NIN input
                await RequestInputAsync(InputRequestType.Nin, RequestParams.Empty());

                // Wait for the input to be provided
                var ninInput = await WaitForInputAsync();
                
                // Store the entered NIN
                _enteredNin = ninInput.Value;

                // Input the NIN into the field
                foreach (char character in ninInput.Value!)
                {
                    await ninInputField.TypeAsync(character.ToString());
                    await Task.Delay(new Random().Next(50, 150));
                }
                Console.WriteLine("NIN entered successfully.");

            }
            catch (Exception ex)
            {
                throw new Exception($"Error during NIN input field handling: {ex.Message}");
            }
        }

        private async Task ClickLoginButtonAsync()
        {
            Console.WriteLine("Clicking login button...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                // Locate the shadow host for the login button
                var acornButton = await _page.QuerySelectorAsync("acorn-button[data-cy='initial-login-button']");
                if (acornButton == null)
                {
                    throw new InvalidOperationException("Login button shadow host not found.");
                }

                // Access the shadow root and find the login button inside it
                var shadowButtonHandle = await acornButton.EvaluateFunctionHandleAsync(
                    @"element => element.shadowRoot.querySelector('div.button.submit')"
                );

                if (shadowButtonHandle == null)
                {
                    throw new InvalidOperationException("Login button not found within shadow DOM.");
                }

                if (shadowButtonHandle is not IElementHandle loginButton)
                {
                    throw new InvalidOperationException("Shadow DOM handle is not an element.");
                }

                // Click the login button
                await loginButton.ClickAsync();
                Console.WriteLine("Login button clicked.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error clicking login button: {ex.Message}");
            }
        }
        
        private async Task WaitForAndCaptureQrCodeAsync()
        {
            Console.WriteLine("Waiting for QR code to appear and be scanned...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(QrCodeScanTimeoutMs);
                string? latestQrCodeBase64 = null;
                var lockObject = new object(); // Ensure thread safety

                // Define the event handler to capture and process the blob URL
                void OnResponse(object? sender, ResponseCreatedEventArgs args)
                {
                    var response = args.Response;
                    var requestUrl = response.Url;

                    if (requestUrl.StartsWith("blob:https://online.swedbank.se/"))
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                // Fetch the blob content and convert to Base64
                                var buffer = await response.BufferAsync();
                                var base64 = "data:image/png;base64," + Convert.ToBase64String(buffer);

                                lock (lockObject)
                                {
                                    latestQrCodeBase64 = base64;
                                    Console.WriteLine($"QR code updated from URL: {requestUrl}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing blob response: {ex.Message}");
                            }
                        });
                    }
                }

                // Attach the response event listener
                _page.Response += OnResponse;

                try
                {
                    while (DateTime.UtcNow < timeoutAt)
                    {
                        // Is waiting for signing?
                        if (await IsWaitingForSigningAsync() || await IsLoggedInAsync())
                        {
                            Console.WriteLine("QR code successfully scanned.");
                            return;
                        }

                        string? base64ToProcess = null;
                        lock (lockObject)
                        {
                            if (!string.IsNullOrEmpty(latestQrCodeBase64))
                            {
                                base64ToProcess = latestQrCodeBase64;
                                latestQrCodeBase64 = null; // Reset to avoid re-sending
                            }
                        }

                        if (!string.IsNullOrEmpty(base64ToProcess))
                        {
                            Console.WriteLine("QR code Base64 captured. Requesting scan...");

                            // Request input for scanning the QR code
                            await RequestInputAsync(InputRequestType.Challenge, RequestParams.With("qrCodeData", base64ToProcess));
                        }

                        await Task.Delay(QrCodeScanIntervalMs);
                    }

                    throw new TimeoutException("QR code was not scanned within the timeout period.");
                }
                finally
                {
                    // Remove the response event listener to clean up resources
                    _page.Response -= OnResponse;
                    Console.WriteLine("Response listener removed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error capturing QR code: {ex.Message}");
            }
        }

        private async Task WaitForSigningAsync()
        {
            Console.WriteLine("Waiting for user to sign...");
            
            await SetInputRequestParamAsync("isScanned", "1");

            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                var timeoutAt = DateTime.UtcNow.AddMilliseconds(WaitForSignTimeoutMs);

                while (DateTime.UtcNow < timeoutAt)
                {
                    // Check if the user is logged in
                    if (await IsLoggedInAsync())
                    {
                        Console.WriteLine("User signed in successfully.");
                        await MarkInputProvidedAsync();
                        return;
                    }

                    await Task.Delay(WaitForSignIntervalMs);
                }

                throw new TimeoutException("User did not sign in within the timeout period.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while waiting for user to sign: {ex.Message}");
            }
        }
        
        private async Task CloseGuidedTourAsync()
        {
            Console.WriteLine("Checking for guided tour popover...");

            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                const int waitIntervalMs = 500;
                const int waitTimeoutMs = 10000; // Maximum wait time (10 seconds)
                var timeoutAt = DateTime.UtcNow.AddMilliseconds(waitTimeoutMs);

                IElementHandle? guidedTourHost = null;

                // Wait for the guided tour host to appear
                while (DateTime.UtcNow < timeoutAt)
                {
                    guidedTourHost = await _page.QuerySelectorAsync("acorn-guided-tour");
                    if (guidedTourHost != null)
                    {
                        // Check if the guided tour is visible
                        var isVisible = await guidedTourHost.EvaluateFunctionAsync<bool>("el => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length)");
                        if (isVisible)
                            break;
                    }

                    await Task.Delay(waitIntervalMs);
                }

                if (guidedTourHost == null)
                {
                    Console.WriteLine("Guided tour not found within the timeout period.");
                    return; // No guided tour present, nothing to close
                }

                Console.WriteLine("Guided tour detected. Attempting to close...");

                // Evaluate the shadow root of the guided tour
                var popoverHostHandle = await guidedTourHost.EvaluateFunctionHandleAsync(
                    "host => host.shadowRoot.querySelector('acorn-internal-popover#popover')"
                );

                if (popoverHostHandle == null)
                {
                    Console.WriteLine("Popover not found in guided tour.");
                    return; // No popover present
                }

                var popoverHostElement = popoverHostHandle as IElementHandle;
                if (popoverHostElement == null)
                {
                    Console.WriteLine("Failed to cast popover host to IElementHandle.");
                    return;
                }

                // Locate the "done-button" inside the popover
                var closeButtonHandle = await popoverHostElement.QuerySelectorAsync("acorn-text-button.done-button[role='button']");
                if (closeButtonHandle == null)
                {
                    Console.WriteLine("Close button not found in guided tour popover.");
                    return; // No close button found
                }

                // Click the close button
                await closeButtonHandle.ClickAsync();
                Console.WriteLine("Guided tour popover closed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while closing guided tour: {ex.Message}");
            }
        }
        
        private async Task CloseConsentPopupAsync()
        {
            Console.WriteLine("Checking for consent popup...");
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                // Wait for the dialog container to appear
                var dialogContainerSelector = "mat-dialog-container#ProfilingDigitalActivityData";
                var buttonSelector = "acorn-button[type='cancel'][role='button']";

                // Wait for the dialog to appear (timeout in 10 seconds)
                var dialogHandle = await _page.WaitForSelectorAsync(dialogContainerSelector, new WaitForSelectorOptions
                {
                    Timeout = 3000
                });

                if (dialogHandle == null)
                {
                    Console.WriteLine("Consent popup not found.");
                    return; // Exit if popup does not appear
                }

                Console.WriteLine("Consent popup detected.");

                // Find the button inside the dialog container
                var buttonHandle = await _page.QuerySelectorAsync($"{dialogContainerSelector} {buttonSelector}");
                if (buttonHandle == null)
                {
                    Console.WriteLine("Consent popup button 'Godkänn inte' not found.");
                    return;
                }

                // Click the button
                await buttonHandle.ClickAsync();
                Console.WriteLine("Consent popup closed.");
            }
            catch (WaitTaskTimeoutException)
            {
                Console.WriteLine("Consent popup did not appear within the timeout period.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while closing consent popup: {ex.Message}");
            }
        }

        private async Task GetUserInformationAsync()
        {
            Console.WriteLine("Retrieving user information...");

            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                // Selector for the shadow host (user info menu)
                const string userInfoMenuSelector = "acorn-menu[data-cy='user-info'][slot='menu']";

                // Wait for the shadow host to be present on the page
                var shadowHostHandle = await _page.WaitForSelectorAsync(userInfoMenuSelector);
                if (shadowHostHandle == null)
                    throw new Exception("User info menu not found.");

                // Access the shadow root of the shadow host
                var shadowContentHandle = await shadowHostHandle.EvaluateFunctionHandleAsync(
                    @"host => host.shadowRoot.querySelector('acorn-internal-icon-button#toggle-button.toggle[icon=""profile""]')"
                );

                if (shadowContentHandle == null)
                    throw new Exception("Profile toggle button not found inside shadow root.");

                // Extract the full name from the 'label' attribute
                var fullName = await shadowContentHandle.EvaluateFunctionAsync<string>(
                    @"button => button.getAttribute('label')"
                );

                if (string.IsNullOrWhiteSpace(fullName))
                    throw new Exception("Full name not found in profile toggle button.");

                // Create the user object with the stored NIN and retrieved full name
                if (string.IsNullOrEmpty(_enteredNin))
                    throw new Exception("NIN is not available. Ensure it was captured before retrieving user information.");

                _user = User.Create(_enteredNin, fullName);
                Session.SetUser(_user);
                Console.WriteLine($"User created: {_user.Nin}, {_user.Name}");

                // Signal successful authentication
                await FireAsync(Signal.Authenticated);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while retrieving user information: {ex.Message}");
            }
        }
        
        // Helpers
        
        private async Task<bool> IsWaitingForSigningAsync()
        {
            if (_page == null)
                throw new InvalidOperationException("Browser page is not initialized.");

            // Check if the specific element indicating waiting for signing is visible
            return await _page.EvaluateFunctionAsync<bool>(
                "() => !!document.querySelector('p[data-cy=\"verify-yourself\"]:not([hidden])')"
            );
        }
        
        private async Task<bool> IsLoggedInAsync()
        {
            try
            {
                if (_page == null)
                    throw new InvalidOperationException("Browser page is not initialized.");

                // Selector for the user info menu
                const string userInfoMenuSelector = "acorn-menu[data-cy='user-info'][slot='menu']";

                // Check if the user info menu is present on the page
                var userInfoMenuHandle = await _page.QuerySelectorAsync(userInfoMenuSelector);
                if (userInfoMenuHandle == null)
                    return false;

                // Verify if the element is visible
                var isVisible = await userInfoMenuHandle.EvaluateFunctionAsync<bool>("el => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length)");
                return isVisible;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if user is logged in: {ex.Message}");
                return false;
            }
        }
        
        private async Task<bool> IsLoggedOutDueToInactivityAsync()
        {
            Console.WriteLine("Checking if user is logged out due to inactivity...");

            if (_page == null)
                throw new InvalidOperationException("Browser page is not initialized.");

            try
            {
                return await _page.EvaluateFunctionAsync<bool>(
                    "() => !!document.querySelector('acorn-button.logout-page__button--wide[role=\"button\"][label=\"Logga in igen\"]')"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking logout status: {ex.Message}");
                return false; // Safeguard against false negatives in case of error
            }
        }

        private async Task<bool> IsLoggedOutAsync()
        {
            Console.WriteLine("Checking if user is logged out...");

            // Currently only checks inactivity but can be extended for other logout reasons
            return await IsLoggedOutDueToInactivityAsync();
        }
    }
}
