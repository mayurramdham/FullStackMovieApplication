import { Component, inject, OnInit } from '@angular/core';
import { ToasterService } from '../../core/toaster.service';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../core/auth.service';
import { Router, RouterLink } from '@angular/router';
import { JwtServiceService } from '../../core/jwt-service.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Auth, signInWithPopup } from '@angular/fire/auth';
import { getAuth, GoogleAuthProvider } from 'firebase/auth';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from '../../msal-Config';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  msalInstance: PublicClientApplication;
  constructor(private auth: Auth) {
    this.msalInstance = new PublicClientApplication(msalConfig);
  }
  private accesstokem: string = localStorage.getItem('accessToken') || '';
  ngOnInit(): void {
    // Initialize the MSAL instance
    this.msalInstance.initialize().catch((error) => {
      console.error('Error initializing MSAL:', error);
    });
    if (this.accesstokem) this.router.navigateByUrl('/dashboard/home');
  }

  private toasterService = inject(ToasterService);
  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private router = inject(Router);
  private jwtService = inject(JwtServiceService);
  private currentRole = this.jwtService.getRole();
  private userId = this.jwtService.getUserId();
  IdToken: string = '';
  isLoading: boolean = false;
  loginValue: any = {};

  LoginData: FormGroup = new FormGroup({
    userEmail: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  loginUser() {
    this.loginValue = this.LoginData.value;
    this.isLoading = true;
    localStorage.setItem('userName', this.LoginData.get('userName')?.value);
    this.authService.loginUser(this.loginValue).subscribe({
      next: (response: any) => {
        if (response.status == 200) {
          if (response.loginRole == 1) {
            this.isLoading = true;
            this.toasterService.showSuccess(response.message);
            localStorage.setItem('accessToken', response.data);
            this.router.navigateByUrl('dashboard/home');
          } else {
            this.isLoading = true;
            this.toasterService.showSuccess(response.message);
            localStorage.setItem('accessToken', response.data);
            this.router.navigateByUrl('dashboard/home');
          }
        } else {
          this.isLoading = false;
          this.toasterService.showError(response.message);
        }
      },

      error: (error) => {
        this.isLoading = false;
        console.log('error', error);

        this.toasterService.showError(
          error.statusText == 'Unknown Error'
            ? 'Something went wrong'
            : error.error?.message
        );
      },
    });
  }

  async loginWithGoogle() {
    this.isLoading = true;
    try {
      const provider = new GoogleAuthProvider();
      const result = await signInWithPopup(this.auth, provider);
      const firebaseToken = await result.user.getIdToken();

      console.log('firebasetoken:', firebaseToken);

      const body = { IdToken: firebaseToken };

      this.authService.fireBaseLogin(body).subscribe({
        next: (response) => {
          if (response.status === 200) {
            this.isLoading = false;
            this.toasterService.showSuccess(response.message);
            localStorage.setItem('accessToken', response.data);
            this.router.navigateByUrl('dashboard/home');
          } else {
            this.isLoading = false;
            this.toasterService.showError(response.message);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.toasterService.showError(error.error?.message || error.message);
        },
      });
    } catch (error) {
      this.isLoading = false;
      console.error('Login failed:', error);
    }
  }

  loginWithMicrosoft() {
    const loginRequest = {
      scopes: ['user.read'],
    };

    this.msalInstance
      .loginPopup(loginRequest)
      .then((response) => {
        console.log('Logged in successfully', response);
        this.IdToken = response.idToken;
        console.log('IdToken', this.IdToken);

        // Now call the backend login with the retrieved token.
        const body = { IdToken: this.IdToken };
        this.authService.loginWithMicrosoft(body).subscribe({
          next: (response) => {
            this.isLoading = false;
            if (response.status === 200) {
              this.toasterService.showSuccess('Login Successful');
              localStorage.setItem('accessToken', response.data.accessToken);
              this.router.navigateByUrl('/dashboard/home');
            } else {
              this.toasterService.showError(response.message);
            }
          },
          error: (error) => {
            this.isLoading = false;
            this.toasterService.showError(
              error.error?.message || error.message
            );
          },
        });
      })
      .catch((error) => {
        console.error('Login failed', error);
        this.toasterService.showError(error.message || 'Login failed');
      });
  }
}
