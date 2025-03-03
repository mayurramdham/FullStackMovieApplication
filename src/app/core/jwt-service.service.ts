import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class JwtServiceService {
  constructor() {}

  getRole() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      console.log('Role', decodedToken.role);
      return decodedToken.role;
    }
  }
  getUserId() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      console.log('Role', decodedToken.UserId);
      console.log('apUserid', decodedToken.UserId);

      return decodedToken.UserId;
    }
  }

  getFirstName() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      return decodedToken.Name;
    }
  }

  getEmail() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      return decodedToken.email;
    }
  }
  getApiKey() {
    const accessToken = localStorage.getItem('accessToken');
    if (accessToken) {
      const decodedToken: any = jwtDecode(accessToken);
      return decodedToken.apiKey;
    }
  }
}
