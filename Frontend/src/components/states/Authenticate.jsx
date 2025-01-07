import React, { useState, useEffect, useRef, useCallback } from "react";
import Base from "../Base";
import QrCode from "../QrCode";
import InputField from "../InputField";

const Authenticate = ({ sessionId, sessionState, sessionInput, onStatePolled, onError, apiConfig }) => {
  const [inputValue, setInputValue] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isInputBeingProcessed, setIsInputBeingProcessed] = useState(false);

  const handleSubmit = async () => {
    if (inputValue.trim() === "") {
      alert("Please enter a valid value.");
      return;
    }

    setIsSubmitting(true);
    setIsInputBeingProcessed(true);

    try {
      const response = await fetch(`${apiConfig.baseUrl}/provide-input`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ sessionId, value: inputValue }),
      });

      if (!response.ok) {
        throw new Error("Failed to submit input.");
      }

      console.log("Input submitted successfully.");
    } catch (error) {
      onError(`Error submitting input: ${error.message}`);
      setIsInputBeingProcessed(false); // Allow retries on submission error
    } finally {
      setIsSubmitting(false);
    }
  };

  const primaryButton =
    sessionInput.requestType === "Nin"
      ? {
          text: "Next",
          onClick: handleSubmit,
          disabled: isSubmitting || isInputBeingProcessed,
        }
      : null;

  return (
    <Base
      title="Authenticate"
      isLoading={sessionInput.status === "NotRequested" || sessionInput.status === "Provided" || isSubmitting || isInputBeingProcessed}
      loadingMessage={(sessionInput.status === "NotRequested" && !isInputBeingProcessed) ? "Authenticating..." : ""}
      shouldPoll={true}
      pollSessionId={sessionId}
      onPollSuccess={(data) => {
        if (data.sessionId === sessionId) {
          onStatePolled(data.sessionId, data.currentState, data.input);
          setIsInputBeingProcessed(false);
        }
      }}
      onPollError={(error) => {
        onError(`Error occurred while polling in Authenticate state: ${error}`);
      }}
      primaryButton={primaryButton}
      apiConfig={apiConfig}
    >
      {sessionInput.requestType === "Challenge" && sessionInput.requestParams?.qrCodeData ? (
        <QrCode 
          base64Data={sessionInput.requestParams?.qrCodeData} 
          isScanned={sessionInput.requestParams?.isScanned ?? "0" == "1"}
        />
      ) : sessionInput.requestType === "Nin" ? (
        <InputField
          label="NIN.."
          value={inputValue}
          onChange={setInputValue}
        />
      ) : (
        <p>Waiting for input...</p>
      )}
    </Base>
  );
};

export default Authenticate;
