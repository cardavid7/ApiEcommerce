import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

// Debe reflejar las reglas por default de ASP.NET Identity (RequireDigit,
// RequireLowercase, RequireUppercase, RequireNonAlphanumeric, RequiredLength=6)
// para no sorprender al usuario con un 400 del backend tras pasar la validación del form.
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$/;

function passwordsMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly form = this.fb.nonNullable.group(
    {
      name: ['', Validators.required],
      userName: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', Validators.required],
    },
    { validators: passwordsMatchValidator },
  );

  readonly errorMessage = signal<string | null>(null);
  readonly isSubmitting = signal(false);

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorMessage.set(null);
    this.isSubmitting.set(true);
    const { name, userName, password } = this.form.getRawValue();

    this.authService.register({ name, userName, password, role: 'User' }).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);
        if (!response.isSuccess) {
          this.errorMessage.set(response.message ?? 'No se pudo crear la cuenta.');
          return;
        }
        this.router.navigate(['/login'], { queryParams: { registered: 'true' } });
      },
      error: (error) => {
        this.isSubmitting.set(false);
        this.errorMessage.set(typeof error?.error === 'string' ? error.error : 'No se pudo crear la cuenta.');
      },
    });
  }
}
