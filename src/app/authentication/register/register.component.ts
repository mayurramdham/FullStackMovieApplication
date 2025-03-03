import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { ToasterService } from '../../core/toaster.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  pateintRegisterForm: FormGroup;
  selectedFile: File | null = null;
  private authService = inject(AuthService);
  private toasterService = inject(ToasterService);
  private router = inject(Router);
  isLoading: boolean = false;
  states: any[] = [];
  countries: any[] = [];
  cities: any[] = [];
  todayDate = new Date().toISOString().split('T')[0];

  constructor(private fb: FormBuilder) {
    this.pateintRegisterForm = this.fb.group({
      userName: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(15),
        ],
      ],

      userEmail: ['', [Validators.required, Validators.email]],
      dateOfBirth: ['', Validators.required],
      mobileNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
      password: [
        '',
        [
          Validators.required,
          Validators.pattern(
            /^(?=[^A-Z]*[A-Z])(?=[^a-z]*[a-z])(?=\D*\d).{8,}$/
          ),
        ],
      ],
      roleId: ['', [Validators.required]],
    });
  }

  get passwordFormField() {
    return this.pateintRegisterForm.get('password');
  }

  isConditionMet(): boolean {
    const value = this.passwordFormField?.value;
    return (
      value?.match('^(?=.*[A-Z])') &&
      value?.match('(?=.*[a-z])') &&
      value?.match('(.*[0-9].*)') &&
      value?.match('(?=.*[!@#$%^&*])') &&
      value?.match('.{8,}')
    );
  }

  validateMobileLength(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    if (value.length > 10) {
      input.value = value.slice(0, 10);
    }
  }

  validateZipcodeLength(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    if (value.length > 6) {
      // Limit the input to 6 digits
      input.value = value.slice(0, 6);
    }
  }
  removeNumbers($event: any) {}
  onFileSelect(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.selectedFile = file; // Store the file for uploading
      console.log('file selected', this.selectedFile);
    }
  }

  onSubmit() {
    const payload = {
      userName: this.pateintRegisterForm.get('userName')?.value,
      userEmail: this.pateintRegisterForm.get('userEmail')?.value,
      dateOfBirth: this.pateintRegisterForm.get('dateOfBirth')?.value,
      mobileNumber: this.pateintRegisterForm.get('mobileNumber')?.value,
      password: this.pateintRegisterForm.get('password')?.value,
      roleId: this.pateintRegisterForm.get('roleId')?.value,
    };
    this.authService.registerUser(payload).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.isLoading = true;
          this.toasterService.showSuccess(response.message);
          this.router.navigateByUrl('/auth/login');
        } else if (response.status === 404) {
          this.isLoading = false;
          this.toasterService.showError(response.message);
        }
      },
      error: (response) => {
        this.isLoading = false;
        this.toasterService.showError(response.message);
      },
    });
  }
}
