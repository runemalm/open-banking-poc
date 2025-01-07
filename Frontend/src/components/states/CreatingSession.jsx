import React, { useState, useEffect } from "react";
import Base from "../Base";

const CreatingSession = ({ sessionType, bankId, integrationId, onSuccess, onError, apiConfig }) => {
  const [isCreating, setIsCreating] = useState(true);

  useEffect(() => {
    const createSession = async () => {
      try {
        setIsCreating(true);

        const response = await fetch(`${apiConfig.baseUrl}/create-session`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ 
            type: sessionType,
            // bankId: "6612eb70-90c3-4864-a9cd-eb9e9e1ed6a1",  // SeSwedbank
            // integrationId: "96724b5e-48d9-4219-a01a-aeca864b7126" // SeSwedbank01
            // bankId: "2ab1b11a-ea7c-427e-bb2a-55eee987dcaa",  // SeKlarna
            // integrationId: "8ca8a737-0ac7-40b0-bc0e-bd4faf598aaa" // SeKlarna01
            // bankId: bankId,
            // integrationId: integrationId
          }),
        });

        if (!response.ok) {
          const responseData = await response.json();
          throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
        }

        const sessionResponse = await response.json();
        onSuccess(sessionResponse.sessionId, sessionResponse.currentState, sessionResponse.input);
      } catch (error) {
        console.error("Error creating session:", error);
        onError(`Error occurred while creating session: ${error.message}`);
      } finally {
        setIsCreating(false);
      }
    };

    createSession();
  }, []);

  return (
    <Base
      title=""
      isLoading={isCreating}
      loadingMessage="Creating Session"
    />
  );
};

export default CreatingSession;
