import React from "react";
import Base from "../Base";

const Failed = ({ sessionId, errorMessage, apiConfig }) => {
  return (
    <Base
      title={null}
      primaryButton={null}
      secondaryButton={null}
    >
      <div style={{ textAlign: "center", marginTop: "50px" }}>
        <div
          style={{
            display: "inline-block",
            width: "100px",
            height: "100px",
            borderRadius: "50%",
            backgroundColor: "red",
            color: "white",
            fontSize: "50px",
            lineHeight: "100px",
          }}
        >
          ✗
        </div>
        <h2 style={{ marginTop: "20px", color: "red" }}>Session Failed</h2>
        {errorMessage && (
          <p style={{ marginTop: "20px", color: "gray", fontSize: "14px" }}>
            Error: {errorMessage}
          </p>
        )}
      </div>
    </Base>
  );
};

export default Failed;
