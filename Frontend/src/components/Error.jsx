import React from "react";
import { Alert } from "react-bootstrap";
import Base from "./Base";

const Error = ({ errorMessage }) => {
  return (
    <Base nextDisabled>
      <Alert variant="danger" className="text-center">
        {errorMessage}
      </Alert>
    </Base>
  );
};

export default Error;
