import React, { useState, useEffect } from "react";
import Base from "../Base";
import { getApiBaseUrl } from "../../config/config";

const StartingSession = ({ sessionId, onSuccess, onFatalError }) => {
  const [isStarting, setIsStarting] = useState(true);

  useEffect(() => {
    const startSession = async () => {
      try {
        setIsStarting(true);

        const response = await fetch(`${getApiBaseUrl()}/create-session`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ sessionId: sessionId }),
        });

        if (!response.ok) {
          throw new Error("Failed to start session.");
        }

        const session = await response.json();
        onSuccess(session);
      } catch (error) {
        console.error("Error starting session:", error);
        onFatalError();
      } finally {
        setIsStarting(false);
      }
    };

    startSession();
  }, []);

  return (
    <Base
      title=""
      isLoading={isStarting}
      loadingMessage="Starting Session"
    />
  );
};

export default StartingSession;
