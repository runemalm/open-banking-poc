import React, { useEffect, useState } from "react";
import Base from "../Base";

const Completed = ({ sessionId, onComplete, apiConfig }) => {
  const [sessionDetails, setSessionDetails] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchSession = async () => {
      try {
        if (!sessionId) {
          throw new Error("Session ID is missing.");
        }

        const response = await fetch(`${apiConfig.baseUrl}/get-session?sessionId=${encodeURIComponent(sessionId)}`, {
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

        const session = await response.json();

        setSessionDetails(session);
      } catch (err) {
        console.error("Error fetching session:", err.message);
        setError(err.message);
      }
    };

    fetchSession();
  }, [sessionId]);

  useEffect(() => {
    if (sessionDetails && onComplete) {
      onComplete(sessionDetails);
    }
  }, [sessionDetails, onComplete]);

  return (
    <Base
      title={null}
      primaryButton={null}
      secondaryButton={null}
    >
      {!onComplete && (
        <div style={{ textAlign: "center", marginTop: "50px" }}>
          <div
            style={{
              display: "inline-block",
              width: "100px",
              height: "100px",
              borderRadius: "50%",
              backgroundColor: "green",
              color: "white",
              fontSize: "50px",
              lineHeight: "100px",
            }}
          >
            ✓
          </div>
          <h2 style={{ marginTop: "20px", color: "green" }}>Session Completed!</h2>
        </div>
      )}
    </Base>
  );
};

export default Completed;
