import Keycloak from 'keycloak-js';
import { Storage } from './storage';

export const AuthService = {
  authProvider: null,

  initialize: async function () {
    this.authProvider = new Keycloak();

    try {

      const authenticated = await this.authProvider.init({
        onLoad: "check-sso",
        redirectUri: "http://localhost:8000/index.php",
        checkLoginIframe: false,
        silentCheckSsoRedirectUri: `${location.origin}/silent-check-sso.html`,
        enableLogging: true,
      });

      if (authenticated) {

        Storage.setItem('token', this.authProvider.token);
        Storage.setItem('idToken', this.authProvider.idToken);
        Storage.setItem('idTokenParsed', this.authProvider.idTokenParsed);

        const userProfile = await this.authProvider.loadUserProfile();

        Storage.setItem(userProfile.id, userProfile.email);
        Storage.setItem("data_user", userProfile.email);
        Storage.setItem('userInfo', userProfile);

        this.authProvider.onTokenExpired = async function () {
          try {
            const tokenRefreshInterval = 30; // Refresh the token every 30 seconds before it expires
            const refreshed = await this.authProvider.updateToken(tokenRefreshInterval);
            if (refreshed) {
              console.log("Token refreshed successfully.");
            } else {
              console.log("Token refresh failed or the session was invalidated.");
            }
          } catch (error) {
            console.error("Token refresh error:", error);
          }
        };

        const self = this;

        document.getElementById('btnLogout')
        .addEventListener('click', async function () {
          await self.authProvider.logout();
        });

      } else {
        await this.authProvider.login();
      }
    } catch (err) {
      console.error("Initialization error", err);
    }
  },
  logout: async function () {
      Storage.clear();

      const options = {
        redirectUri: "http://localhost:8000/exit.php",
      };

      await this.authProvider.logout(options);
    },
};