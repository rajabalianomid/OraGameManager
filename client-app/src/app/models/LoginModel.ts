export interface LoginModel {
    form: {
      email: string;
      password: string;
    };
    errors: {
      email?: string;
      password?: string;
    };
  }
  