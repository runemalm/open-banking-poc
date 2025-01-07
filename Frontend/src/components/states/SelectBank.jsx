import React, { useState, useEffect } from "react";
import Base from "../Base";

const SelectBank = ({ sessionId, onSuccess, onError, apiConfig }) => {
  const [selectedBankId, setSelectedBankId] = useState(null);
  const [banks, setBanks] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const loadBanks = async () => {
      try {
        setIsLoading(true);

        const response = await fetch(`${apiConfig.baseUrl}/get-banks`, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        });

        if (!response.ok) {
          const responseData = await response.json();
          throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
        }

        const banks = await response.json();
        setBanks(banks);
      } catch (error) {
        console.error("Error getting banks:", error);
        onError(`Error occurred while getting banks: ${error.message}`);
      } finally {
        setIsLoading(false);
      }
    };

    loadBanks();
  }, []);

  const getBankColors = (bankName) => {
    const colorMap = {
      "Santander": { background: "#FF5733", text: "#FFFFFF" },
      "Ica Banken": { background: "#28B463", text: "#FFFFFF" },
      "Swedbank": { background: "#000000", text: "#D58534" },
      "SEB": { background: "#7AC83E", text: "#FFFFFF" },
      "Nordax Bank": { background: "#6A1B9A", text: "#FFFFFF" },
      "Coop": { background: "#469B46", text: "#FFFFFF" },
      "Wasa Kredit": { background: "#255C9B", text: "#FFFFFF" },
      "Seven Day": { background: "#FFC300", text: "#000000" },
      "Collector Bank": { background: "#A569BD", text: "#FFFFFF" },
      "Klarna": { background: "#FFFFFF", text: "#000000" },
    };

    return (
      colorMap[bankName] || {
        background: `#${Math.floor(
          bankName.split("").reduce((acc, char) => acc + char.charCodeAt(0), 0) % 16777215
        ).toString(16)}`,
        text: "#FFFFFF", // Default text color
      }
    );
  };

  const isWhiteColor = (color) => ['white', '#fff', '#ffffff'].includes(color.toLowerCase());

  const handleNextClick = async () => {
    try {
      setIsSubmitting(true);

      const response = await fetch(`${apiConfig.baseUrl}/select-bank`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          sessionId: sessionId,
          bankId: selectedBankId,
        }),
      });

      if (!response.ok) {
        const responseData = await response.json();
        throw new Error(responseData.errorMessage || `HTTP Error ${response.status}: ${response.statusText}`);
      }

      const sessionResponse = await response.json();
      onSuccess(sessionResponse.currentState, selectedBankId);
    } catch (error) {
      console.error("Error selecting bank:", error);
      onError(`Error occurred while selecting bank: ${error.message}`);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Base
      title="Select Your Bank"
      primaryButton={{
        text: "Next",
        onClick: handleNextClick,
        disabled: !selectedBankId,
      }}
      isLoading={isSubmitting}
      loadingMessage={isLoading ? "Loading banks..." : null}
    >
      <form style={{ display: "flex", flexDirection: "column", gap: "8px", width: "100%" }}>
        {banks.map((bank) => (
          <label
            key={bank.id}
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              padding: "12px 16px",
              borderRadius: "4px",
              border: selectedBankId === bank.id ? "1px solid #007bff" : "1px solid #ccc",
              backgroundColor: selectedBankId === bank.id ? "#f0f8ff" : "#fff",
              width: "100%",
              cursor: "pointer",
              transition: "background-color 0.2s, box-shadow 0.2s",
            }}
          >
            <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
              <div
                style={{
                  width: "32px",
                  height: "32px",
                  backgroundColor: getBankColors(bank.name).background,
                  color: getBankColors(bank.name).text,
                  border: `1px solid ${isWhiteColor(getBankColors(bank.name).background) ? '#ccc' : 'transparent'}`,
                  display: "flex",
                  justifyContent: "center",
                  alignItems: "center",
                  borderRadius: "50%",
                  fontSize: "14px",
                  fontWeight: "bold",
                }}
              >
                {bank.name[0]}
              </div>
              <p style={{ fontSize: "14px", margin: 0 }}>{bank.name}</p>
            </div>
            <input
              type="radio"
              name="bank"
              value={bank.id}
              checked={selectedBankId === bank.id}
              onChange={() => setSelectedBankId(bank.id)}
              style={{ marginLeft: "auto" }}
            />
          </label>
        ))}
      </form>
    </Base>
  );
};

export default SelectBank;
