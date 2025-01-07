import React from 'react';

const QrCode = ({ base64Data, isScanned }) => {
  if (!base64Data) {
    return <p>No QR code available</p>;
  }

  return (
    <div style={{ textAlign: 'center', margin: '20px' }}>
      <img
        src={base64Data}
        alt="QR Code"
        style={{
          width: '200px',
          height: '200px',
          border: '1px solid #ccc',
          opacity: isScanned ? 0.5 : 1, // Dim the QR code if scanned
        }}
      />
      {isScanned ? (
        <p style={{ color: 'green', marginTop: '10px' }}>
          QR code scanned. Please complete authentication in your BankID app.
        </p>
      ) : (
        <p style={{ color: 'blue', marginTop: '10px' }}>
          Please scan the QR code with your BankID app.
        </p>
      )}
    </div>
  );
};

export default QrCode;
