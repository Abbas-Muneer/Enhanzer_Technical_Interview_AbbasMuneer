import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../../services/auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login']);
    authServiceSpy.login.and.returnValue(of({ success: true, message: 'ok', locations: [] }));

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('marks email and password as required', () => {
    component['submit']();

    expect(component['loginForm'].invalid).toBeTrue();
    expect(component['loginForm'].get('email')?.hasError('required')).toBeTrue();
    expect(component['loginForm'].get('password')?.hasError('required')).toBeTrue();
    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('validates email format before submission', () => {
    component['loginForm'].patchValue({
      email: 'invalid-email',
      password: 'secret'
    });

    component['submit']();

    expect(component['loginForm'].get('email')?.hasError('email')).toBeTrue();
    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('allows the admin demo username', () => {
    component['loginForm'].patchValue({
      email: 'admin',
      password: 'admin@123'
    });

    expect(component['loginForm'].valid).toBeTrue();
  });
});
