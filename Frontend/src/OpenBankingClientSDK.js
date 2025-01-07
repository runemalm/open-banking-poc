import React from 'react';
import ReactDOM from 'react-dom/client';
import OpenBankingClient from './components/OpenBankingClient';

export default class OpenBankingClientSDK {
  constructor(apiBaseUrl = "http://localhost:5122/api") {
    this.config = {
      api: {
        baseUrl: apiBaseUrl,
      },
    };
    this.overlay = null;
    this.root = null;
  }

  openClient({ targetElementId, sessionType, onComplete, onFailed, onFatalError }) {
    let targetElement;

    if (targetElementId) {
      targetElement = document.getElementById(targetElementId);

      if (!targetElement) {
        console.error(`Target element with ID "${targetElementId}" not found.`);
        return;
      }
    } else {
      targetElement = this._createOverlay();
    }

    // Create React root and render the client
    this.root = ReactDOM.createRoot(targetElement);
    this.root.render(
      React.createElement(OpenBankingClient, {
        sessionType,
        onComplete: (session) => {
          onComplete?.(session);
          this._cleanup();
        },
        onFailed: (errorMessage) => {
          onFailed?.(errorMessage);
          this._cleanup();
        },
        onFatalError: (errorMessage) => {
          onFatalError?.(errorMessage);
          this._cleanup();
        },
        config: this.config,
      })
    );
  }

  _createOverlay() {
    // Check if an overlay already exists
    this.overlay = document.getElementById('open-banking-overlay');
    if (!this.overlay) {
      this.overlay = document.createElement('div');
      this.overlay.id = 'open-banking-overlay';
      Object.assign(this.overlay.style, {
        position: 'fixed',
        top: '0',
        left: '0',
        width: '100%',
        height: '100%',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        zIndex: '9999',
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        pointerEvents: 'none',
      });

      // Append to body
      document.body.appendChild(this.overlay);
    }

    // Create a container for the client UI
    const container = document.createElement('div');
    container.style.pointerEvents = 'auto';
    this.overlay.appendChild(container);
    return container;
  }

  _cleanup() {
    // Unmount React component
    if (this.root) {
      this.root.unmount();
      this.root = null;
    }

    // Remove overlay if it exists
    if (this.overlay) {
      document.body.removeChild(this.overlay);
      this.overlay = null;
    }
  }
}
