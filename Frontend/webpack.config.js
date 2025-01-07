const path = require('path');

module.exports = {
  entry: './src/OpenBankingClientSDK.js',
  output: {
    filename: 'open-banking-client-sdk.js',
    path: path.resolve(__dirname, 'demo'),
    library: 'OpenBankingClientSDK', // Global variable name
    libraryTarget: 'umd', // Universal Module Definition
    libraryExport: 'default',
  },
  resolve: {
    extensions: ['.js', '.jsx'],
  },
  module: {
    rules: [
      {
        test: /\.css$/, // Process CSS files
        use: ['style-loader', 'css-loader'],
      },
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
              presets: [
                '@babel/preset-env', // Transpile modern JS
                ['@babel/preset-react', { runtime: 'automatic' }], // Transpile React JSX
              ],
              plugins: ['@babel/plugin-transform-runtime'],
            },
        },
      },
    ],
  },
  externals: {
    react: 'React',
    'react-dom': 'ReactDOM',
  }, 
  mode: 'development',
  devtool: "eval-source-map"
};
