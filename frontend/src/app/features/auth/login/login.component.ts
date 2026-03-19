import { CommonModule } from '@angular/common';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly isSubmitting = signal(false);
  protected readonly showPassword = signal(false);
  protected readonly serverError = signal('');

  protected readonly loginForm = this.fb.group({
    email: ['', [Validators.required, this.emailOrAdminValidator()]],
    password: ['', [Validators.required]]
  });

  protected togglePasswordVisibility(): void {
    this.showPassword.update((currentValue) => !currentValue);
  }

  protected submit(): void {
    if (this.loginForm.invalid || this.isSubmitting()) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.serverError.set('');
    this.isSubmitting.set(true);

    this.authService.login(this.loginForm.getRawValue() as { email: string; password: string; })
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.isSubmitting.set(false))
      )
      .subscribe({
        next: () => this.router.navigate(['/purchase-bill']),
        error: (error: Error) => this.serverError.set(error.message)
      });
  }

  protected hasError(controlName: 'email' | 'password', errorKey: string): boolean {
    const control = this.loginForm.get(controlName);
    return !!control && control.touched && control.hasError(errorKey);
  }

  private emailOrAdminValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = (control.value ?? '').toString().trim();
      if (!value) {
        return null;
      }

      if (value.toLowerCase() === 'admin') {
        return null;
      }

      return Validators.email(control);
    };
  }
}
