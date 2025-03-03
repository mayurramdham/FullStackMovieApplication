import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AngularFireAuth } from '@angular/fire/compat/auth';
import firebase from 'firebase/compat/app';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private afAuth = inject(AngularFireAuth);
  constructor(private http: HttpClient) {}
  registerUser(register: any): Observable<any> {
    return this.http.post('https://localhost:7059/api/User/register', register);
  }
  loginUser(login: any): Observable<any> {
    return this.http.post('https://localhost:7059/loginUser', login);
  }

  fireBaseLogin(body: { IdToken: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    return this.http.post('https://localhost:7059/api/Auth/google', body, {
      headers,
    });
  }
  loginWithMicrosoft(body: { IdToken: string }): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post('https://localhost:7059/api/Auth/microsoft', body, {
      headers,
    });
  }
}
