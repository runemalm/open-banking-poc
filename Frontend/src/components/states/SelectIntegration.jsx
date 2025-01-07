import React, { useState, useEffect } from "react";
import Base from "../Base";

const SelectIntegration = ({ sessionId, bankId, onStateChanged, onSuccess, onError, apiConfig }) => {
  const [selectedIntegrationId, setSelectedIntegrationId] = useState(null);
  const [integrations, setIntegrations] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [hasSubmitted, setHasSubmitted] = useState(false);

  useEffect(() => {
    const loadIntegrations = async () => {
      try {
        setIsLoading(true);

        const response = await fetch(`${apiConfig.baseUrl}/get-integrations?bankId=${bankId}`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        });

        if (!response.ok) {
          const responseData = await response.json();
          throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
        }

        const integrations = await response.json();
        setIntegrations(integrations);

        // Auto-select integration if only one is available
        if (integrations.length === 1) {
          const singleIntegration = integrations[0];
          setSelectedIntegrationId(singleIntegration.id);
          handleNextClick(singleIntegration.id); // Automatically proceed
        }
      } catch (error) {
        console.error("Error getting integrations:", error);
        onError(`Error occurred while getting integrations: ${error.message}`);
      } finally {
        setIsLoading(false);
      }
    };

    loadIntegrations();
  }, []);

  const handleBackClick = async () => {
    try {
      setIsSubmitting(true);

      const response = await fetch(`${apiConfig.baseUrl}/select-bank`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ 
          sessionId: sessionId,
          bankId: null,
        }),
      });

      if (!response.ok) {
        const responseData = await response.json();
        throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
      }

      setHasSubmitted(true);
      const sessionResponse = await response.json();
      onStateChanged(sessionResponse.currentState);
    } catch (error) {
      console.error("Error unselecting bank:", error);
      onError(`Error occurred while unselecting bank: ${error.message}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleNextClick = async (integrationId = selectedIntegrationId) => {
    try {
      setIsSubmitting(true);

      const response = await fetch(`${apiConfig.baseUrl}/select-integration`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ 
          sessionId: sessionId,
          integrationId: integrationId,
        }),
      });

      if (!response.ok) {
        const responseData = await response.json();
        throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
      }

      setHasSubmitted(true);
      const sessionResponse = await response.json();
      onSuccess(sessionResponse.currentState, integrationId);
    } catch (error) {
      console.error("Error selecting integration:", error);
      onError(`Error occurred while selecting integration: ${error.message}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  const formatIntegrationDisplayName = (integration) => {
    const numberMatch = integration.name.match(/\d+/); // Extracts the first number from the name
    const number = numberMatch ? numberMatch[0] : "Unknown";
    return `${number}. ${integration.clientDisplayName}`;
  };

  return (
    <Base
      title="Select Integration"
      primaryButton={{
        text: "Next",
        onClick: () => handleNextClick(),
        disabled: !selectedIntegrationId,
      }}
      backButton={{
        onClick: handleBackClick,
        disabled: isSubmitting,
      }}
      isLoading={isSubmitting || hasSubmitted}
      loadingMessage={isLoading ? "Loading integrations..." : null}
    >
      {integrations.length > 1 && ( // Only show options if there are multiple integrations
        <form>
          {integrations.map((integration) => (
            <div className="form-check mb-3" key={integration.id}>
              <input
                className="form-check-input"
                type="radio"
                name="integration"
                id={integration.id}
                value={integration.id}
                onChange={(e) => setSelectedIntegrationId(e.target.id)}
                checked={selectedIntegrationId === integration.id}
              />
              <label className="form-check-label" htmlFor={integration.id}>
                {formatIntegrationDisplayName(integration)}
              </label>
            </div>
          ))}
        </form>
      )}
    </Base>
  );
};

export default SelectIntegration;
