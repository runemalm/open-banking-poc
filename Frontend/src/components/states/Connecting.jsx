import React from "react";
import Base from "./Base";

const Connecting = ({ onNext, onCancel }) => {
  return (
    <Base onNext={onNext} onCancel={onCancel} nextDisabled>
      <div className="text-center">
        <h4>Connecting...</h4>
        <p>Please wait while we establish a connection.</p>
      </div>
    </Base>
  );
};

export default Connecting;
