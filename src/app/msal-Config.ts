import { Configuration } from '@azure/msal-browser';

export const msalConfig: Configuration = {
  auth: {
    clientId: 'c861bf3d-dc76-4194-8985-7fe2e29e58ba', // Replace with your Azure AD app's client ID
    authority: 'https://login.microsoftonline.com/common', // Replace with your tenant ID
    redirectUri: 'http://localhost:4200/dashboard/home', // Replace with your redirect URI
  },
  cache: {
    cacheLocation: 'sessionStorage', // Choose between 'localStorage' or 'sessionStorage'
    storeAuthStateInCookie: true, // Set to true for IE11 or Edge compatibility
  },
};
