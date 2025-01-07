import React from "react";
import { Form } from "react-bootstrap";

const InputField = ({ label, value, onChange }) => {
  return (
    <div style={{ textAlign: "center", margin: "20px" }}>
      <Form.Group controlId="inputField" style={{ display: "inline-block", width: "100%" }}>
        <Form.Control
          type="text"
          placeholder={label} // Placeholder acts as the label
          value={value}
          onChange={(e) => onChange(e.target.value)}
        />
      </Form.Group>
    </div>
  );
};

export default InputField;
