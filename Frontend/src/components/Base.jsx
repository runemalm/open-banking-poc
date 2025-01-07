import React, { useState, useEffect, useRef } from "react";
import { Button, Modal, Spinner } from "react-bootstrap";

const Base = ({ 
  children, 
  title = null, 
  primaryButton = null, 
  secondaryButton = null, 
  backButton = null,
  isLoading = false, // Show loading spinner
  loadingMessage = null, // Optional loading message
  shouldPoll = false, // Determines if polling should be performed
  pollSessionId = null, // The session ID for which polling should happen
  pollInterval = 2000, // Polling interval in milliseconds
  onPollSuccess = () => {}, // Callback for successful polling
  onPollError = () => {}, // Callback for polling errors
  apiConfig = null,
}) => {
  const pollingRef = useRef(null);

  useEffect(() => {
    if (shouldPoll && pollSessionId) {
      startPolling();
    }

    return () => {
      stopPolling();
    };
  }, [shouldPoll, pollSessionId]);

  const startPolling = () => {
    console.log("Starting polling...");
    pollingRef.current = setInterval(async () => {
      try {
        const pollEndpoint = `/get-state?sessionId=${encodeURIComponent(pollSessionId)}`;
        const response = await fetch(`${apiConfig.baseUrl}${pollEndpoint}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        });

        if (!response.ok) {
          const responseData = await response.json();
          throw new Error(
            responseData.errorMessage ||
              `HTTP Error ${response.status}: ${response.statusText}`
          );
        }

        const data = await response.json();
        console.log("Polling response:", data);
        onPollSuccess(data);
      } catch (error) {
        console.error("Error during polling:", error);
        stopPolling();
        onPollError(error.message);
      }
    }, pollInterval);
  };

  const stopPolling = () => {
    console.log("Stopping polling...");
    if (pollingRef.current) {
      clearInterval(pollingRef.current);
      pollingRef.current = null;
    }
  };

  return (
    <>
      {/* Main Content */}
      <div
        style={{
          width: "300px",
          height: "450px",
          border: "1px solid #ccc",
          borderRadius: "8px",
          padding: "20px",
          display: "flex",
          flexDirection: "column",
          backgroundColor: "white",
        }}
      >
        {isLoading ? (
          /* Loading Spinner Section */
          <div
            style={{
              flexGrow: 1,
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
              alignItems: "center",
              textAlign: "center",
            }}
          >
            <Spinner animation="border" role="status" />
            {loadingMessage && <p className="mt-3">{loadingMessage}</p>}
          </div>
        ) : (
          /* Normal Layout */
          <>
            {/* Header */}
            {title && (
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: backButton ? "flex-start" : "center", // Center title if no back button
                  marginBottom: "20px",
                }}
              >
                {backButton && (
                  <Button
                    variant="link"
                    onClick={backButton.onClick}
                    disabled={backButton.disabled}
                    style={{ marginRight: "8px", padding: 0 }}
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                      strokeWidth={2}
                      stroke="currentColor"
                      style={{ width: "24px", height: "24px" }}
                    >
                      <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" />
                    </svg>
                  </Button>
                )}
                <h4 style={{ margin: 0 }}>{title}</h4>
              </div>
            )}

            {/* Content Area */}
            <div
              style={{
                flexGrow: 1,
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
              }}
            >
              {children}
            </div>

            {/* Footer */}
            <div
              className="d-flex flex-column mt-3"
              style={{
                marginBottom: !primaryButton && !secondaryButton ? "20px" : "0",
              }}
            >
              {primaryButton && (
                <Button
                  variant="primary"
                  className="mb-2"
                  onClick={primaryButton.onClick}
                  disabled={primaryButton.disabled}
                >
                  {primaryButton.text}
                </Button>
              )}
              {secondaryButton && (
                <Button
                  variant="secondary"
                  onClick={secondaryButton.onClick}
                  disabled={secondaryButton.disabled}
                >
                  {secondaryButton.text}
                </Button>
              )}
            </div>
          </>
        )}
      </div>
    </>
  );
};

export default Base;
