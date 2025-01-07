import React, { useState, useEffect } from "react";
import Base from "../Base";

const LoadingSession = ({ sessionId, onSuccess, onError, apiConfig }) => {
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadSession = async () => {
      try {
        setIsLoading(true);

        const response = await fetch(`${apiConfig.baseUrl}/load-session`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ sessionId: sessionId }),
        });

        if (!response.ok) {
          throw new Error("Failed to load session.");
        }

        const session = await response.json();
        onSuccess(session);
      } catch (error) {
        console.error("Error loading session:", error);
        onError("Fatal error occurred while loading session.");
      } finally {
        setIsLoading(false);
      }
    };

    loadSession();
  }, []);

  return (
    <Base
      title=""
      isLoading={isLoading}
      loadingMessage="Loading Session"
    />
  );
};

export default LoadingSession;
