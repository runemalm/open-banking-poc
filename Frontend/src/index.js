import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
// import 'bootstrap/dist/css/bootstrap.min.css';

const config = {
  api: {
    // baseUrl: "http://localhost:5122/api"
    // baseUrl: "http://localhost:8080/api"
    baseUrl: "https://sessions-backend-102101218198.europe-north1.run.app/api"
  }
};

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <App config={config} />
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
