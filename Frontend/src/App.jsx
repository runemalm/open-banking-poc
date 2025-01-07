import React, { useState } from "react";
import { Navbar, Container, Nav, DropdownButton, Dropdown, Button, Form, Modal } from "react-bootstrap";
import OpenBankingClient from "./components/OpenBankingClient";
import { ToastContainer, toast, cssTransition } from "react-toastify";
// import 'bootstrap/dist/css/bootstrap.min.css';
import "react-toastify/dist/ReactToastify.css";
import "./toast.css";
const Fade = cssTransition({
  enter: "fade-toast--enter",
  exit: "fade-toast--exit",
  duration: 500,
});

const App = ({ config }) => {
  const [isClientActive, setIsClientActive] = useState(false);
  const [sessionType, setSessionType] = useState("Authenticate");
  const [clientKey, setClientKey] = useState(0);
  const [fatalError, setFatalError] = useState(null); // Track fatal error messages

  const startNewSession = () => {
    console.log(`Using Source Code. Starting a new session of type: ${sessionType}`);
    setIsClientActive(true);
    setClientKey((prevKey) => prevKey + 1);
  };

  const handleComplete = (session) => {
    console.log("Session completed");

    const createdAt = new Date(session.createdAt);
    const updatedAt = new Date(session.updatedAt);
    const durationSeconds = Math.floor((updatedAt - createdAt) / 1000);

    console.log("Session Details:");
    console.log(`- Session ID: ${session.id}`);
    console.log(`- Type: ${session.type}`);
    console.log(`- State: ${session.state}`);
    console.log(`- Created At: ${new Date(session.createdAt).toLocaleString()}`);
    console.log(`- Last Updated: ${new Date(session.updatedAt).toLocaleString()}`);
    console.log(`- Session Duration: ${durationSeconds} seconds`);
    console.log(session);

    setIsClientActive(false);

    toast.success(
      `Success!`,
      {
        autoClose: 5000,
        style: {
          backgroundColor: "#d4edda", // Green background
          color: "#155724", // Dark green text
          fontWeight: "bold",
          fontSize: "16px",
          borderRadius: "8px",
          boxShadow: "0 4px 6px rgba(0, 0, 0, 0.1)",
          width: "600px",
          maxWidth: "80%",
        },
      }
    );
  };

  const handleFailed = (errorMessage) => {
    console.log(`Session failed: errorMessage: ${errorMessage}`);
    setIsClientActive(false);
    toast.error(`${errorMessage}`, {
      autoClose: 5000,
      style: {
        backgroundColor: "#f8d7da", // Red background
        color: "#58151c", // White text
        fontWeight: "bold",
        fontSize: "16px",
        borderRadius: "8px",
        boxShadow: "0 4px 6px rgba(0, 0, 0, 0.1)",
        width: "600px",
        maxWidth: "80%"
      },
    });
  };

  const handleFatalError = (errorMessage) => {
    console.log("Fatal error occurred:", errorMessage);
    setIsClientActive(false); // Unmount the Client immediately
    toast.error(`${errorMessage}`, {
      autoClose: 5000,
      style: {
        backgroundColor: "#f8d7da", // Red background
        color: "#58151c", // White text
        fontWeight: "bold",
        fontSize: "16px",
        borderRadius: "8px",
        boxShadow: "0 4px 6px rgba(0, 0, 0, 0.1)",
        width: "600px",
        maxWidth: "80%"
      },
    });
  };

  const handleCloseFatalErrorModal = () => {
    setFatalError(null); // Clear fatal error
  };

  const sessionTypeMapping = {
    "Authenticate": "Authenticate",
    "Get Transaction History": "GetTransactionHistory",
    "Initiate Payment": "InitiatePayment",
  };

  const handleSelectSessionType = (displayName) => {
    const sessionType = sessionTypeMapping[displayName];
    setSessionType(sessionType);
  };

  return (
    <div>
      <Navbar bg="dark" variant="dark">
        <Container>
          <Navbar.Brand href="/">Test Client</Navbar.Brand>
          <Nav className="ml-auto">
            <div className="d-flex align-items-center me-3">
              <Form.Label className="text-light mb-0 me-2">Session Type:</Form.Label>
              <DropdownButton
                id="dropdown-basic-button"
                title={Object.keys(sessionTypeMapping).find(
                  (key) => sessionTypeMapping[key] === sessionType
                )}
                onSelect={handleSelectSessionType}
                variant="secondary"
              >
                <Dropdown.Header>Account Information</Dropdown.Header>
                <Dropdown.Item eventKey="Authenticate" active={sessionType === "Authenticate"}>
                  Authenticate
                </Dropdown.Item>
                <Dropdown.Item
                  eventKey="Get Transaction History"
                  active={sessionType === "GetTransactionHistory"}
                >
                  Get Transaction History
                </Dropdown.Item>
                <Dropdown.Divider />
                <Dropdown.Header>Transfers</Dropdown.Header>
                <Dropdown.Item
                  eventKey="Initiate Payment"
                  active={sessionType === "InitiatePayment"}
                >
                  Initiate Payment
                </Dropdown.Item>
              </DropdownButton>
            </div>
            <Button className="ms-3" onClick={startNewSession}>
              Start
            </Button>
          </Nav>
        </Container>
      </Navbar>

      <Container className="d-flex justify-content-center align-items-center" style={{ height: "calc(100vh - 56px)" }}>
        {isClientActive && (
          <OpenBankingClient
            key={clientKey}
            sessionType={sessionType}
            onComplete={handleComplete}
            onFailed={handleFailed}
            onFatalError={handleFatalError}
            config={config}
          />
        )}
      </Container>

      <ToastContainer
        transition={Fade}
        position="bottom-left"
        autoClose={5000}
        hideProgressBar={true}
        closeOnClick
        pauseOnHover
        draggable
        newestOnTop={true}
      />

    </div>
  );
};

export default App;
