import React from "react";
import Base from "../Base";

const ReadyToStart = ({ sessionId, onStatePolled, onError, apiConfig }) => {
  return (
    <Base
      title="Ready to Start"
      isLoading={true}
      loadingMessage="Waiting for start..."
      shouldPoll={true}
      pollSessionId={sessionId}
      onPollSuccess={(data) => {
        if (data.sessionId === sessionId) {
          onStatePolled(data.sessionId, data.currentState, data.input);
        }
      }}
      onPollError={(error) => {
        onError(`Error occurred while polling in ReadyToStart state: ${error}`);
      }}
      apiConfig={apiConfig}
    >
      
    </Base>
  );
};

export default ReadyToStart;
