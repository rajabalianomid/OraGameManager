import { LoginModel } from "../models/LoginModel";

export class LoginValidator{
    private emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    validate(loginModel: LoginModel): LoginModel {
        const { form } = loginModel;
        const errors: LoginModel['errors'] = {};
    
        // Validate email
        if (!form.email.trim()) {
          errors.email = 'Please enter your email';
        } else if (!this.emailRegex.test(form.email)) {
          errors.email = 'Please enter a valid email address';
        }
    
        // Validate password
        if (!form.password.trim()) {
          errors.password = 'Please provide a password';
        } else if (form.password.length < 8) {
          errors.password = 'Password must be at least 8 characters long';
        } else if (!/[A-Z]/.test(form.password) || !/[0-9]/.test(form.password)) {
          errors.password = 'Password must contain at least one uppercase letter and one number';
        }
    
        return { ...loginModel, errors };
      }
}