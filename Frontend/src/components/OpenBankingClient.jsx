import React, { useState, useEffect, useRef } from "react";
import root from 'react-shadow';
import Error from "./Error";
import LoadingSession from "./states/LoadingSession";
import CreatingSession from "./states/CreatingSession";
import SelectBank from "./states/SelectBank";
import SelectIntegration from "./states/SelectIntegration";
import ReadyToStart from "./states/ReadyToStart";
import Authenticate from "./states/Authenticate";
import Completed from "./states/Completed";
import Failed from "./states/Failed";

const OpenBankingClient = ({ 
  existingSessionId = null, 
  sessionType = null, 
  onComplete, 
  onFailed, 
  onFatalError, 
  config 
}) => {

  useEffect(() => {
    console.log("API Base URL:", config.api.baseUrl);
  }, [config]);

  const [sessionId, setSessionId] = useState(existingSessionId || null);
  const [bankId, setBankId] = useState(null);
  const [integrationId, setIntegrationId] = useState(null);
  const [sessionState, setSessionState] = useState(existingSessionId != null ? "LoadingSession" : "CreatingSession");
  const [sessionInput, setSessionInput] = useState(null);
  const [errorMessage, setErrorMessage] = useState(null);

  const renderState = () => {

    switch (sessionState) {
      case "LoadingSession":
        return (
          <LoadingSession
            apiConfig={config.api}
            sessionId={sessionId}
            onSuccess={(session) => {
              setSessionId(session.id);
              setSessionState(session.currentState);
            }}
            onError={(errorMessage) => {
              onFatalError(errorMessage)
            }}
          />
        );
      case "CreatingSession":
        return (
          <CreatingSession
            apiConfig={config.api}
            sessionType={sessionType}
            bankId={bankId}
            integrationId={integrationId}
            onSuccess={(sessionId, sessionState, sessionInput) => {
              setSessionId(sessionId);
              setSessionState(sessionState);
              setSessionInput(sessionInput);
            }}
            onError={(errorMessage) => {
              onFatalError(errorMessage)
            }}
          />
        );
      case "SelectBank":
        return (
          <SelectBank
            apiConfig={config.api}
            sessionId={sessionId}
            onSuccess={(sessionState, bankId) => {
              setSessionState(sessionState);
              setBankId(bankId);
            }}
            onError={onFatalError}
          />
        );
      case "SelectIntegration":
        return (
          <SelectIntegration
            apiConfig={config.api}
            sessionId={sessionId}
            bankId={bankId}
            onStateChanged={(sessionState) => {
              setSessionState(sessionState);
            }}
            onSuccess={(sessionState, integrationId) => {
              setSessionState(sessionState);
              setIntegrationId(integrationId);
            }}
            onError={onFatalError}
          />
        );
      case "ReadyToStart":
        return (
          <ReadyToStart
            apiConfig={config.api}
            sessionId={sessionId}
            onStatePolled={(sessionId, sessionState, sessionInput) => {
              setSessionId(sessionId);
              setSessionState(sessionState);
              setSessionInput(sessionInput);
            }}
            onError={onFatalError}
          />
        );
      case "Authenticate":
        return (
          <Authenticate
            apiConfig={config.api}
            sessionId={sessionId}
            sessionState={sessionState}
            sessionInput={sessionInput}
            onStatePolled={(sessionId, sessionState, sessionInput) => {
              setSessionId(sessionId);
              setSessionState(sessionState);
              setSessionInput(sessionInput);
            }}
            onError={onFatalError}
          />
        );
      case "Completed":
        return (
          <Completed 
            apiConfig={config.api}
            sessionId={sessionId} 
            onComplete={onComplete} 
          />
        );
      case "Failed":
        return (
          <Failed 
            apiConfig={config.api}
            sessionId={sessionId} 
            errorMessage={errorMessage} 
            onFailed={onFailed} 
          />
        );
      default:
        return <Error errorMessage={`Unknown State: ${sessionState || "Unknown"}`} />;
    }
  };

  return (
    renderState()
  );
};

export default OpenBankingClient;
